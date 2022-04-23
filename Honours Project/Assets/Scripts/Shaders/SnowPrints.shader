
// Adapted from https://youtu.be/-yaqhzX-7qo

Shader "Unlit/SnowPrints"
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Coordinate, _Color;
			float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			// Taken from https://stackoverflow.com/questions/41661369/smoothly-mapping-a-2d-uv-point-onto-a-3d-xyz-sphere
			// Maps a coordinate on a square to a point on a sphere's surface
			float3 SquareToSphere(float2 pos) {
				float lat = pos.y * 3.14159 - 3.14159 / 2;
				float lon = pos.x * 2 * 3.14159 - 3.14159;

				return normalize(float3(cos(lat) * cos(lon), sin(lat), cos(lat) * sin(lon)));
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float dist = distance(SquareToSphere(i.uv), _Coordinate);			// Convert pixel coordinate to point on sphere and work out distance to the snow print coordinate
				float draw = pow(saturate(1 - dist), _Size);						// Draw strength is determined by distance to snow print coordinate
				fixed4 drawCol = _Color * draw;
				return saturate(col + drawCol);
            }
            ENDCG
        }
    }
}
