
// Adapted from https://youtu.be/LMSDFhGP73g

Shader "Unlit/SnowFall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            };

			float rand(float3 co) {
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

            sampler2D _MainTex;
            float4 _MainTex_ST;
			half _FlakeAmount, _FlakeOpacity;
			float3 _Origin;
			float _RadiusSquare;

			// Taken from https://stackoverflow.com/questions/41661369/smoothly-mapping-a-2d-uv-point-onto-a-3d-xyz-sphere
			// Maps a coordinate on a square to a point on a sphere's surface
			float3 SquareToSphere(float2 pos) {
				float lat = pos.y * 3.14159 - 3.14159 / 2;
				float lon = pos.x * 2 * 3.14159 - 3.14159;

				return normalize(float3(cos(lat) * cos(lon), sin(lat), cos(lat) * sin(lon)));
			}

			// Returns the square distance between to points
			float SquareDistance(float3 pos1, float3 pos2) {
				float3 dir = pos2 - pos1;
				return dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

				float3 pos = SquareToSphere(i.uv);									// Convert the pixel coordinate on the displacement map to a point on a sphere

				if(SquareDistance(pos, _Origin) > _RadiusSquare) return col;		// If the current point is not within the snow fall area, return the original colour

				float rValue = ceil(rand(pos * _Time.x) - (1 - _FlakeAmount));		// Randomly decide whether this pixel should be filled with snow

                return saturate(col - (rValue * _FlakeOpacity));
            }
            ENDCG
        }
    }
}
