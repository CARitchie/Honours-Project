Shader "My Shaders/Pixel Fade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Noise("Noise Texture", 2D) = "white" {}
		_Threshold("Threshold", Range(0.0, 1.0)) = 0
		_Scale("Scale", float) = 3
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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

			Texture2D _Noise;
			SamplerState sampler_Noise;

			float _Threshold;
			float _Scale;

			float2 squareUV(float2 uv) {
				float width = _ScreenParams.x;
				float height = _ScreenParams.y;
				//float minDim = min(width, height);
				float scale = 1000;
				float x = uv.x * width;
				float y = uv.y * height;
				return float2 (x / scale, y / scale);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				float value = _Noise.SampleLevel(sampler_Noise, squareUV(i.uv * _Scale), 0);
				if (value > _Threshold || _Threshold == 0) col.rgb = 0;
                return col;
            }
            ENDCG
        }
    }
}
