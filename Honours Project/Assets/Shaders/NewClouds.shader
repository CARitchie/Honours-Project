Shader "My Shaders/New Cloud Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_WeatherMap("Weather Map", 2D) = "white" {}

		_Gd("Global cloud density (0 to infinity)", float) = 1

		_WeatherScale("Weather Scale", float) = 1

		_ShapeNoise("Shape Noise", 3D) = "white" {}
		_DetailNoise("Detail Noise", 3D) = "white" {}

		_ShapeNoiseScale("Shape Noise Scale", float) = 1
		_DetailNoiseScale("Detail Noise Scale", float) = 1

		_BlueNoise("Blue Noise", 2D) = "white" {}

		_InS("In-Scatter amount", Range(0.0,1.0)) = 1
		_OutS("Out-Scatter amount", Range(0.0,1.0)) = 1
		_IvO("In vs Out Scatter", Range(0.0,1.0)) = 0.5

		_SunAbsorption("Sun Absorption", float) = 1.2
		_CloudAbsorption("Cloud Absorption", float) = 0.75
		_PhaseFactor("Phase Factor", Range(0.0,1.0)) = 0.488
		_AMin("Minimum attenuation ambient", Range(0.0,1.0)) = 0.2
		_DarknessThreshold("Darkness Threshold", Range(0.0,1.0)) = 0.2

		_BlueNoisePower("Blue Noise Power", float) = 5.1
		_BlueNoiseScale("Blue Noise Scale", float) = 10

		_BlueDrop("Blue Drop", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float3 viewDir = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
				o.viewDir = mul(unity_CameraToWorld, float4(viewDir, 0));
				return o;
            }

            sampler2D _MainTex;
			sampler2D _CameraDepthTexture;

			float _Gc;	// Global coverage term
			float _Gd;	// Global density term

			Texture2D _WeatherMap;
			SamplerState sampler_WeatherMap;

			float _WeatherScale;

			int _NumberOfSteps;

			Texture3D<float4> _ShapeNoise;
			SamplerState sampler_ShapeNoise;

			Texture3D<float4> _DetailNoise;
			SamplerState sampler_DetailNoise;

			float _ShapeNoiseScale;
			float _DetailNoiseScale;

			Texture2D _BlueNoise;
			SamplerState sampler_BlueNoise;

			float3 _SunPosition;

			float _InS;
			float _OutS;
			float _IvO;

			float _CloudAbsorption;

			float _SunAbsorption;
			float _PhaseFactor;
			float _AMin;

			float _DarknessThreshold;

			float4 _SunColour;

			float3 _PlanetPos;

			float _MinHeight;
			float _MaxHeight;

			float _BlueNoisePower;
			float _BlueNoiseScale;

			float2 _WeatherOffset;

			float _StartSunSet;
			float _EndSunSet;
			float _StartDarkness;
			float _EndDarkness;

			float _BlueDrop;

			float2 SphereToSquare(float3 position) {
				float3 pos = normalize(position - _PlanetPos);

				float pi = 3.14159;

				float x = 0.5 + atan2(pos.x, pos.z) / (2 * pi);
				float y = 0.5 - asin(pos.y) / pi;
				return float2(x, y);
			}

			float2 squareUV(float2 uv) {
				float width = _ScreenParams.x;
				float height = _ScreenParams.y;
				//float minDim = min(width, height);
				float scale = 1000;
				float x = uv.x * width;
				float y = uv.y * height;
				return float2 (x / scale, y / scale);
			}

			float2 BoxCollision(float3 position, float3 direction, float3 boundsMin, float3 boundsMax) {
				float3 t0 = (_PlanetPos + boundsMin - position) / direction;
				float3 t1 = (_PlanetPos + boundsMax - position) / direction;
				float3 tmin = min(t0, t1);
				float3 tmax = max(t0, t1);

				float dstA = max(max(tmin.x, tmin.y), tmin.z);
				float dstB = min(tmax.x, min(tmax.y, tmax.z));

				float dstToBox = max(0, dstA);
				float dstInsideBox = max(0, dstB - dstToBox);
				return float2(dstToBox, dstInsideBox);
			}

			float2 SphereCollision(float3 position, float3 direction, float3 sphereCentre, float sphereRadius)
			{
				// Used https://link.springer.com/content/pdf/10.1007%2F978-1-4842-4427-2_7.pdf

				float3 f = position - sphereCentre;

				float a = dot(direction, direction);
				float b = 2 * dot(f, direction);
				float c = dot(f, f) - (sphereRadius * sphereRadius);

				float discriminant = (b*b) - 4 * a * c;

				// If the ray intersects more than once
				if (discriminant > 0) {
					float root = sqrt(discriminant);

					float val1 = (-b - root) / (2 * a);
					float val2 = (-b + root) / (2 * a);

					// If the entry point is behind, set entry point to ray origin
					if (val1 < 0) val1 = 0;

					if (val2 >= 0) return float2(val1, val2);
				}

				return float2(-1, -1);
			}

			float R(float v, float Lo, float Ho, float Ln, float Hn) {
				return Ln + (((v - Lo) * (Hn - Ln)) / (Ho - Lo));
			}

			float SAT(float v) {
				if (v < 0) return 0;
				if (v > 1) return 1;
				return v;
			}

			float Li(float Vo, float V1, float iVAL) {
				return (1 - iVAL) * Vo + iVAL * V1;
			}

			float4 SampleMap(float3 position) {
				float2 uvw = (SphereToSquare(position) + _WeatherOffset) * _WeatherScale * 0.001;
				return _WeatherMap.Sample(sampler_WeatherMap, uvw.xy);
			}

			float4 SampleShapeNoise(float3 position) {
				float3 uvw = (position - _PlanetPos) * _ShapeNoiseScale * 0.001;
				return _ShapeNoise.SampleLevel(sampler_ShapeNoise, uvw, 0);
			}

			float4 SampleDetailNoise(float3 position) {
				float3 uvw = (position - _PlanetPos) * _DetailNoiseScale * 0.001;
				return _DetailNoise.SampleLevel(sampler_DetailNoise, uvw, 0);
			}

			float SquareMag(float3 vec) {
				return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;
			}

			float PercentHeight(float3 pos) {
				float height = distance(pos, _PlanetPos);
				return (height - _MinHeight) / (_MaxHeight - _MinHeight);
			}

			float Density(float3 pos) {

				float Ph = PercentHeight(pos);
				if (Ph <= 0) return 0;

				float4 Wc = SampleMap(pos);
				float WMc = max(Wc.r, (_Gc - 0.5) * Wc.g * 2);

				float Wh = Wc.b;	// Weather map height;
				float Wd = 1;		// Weather map density, should be Wc.a

				float SRb = SAT(R(Ph, 0.0, 0.07, 0.0, 1.0));
				float SRt = SAT(R(Ph, Wh * 0.2, Wh, 1.0, 0.0));

				float SA = SRb * SRt;

				float DRb = Ph * SAT(R(Ph, 0.0, 0.15, 0.0, 1.0));
				float DRt = SAT(R(Ph, 0.9, 1.0, 1.0, 0.0));
				float DA = _Gd * DRb * DRt * Wd * 2;

				float4 sn = SampleShapeNoise(pos);
				
				float SNsample = R(sn.r, (sn.g * 0.625 + sn.b * 0.25) - 1, 1.0, 0.0, 1.0);
				
				float4 dn = SampleDetailNoise(pos);
				float DNfbm = dn.r * 0.625 + dn.g * 0.25 + dn.b * 0.125;

				float DNmod = 0.35 * exp(-_Gc * 0.75) * Li(DNfbm, 1 - DNfbm, SAT(Ph * 5.0));
				float SNnd = SAT(R(SNsample * SA, 1 - _Gc * WMc, 1, 0, 1));

				float d = SAT(R(SNnd, DNmod, 1.0, 0.0, 1.0)) * DA;

				return d;
			}

			float DensityAlongSunRay(float3 rayOrigin) {
				int SunSteps = 10;

				float3 pos = rayOrigin;
				float3 dir = normalize(_SunPosition - pos);
				float dist = SphereCollision(pos, dir, _PlanetPos, _MaxHeight).y;

				float step = dist / (SunSteps - 1);
				float density = 0;

				//[unroll(10)]
				for (int i = 0; i < SunSteps; i++) {
					density += max(0, Density(pos) * step);
					pos += dir * step;
				}

				return density;
			}

			float HG(float theta, float g) {
				float pi = 1 / (4 * 3.14159);
				float square = g * g;
				return pi * ((1 - square) / (pow(1 + square - 2 * g*theta, 1.5)));
			}

			float4 ColourLerp(float4 col1, float4 col2, float percent) {
				percent = saturate(percent);
				float4 col3 = float4(1, 1, 1, 1);

				col3.r = lerp(col1.r, col2.r, percent);
				col3.g = lerp(col1.g, col2.g, percent);
				col3.b = lerp(col1.b, col2.b, percent);

				return col3;
			}

			float4 AlteredColour(float3 pos) {
				float4 alteredColour = float4(1, 1, 1, 1);

				float angle = dot(normalize(_PlanetPos - pos), normalize(_SunPosition - pos));

				if (angle >= _EndDarkness) return float4(0, 0, 0, 0);
				if (angle >= _StartDarkness) {
					angle = (angle - _StartDarkness) / (_EndDarkness - _StartDarkness);
					return ColourLerp(_SunColour, float4(0, 0, 0, 0), angle);
				}
				if (angle >= _EndSunSet) return _SunColour;

				if (angle >= _StartSunSet) {
					angle = (angle - _StartSunSet) / (_EndSunSet - _StartSunSet);

					return ColourLerp(alteredColour, _SunColour, angle);
				}

				return alteredColour;
			}

			float CalculateLight(float3 position) {
				float totalDensity = DensityAlongSunRay(position);

				float transmittance = exp(-totalDensity * _SunAbsorption);
				return (_DarknessThreshold + transmittance * (1 - _DarknessThreshold));
			}

			float phase(float a) {
				float hgBlend = HG(a, _InS) * (1 - _IvO) + HG(a, -_OutS) * _IvO;
				return _AMin + hgBlend * _PhaseFactor;
			}

			float4 GetColour(float3 origin, float3 direction, float distance) {
				_NumberOfSteps = 35;
				
				float3 position = origin;

				float stepSize = 0;
				stepSize = distance / (_NumberOfSteps - 1);

				float3 light = 0;
				float transmittance = 1;

				float3 sunDir = normalize(_SunPosition - origin);
				float cosAngle = dot(-direction, sunDir);
				float phaseVal = phase(cosAngle);

				float progress = 0;

				[unroll(35)]
				for(int i = 0 ; i < _NumberOfSteps; i++ ){
					float density = Density(position);
					if (density > 0) {
						float lightResult = CalculateLight(position);

						float4 alteredColour = AlteredColour(position);

						light += density * stepSize * transmittance * lightResult * phaseVal * alteredColour;
						transmittance *= exp(-density * stepSize * _CloudAbsorption);
						if (transmittance < 0.01) break;
					}
					
					position += direction * stepSize;
					progress += stepSize;
				}

				return float4(light.x, light.y, light.z, transmittance);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);

				float viewLength = length(i.viewDir);
				float3 dir = i.viewDir / viewLength;
				float3 origin = _WorldSpaceCameraPos;

				float depth = DECODE_EYEDEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * viewLength;

				// Find out if the current pixel collides with the atmosphere
				float2 result = SphereCollision(origin, dir, _PlanetPos, _MaxHeight);

				float dstToAtmosphere = result.x;
				float dstThroughAtmosphere = min(result.y - result.x, depth - dstToAtmosphere);

				if (dstThroughAtmosphere > 0) {

					float properScale = ((dstToAtmosphere + 1)/ _BlueDrop) * _BlueNoiseScale;

					float offset = _BlueNoise.SampleLevel(sampler_BlueNoise, squareUV(i.uv * properScale), 0);
					offset *= _BlueNoisePower;

					float2 innerResult = SphereCollision(origin, dir, _PlanetPos, _MinHeight);

					if (innerResult.x == 0) {
						float innerDistance = innerResult.y - innerResult.x;

						if (depth < innerDistance) return col;

						dstToAtmosphere = innerDistance;
						dstThroughAtmosphere = result.y - dstToAtmosphere;
						dstThroughAtmosphere = min(dstThroughAtmosphere, depth - dstToAtmosphere);
					}
					
					float3 entry = origin + dir * (dstToAtmosphere + offset);

					float4 colourResult = GetColour(entry, dir, dstThroughAtmosphere);

					float3 newColour = colourResult.rgb;

					col.rgb *= colourResult.a;
					col.rgb += newColour.rgb;
				}

				return col;
            }
            ENDCG
        }
    }
}
