Shader "My Shaders/Cloud Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WeatherMap ("Weather Map", 2D) = "white" {}
        _BoundsMax("Max Height", Vector) = (0,0,0)
        _BoundsMin("Min Height", Vector) = (0,0,0)
        
		_Coverage("Global cloud coverage", Range(0.0, 1.0)) = 0.5
        _Density("Global Density", float) = 1
        _AnvilAmount("Anvil Amount",float) = 0
        _CloudScale("Cloud Scale", float) = 1
        _NumberOfSteps("Number Of Steps", Range(0,100)) = 10
        _Balance("Absorption",float) = 1
        _CSi("Extra in scatter intensity",float) = 10
        _CSe("How centralized is intensity",float) = 20
        _ins("In scatter amount", Range(0.0,1.0)) = 1
        _outs("Out scatter amount", Range(0.0,1.0)) = 1
        _ivo("Inscatter vs Outscatter", Range(0.0,1.0)) = 0.5
        _MaxDensity("Maximum Density",float) = 1

		_NoiseTex("Noise Tex", 3D) = "white" {}
		_NoiseScale("Noise Scale", float) = 1
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
            Texture2D _WeatherMap;

            SamplerState sampler_WeatherMap;

			Texture3D<float4> Noise;
			SamplerState samplerNoise;

            float3 _BoundsMin;
            float3 _BoundsMax;
            float3 _SunPosition;

            float _Coverage;
            float _Density;
            float _CloudScale;
            float _AnvilAmount;
            float _Balance;

            float _CSi;
            float _CSe;

            float _ins;
            float _outs;
            float _ivo;

            float _MaxDensity;
			float _NoiseScale;

            int _NumberOfSteps;

            float CloudProbability(float4 data){
                return max(data.r,saturate(_Coverage-0.5) * data.g * 2);
            }

            float4 GetData(float3 position){
                //float3 uvw = (position - _PlanetOrigin) * _CloudScale * 0.001;
                float3 uvw = position * _CloudScale * 0.001;
                return _WeatherMap.Sample(sampler_WeatherMap, uvw.xz);
            }

            float2 BoxCollision(float3 position, float3 direction, float3 boundsMin, float3 boundsMax){
                float3 t0 = (boundsMin - position) / direction;
                float3 t1 = (boundsMax - position) / direction;
                float3 tmin = min(t0,t1);
                float3 tmax = max(t0,t1);

                float dstA = max(max(tmin.x,tmin.y),tmin.z);
                float dstB = min(tmax.x,min(tmax.y,tmax.z));

                float dstToBox = max(0, dstA);
                float dstInsideBox = max(0, dstB - dstToBox);
                return float2(dstToBox,dstInsideBox);
            }

            float Remap(float val, float lo, float ho, float ln, float hn){
                return ln + ((val - lo) * (hn - ln)) / (ho - lo);
            }

            float Li(float v0, float v1, float iVal){
                return (1 - iVal) * v0 + iVal * v1;
            }

            float GetHeightPercent(float3 pos){
                return (pos.y - _BoundsMin.y) / ( _BoundsMax.y - _BoundsMin.y);
            }

            float E(float b, float ds){
                return exp(-b * ds);
            }

            float HG(float theta, float g){
                float square = pow(g,2);

                float val1 = 1 / (4 * 3.14159);
                float val2 = (1 - square) / pow((1 + square - 2 * g * cos(theta)),3/2);
                return val1 * val2;
            }

            float ISextra(float theta){
                return _CSi * pow(saturate(theta),_CSe);
            }

            float IOS(float theta){
                return Li( max( HG(theta, _ins), ISextra(theta) ), HG(theta, -_outs), _ivo);
            }

            float OSambient(float density, float height){
                float _OSa = 0.9;
                return 1 - saturate(_OSa * pow(density, Remap(height,0.3,0.9,0.5,1.0))) * saturate(pow(Remap(height,0,0.3,0.8,1.0),0.8));
            }

            float Aclamp(float densityToSun){
                float b = 12;
                float ac = 0.2;

                return max(E(b, densityToSun), E(b,ac));
            }

            float Aalter(float densityAtPoint, float densityToSun){
                float amin = 0.2;
                return max(densityAtPoint * amin, densityToSun);
            }

            float Lfinal(float theta, float densityAtPoint, float densityToSun, float height){
                return Aalter(densityAtPoint, densityToSun) * IOS(theta) * OSambient(densityAtPoint, height);
            }

			float4 GetNoise(float3 pos) {
				float3 uvw = pos * _NoiseScale;

				float r = Noise.SampleLevel(samplerNoise, uvw, 0);
				float g = Noise.SampleLevel(samplerNoise, uvw * 0.6, 0);
				float b = Noise.SampleLevel(samplerNoise, uvw * 0.3, 0);
				float a = Noise.SampleLevel(samplerNoise, uvw * 0.1, 0);

				return float4(r, g, b, a);
			}

            float Density(float3 pos){
                float4 data = GetData(pos);

                float WMc = CloudProbability(data);

                float height = GetHeightPercent(pos);
                float SRb = saturate(Remap(height,0,0.07,0,1));
                float SRt = saturate(Remap(height,data.b * 0.2,data.b,1,0));

                float SA = SRb * SRt;
                //SA = pow(SA, saturate(Remap(height,0.65,0.95,1,1-_AnvilAmount * _Coverage)));

                float DRb = height * saturate(Remap(height,0,0.15,0,1));
                float DRt = saturate(Remap(height,0.9,1.0,1,0));

                float DA = _Density * DRb * DRt * 2;

				float4 noiseData = GetNoise(pos);
				float SNsample = Remap(noiseData.r, (noiseData.g * 0.625 + noiseData.b * 0.25 + noiseData.a * 0.125) - 1, 1, 0, 1);

                return saturate(Remap(SNsample * SA, 1 - _Coverage * WMc,1,0,1)) * DA;
            }

            float DensityAlongSunRay(float3 rayOrigin){
                int SunSteps = 10;

                float3 pos = rayOrigin;
                float3 dir = -normalize(_SunPosition - pos);
                float dist = BoxCollision(pos, dir, _BoundsMin,_BoundsMax).y;

				float step = dist / (SunSteps - 1);
				float density = 0;

				for(int i = 0 ; i < SunSteps ; i++){
                    density += Density(pos);
					pos += dir * step;
				}

                return density;
            }

            float AngleToSun(float3 origin, float3 pos){
                float3 dir1 = origin - pos;
                float3 dir2 = _SunPosition - pos;
                float cosTheta = (dot(dir1,dir2) / (length(dir1) * length(dir2)));

                return acos(cosTheta);
            }

            float4 CalculateLight(float3 rayOrigin, float3 dir, float dist){

                _NumberOfSteps = 20;

				float3 pos = rayOrigin;
				float step = dist / (_NumberOfSteps - 1);
				float value = 0;

                float densities[30];
                float totalDensity = 0;

				for(int i = 0 ; i < _NumberOfSteps ; i++){

                    densities[i] = Density(pos);                    
					pos += dir * step;
				}

                pos = rayOrigin;
                for(int j = 0; j < _NumberOfSteps; j++){
                    if(densities[j] > 0){
                        totalDensity += densities[j];
                        float sunRayDensity = DensityAlongSunRay(pos);
                        //float light = E(_Balance,sunRayDensity);
                        value += Lfinal(AngleToSun(rayOrigin - dir,pos), densities[j], sunRayDensity, GetHeightPercent(pos));
                    }
                    pos += dir * step;
                }

                value = clamp(value,0,1);
                return float4(value,value,value,totalDensity);
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

				if(dstThroughAtmosphere > 0){
                    float3 entryPos = origin + dstToAtmosphere * dir;

                    float4 light = CalculateLight(entryPos, dir, dstThroughAtmosphere);

                    float percent = 1 - saturate(light.a / _MaxDensity);
                    col *= percent;
                    col.x += light.x;
                    col.y += light.y;
                    col.z += light.z;
				}

                return col;
            }
            ENDCG
        }
    }
}
