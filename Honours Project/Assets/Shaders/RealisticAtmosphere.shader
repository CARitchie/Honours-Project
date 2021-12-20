Shader "My Shaders/Realistic Atmosphere Shader"
{
Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Closeness("Closeness", float) = 25
		_DensityFalloff("Density Falloff", float) = 1
		_OutScatterPoints("Out Scattering Points", int) = 1
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
			float _Closeness;

            float3 scatteringCoefficients;
			int _OutScatterPoints;
			float _DensityFalloff;

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

					if(val2 >= 0) return float2(val1, val2 - val1);
				}

				return float2(-1,-1);
			}

			float SquareMag(float3 vec) {
				return vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;
			}

			float SquareDistToCentre(float3 centre, float3 pos) {
				float3 dir = centre - pos;
				return dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
			}

			// g = 0 for Rayleigh scattering
			// g > -1 && g <= -0.75 ofr Mie scattering
			// Theta is angle between ray and sun
			float PhaseFunction(float cosTheta, float g){
				float square = pow(g, 2);

				float val1 = (3 * ( 1 - square )) / (2 * ( 2 + square ));
				float val2 = (1 + pow(cosTheta,2)) / pow((1 + square) - (2 * g * cosTheta), 3/2);

				return val1 * val2;
			}

			float Density(float3 pos){
				float height = length(pos - _PlanetOrigin) - _PlanetRadius;
				float heightPercent = height / (_AtmosphereRadius - _PlanetRadius);
				float density = exp(-heightPercent * _DensityFalloff) * (1 - heightPercent);
				return clamp(density, 0, 1);
			}

			float OutScattering(float3 origin, float3 dir, float dist){
				float3 pos = origin;
				float step = dist / (_OutScatterPoints - 1);
				float opticalDepth = 0;

				for(int i = 0; i < _OutScatterPoints ; i++){
					float density = Density(pos);
					opticalDepth += density * step;
					pos += dir * step;
				}

				return opticalDepth;
			}

			float3 CalculateLight(float3 origin, float3 dir, float dist, float3 cameraPos){
				float3 pos = origin;
				float step = dist / (_NumberOfSteps - 1);
				float3 inScatteredLight = float3(0,0,0);

				for(int i =0 ; i < _NumberOfSteps ; i++){

					float3 sunDir = normalize(_LightOrigin - pos);
					float sunRayDist = SphereCollision(pos, sunDir, _PlanetOrigin, _AtmosphereRadius).y;
					float sunRayOpticalDepth = OutScattering(pos, sunDir, sunRayDist);
					
					float viewRayOpticalDepth = OutScattering(pos, -dir, step * i);
					
					float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * scatteringCoefficients);

					float closeness = SquareMag(pos - cameraPos) / _Closeness;
					closeness = clamp(closeness, 0, 1);

					inScatteredLight += Density(pos) * transmittance * scatteringCoefficients * step * closeness;
					
					pos += dir * step;
				}

				return inScatteredLight * _Strength;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);

				float viewLength = length(i.viewDir);
				float3 dir = i.viewDir / viewLength;
				float3 origin = _WorldSpaceCameraPos;

				float depth = DECODE_EYEDEPTH(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * viewLength;

				// Find out if the current pixel collides with the atmosphere
				float2 result = SphereCollision(origin, dir, _PlanetOrigin, _AtmosphereRadius);

				float dstToAtmosphere = result.x;
				float dstThroughAtmosphere = min(result.y, depth - dstToAtmosphere);

				if(dstThroughAtmosphere > 0){
					float3 entryPoint = origin + dir * dstToAtmosphere;
					float3 light = CalculateLight(entryPoint, dir, dstThroughAtmosphere, origin);
					col.xyz += light;
				}

				return col;

			}
            ENDCG
        }
    }
}
