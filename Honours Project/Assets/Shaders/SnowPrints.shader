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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

			float WrappedDistance(float2 uv) {
				float distances[9];
				distances[0] = distance(uv, _Coordinate.xy);
				distances[1] = distance(uv, float2(_Coordinate.x - 1, _Coordinate.y + 1));
				distances[2] = distance(uv, float2(_Coordinate.x, _Coordinate.y + 1));
				distances[3] = distance(uv, float2(_Coordinate.x + 1, _Coordinate.y + 1));
				distances[4] = distance(uv, float2(_Coordinate.x - 1, _Coordinate.y));
				distances[5] = distance(uv, float2(_Coordinate.x + 1, _Coordinate.y));
				distances[6] = distance(uv, float2(_Coordinate.x - 1, _Coordinate.y - 1));
				distances[7] = distance(uv, float2(_Coordinate.x, _Coordinate.y - 1));
				distances[8] = distance(uv, float2(_Coordinate.x + 1, _Coordinate.y - 1));

				float min = distances[0];
				for (int i = 1; i < 9; i++) {
					if (distances[i] < min) min = distances[i];
				}
				return min;

			}

			float3 squareToSphere(float2 pos) {
				float lat = pos.y * 3.14159 - 3.14159 / 2;
				float lon = pos.x * 2 * 3.14159 - 3.14159;

				return normalize(float3(cos(lat) * cos(lon), sin(lat), cos(lat) * sin(lon)));
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float dist = distance(squareToSphere(i.uv), _Coordinate);
				float draw = pow(saturate(1 - dist), 100);
				fixed4 drawCol = _Color * (draw * 1);
				return saturate(col + drawCol);
            }
            ENDCG
        }
    }
}
