// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "My Shaders/Atmosphere Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Colour("Colour", Color) = (1, 1, 1, 1)
		_SunsetColour("Sunset Colour", Color) = (1, 1, 1, 1)
		_SunColour("Sun Colour", Color) = (1, 1, 1, 1)
		_Closeness("Closeness", float) = 25
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
			float3 _PlanetOrigin;
			float3 _PlanetRadius;
			float _AtmosphereRadius;
			sampler2D _CameraDepthTexture;
			int _NumberOfSteps;
			float3 _LightOrigin;
			float _Strength;
			float _SunsetStrength;
			float4 _Colour;
			float4 _SunsetColour;
			float4 _SunColour;
			float _Closeness;


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

			float SquareMag(float3 vec) {
				return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;
			}

			float3 Density(float3 position) {
				float2 sunResult = SphereCollision(position, _LightOrigin - position, _PlanetOrigin, _AtmosphereRadius);

				float distanceIn = sunResult.y - sunResult.x;

				float sunStrength = (distanceIn) / _AtmosphereRadius;
				float closeness = 1 - (length(position - _PlanetOrigin) - _PlanetRadius) / (_AtmosphereRadius - _PlanetRadius);

				float opacity = closeness - sunStrength * 500;

				float sunset;

				if (opacity <= 0) {
					opacity = 0;
					sunset = 0;
				}
				else {
					sunset = (distanceIn / (_AtmosphereRadius - _PlanetRadius)) * closeness * opacity * 100;
				}

				float3 density = float3(opacity, opacity, opacity) + (_SunsetColour.rgb * (sunset * _SunsetStrength));

				return density;

			}

			float SquareDistToCentre(float3 centre, float3 pos) {
				float3 dir = centre - pos;
				return dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
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

				float step = distanceIn / _NumberOfSteps;
				float limit = min(distanceIn, depth - result.x);

				float progress = 0;
				float3 density = 0;

				while (progress < limit) {
					float3 position = origin + normalize(i.viewDir) * (result.x + progress);

					float distance = SquareMag(position - origin) / _Closeness;
					distance = clamp(distance, 0, 1);

					density += Density(position) * step * distance;
					float _InnerRadius = 55;

					float middle = ((_AtmosphereRadius - _InnerRadius) / 2) + _InnerRadius;
					middle = middle * middle;
					float multiplier = 1;

					float distanceToCentre = SquareDistToCentre(_PlanetOrigin, position);

					if (distanceToCentre < middle) {
						float squared = _InnerRadius * _InnerRadius;
						middle -= squared;
						distanceToCentre -= squared;
						multiplier = distanceToCentre / middle;
					}
					else {
						float top = _AtmosphereRadius * _AtmosphereRadius;
						top -= middle;
						distanceToCentre -= middle;
						multiplier = 1 - (distanceToCentre / top);
					}

					multiplier = max(0, multiplier);		
					
					progress += step;
				}

				density /= _NumberOfSteps;

				float value = 0;

				/*
				float2 sunResult = SphereCollision(_WorldSpaceCameraPos, i.viewDir, _LightOrigin, 200);
				if (sunResult.x > 0 && sunResult.x < depth) {
					col.rgb += _SunColour * density;
				}*/

				float3 newColour = (_Colour.rgb * density * _Strength);
				col.rgb += newColour;

				return col;
            }
            ENDCG
        }
    }
}
