Shader "My Shaders/New Cloud Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_WeatherMap("Weather Map", 2D) = "white" {}

		_BoundsMax("Max Height", Vector) = (0,0,0)
		_BoundsMin("Min Height", Vector) = (0,0,0)

		_Gc("Global cloud coverage", float) = 0.5
		_Gd("Global cloud density (0 to infinity)", float) = 1

		_WeatherScale("Weather Scale", float) = 1
		_NumberOfSteps("Number of steps", Range(1,200)) = 20

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

		_SunColour("Sun Colour", Color) = (1,1,1,1)
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

			float3 _BoundsMin;
			float3 _BoundsMax;

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
				float3 uvw = (position - _PlanetPos) * _WeatherScale * 0.001;
				return _WeatherMap.Sample(sampler_WeatherMap, uvw.xz);
			}

			float4 SampleShapeNoise(float3 position) {
				float3 uvw = (position - _PlanetPos) * _ShapeNoiseScale * 0.001;
				return _ShapeNoise.SampleLevel(sampler_ShapeNoise, uvw, 0);
			}

			float4 SampleDetailNoise(float3 position) {
				float3 uvw = (position - _PlanetPos) * _DetailNoiseScale * 0.001;
				return _DetailNoise.SampleLevel(sampler_DetailNoise, uvw, 0);
			}

			float Density(float3 pos) {
				float4 Wc = SampleMap(pos);
				float WMc = max(Wc.r, (_Gc - 0.5) * Wc.g * 2);

				float Ph = (pos.y - _BoundsMin.y) / (_BoundsMax.y - _BoundsMin.y);
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
				float dist = BoxCollision(pos, dir, _BoundsMin, _BoundsMax).y;

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

			float CalculateLight(float3 position) {
				float totalDensity = DensityAlongSunRay(position);

				float transmittance = exp(-totalDensity * _SunAbsorption);
				return _DarknessThreshold + transmittance * (1 - _DarknessThreshold);
			}

			float phase(float a) {
				float hgBlend = HG(a, _InS) * (1 - _IvO) + HG(a, -_OutS) * _IvO;
				return _AMin + hgBlend * _PhaseFactor;
			}

			float2 GetColour(float3 origin, float3 direction, float distance) {
				_NumberOfSteps = 40;
				
				float3 position = origin;
				float stepSize = distance / (_NumberOfSteps - 1);

				float light = 0;
				float transmittance = 1;

				float cosAngle = dot(-direction, normalize(_SunPosition - origin));
				float phaseVal = phase(cosAngle);

				[unroll(40)]
				for(int i = 0 ; i < _NumberOfSteps; i++ ){
					float density = Density(position);
					if (density > 0) {
						float lightResult = CalculateLight(position);

						light += density * stepSize * transmittance * lightResult * phaseVal;
						transmittance *= exp(-density * stepSize * _CloudAbsorption);
					}
					
					position += direction * stepSize;
				}

				return float2(light, transmittance);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);

				float viewLength = length(i.viewDir);
				float3 dir = i.viewDir / viewLength;
				float3 origin = _WorldSpaceCameraPos;

				float depth = DECODE_EYEDEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * viewLength;

				// Find out if the current pixel collides with the atmosphere
				float2 result = BoxCollision(origin, dir, _BoundsMin, _BoundsMax);

				float dstToAtmosphere = result.x;
				float dstThroughAtmosphere = min(result.y, depth - dstToAtmosphere);

				float offset = (_BlueNoise.Sample(sampler_BlueNoise, i.uv * 10) - 0.5) * 2;
				offset *= 5.1;

				float3 entry = origin + dir * (dstToAtmosphere + offset);

				if (dstThroughAtmosphere > 0) {
					float2 result = GetColour(entry, dir, dstThroughAtmosphere);

					float4 newColour = _SunColour * result.r;

					col.rgb *= result.g;
					col.rgb += newColour.rgb;
				}

				return col;
            }
            ENDCG
        }
    }
}
