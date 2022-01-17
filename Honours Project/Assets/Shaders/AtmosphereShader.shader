// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Created by following equations used in https://developer.nvidia.com/gpugems/gpugems2/part-ii-shading-lighting-and-shadows/chapter-16-accurate-atmospheric-scattering

Shader "My Shaders/Atmosphere Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Closeness("Closeness", float) = 90

		_Strength("Atmosphere Strength", float) = 1
		_ScaleHeight("Average Density Height", float) = 1
		_ScatterConstant("Scattering Constants", Vector) = (1,1,1,1)
		_SunIntensity("Sun Intensity", Color) = (1,1,1,1)
		_InScatterSteps("InScatter Steps", Range(2,200)) = 10
		_OutScatterSteps("OutScatter Steps", Range(2,200)) = 10
		_StarFade("Star Fade", float) = 0
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

			float3 _PlanetOrigin;
			float3 _PlanetRadius;
			float _AtmosphereRadius;
			float3 _LightOrigin;
			
			float _Strength;
			float _Closeness;
			float _ScaleHeight;
			float3 _ScatterConstant = float3(1,1,1);
			float4 _SunIntensity;

			int _InScatterSteps;
			int _OutScatterSteps;

			float _StarFade;


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

					if(val2 >= 0) return float2(val1,val2);
				}

				return float2(-1,-1);
			}

			// g = 0 for Rayleigh scattering
			// g > -1 && g <= -0.75 ofr Mie scattering
			// Theta is angle between ray and sun
			float PhaseFunction(float theta, float g) {
				float square = g * g;
				float val1 = (3 * (1 - square)) / (2 * (2 + square));
				float val2 = (1 + pow(cos(theta), 2)) / pow(1 + square - 2 * g *cos(theta), 1.5);
				return val1 * val2;
			}

			float HeightPercent(float3 pos) {
				float height = distance(pos, _PlanetOrigin) - _PlanetRadius;
				return saturate(height / (_AtmosphereRadius - _PlanetRadius));
			}

			float Density(float3 pos) {
				float height = HeightPercent(pos);
				return exp(-height / (_ScaleHeight * 0.01));
			}

			float DensityAlongRay(float3 origin, float3 direction, float distance) {
				float step = distance / _OutScatterSteps;
				float3 pos = origin;
				
				float density = 0;

				for(int i = 0 ; i < _OutScatterSteps; i++){
					density += Density(pos) * step;
					pos += direction * step;
				}

				return density;
			}

			float3 OutScatter(float3 origin, float3 direction, float distance) {
				return DensityAlongRay(origin, direction, distance) * 4 * 3.14159 * _ScatterConstant;
			}

			float GetSunAngle(float3 pos, float3 direction) {
				float3 dirToSun = normalize(_LightOrigin - pos);
				float3 dirToOrigin = normalize(-direction);
				float dotProduct = dot(dirToSun, dirToOrigin);

				return acos(dotProduct);
			}

			float3 InScatter(float3 origin, float3 direction, float distance) {
				float sunAngle = GetSunAngle(origin, direction);
				float phase = PhaseFunction(sunAngle, 0);
				phase = 1;

				float progress = 0;

				float step = distance / _InScatterSteps;
				float3 pos = origin;

				float3 lightIn = float3(0,0,0);

				while (progress < distance) {

					float density = Density(pos);

					float3 sunDir = normalize(_LightOrigin - pos);
					float sunDist = SphereCollision(pos, sunDir, _PlanetOrigin, _AtmosphereRadius).y;
					float3 sunOutScatter = OutScatter(pos, sunDir, sunDist);

					float3 outScatter = OutScatter(pos, -direction, progress);

					lightIn += density * exp(-sunOutScatter - outScatter) * step;

					progress += step;
					pos += direction * step;
				}

				return _SunIntensity * phase * lightIn * _Strength * _ScatterConstant;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float3 origin = _WorldSpaceCameraPos;
				float viewLength = length(i.viewDir);
				float3 direction = i.viewDir / viewLength;

				// Find out if the current pixel collides with the atmosphere
				float2 result = SphereCollision(origin, direction, _PlanetOrigin, _AtmosphereRadius);

				float depth = DECODE_EYEDEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * viewLength;

				// If there is no atmosphere here, or it is being obstructed, return the original colour
				if (result.x < 0 || result.x >= depth) {
					return col;
				}

				float distanceIn = result.y - result.x;
				float limit = min(distanceIn, depth - result.x);
				
				float3 position = origin + direction * result.x;
				float3 light = InScatter(position, normalize(direction), limit);

				if (depth < _Closeness) {
					light *= depth / _Closeness;
				}
				else if (depth > 10000) {
					col.rgb *= 1 - saturate(light.g / _StarFade);
				}

				col.rgb += light;

				return col;
            }
            ENDCG
        }
    }
}
