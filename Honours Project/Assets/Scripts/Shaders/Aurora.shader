
// Inspired by https://www.youtube.com/watch?v=SNP5QqAQqn4

Shader "Unlit/Aurora"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
		_NoiseScale("Noise Scale", float) = 1
		_Speed("Speed", float) = 1
		_MaskTex("Mask Texture", 2D) = "white" {}
		[HDR] _Colour("Color", Color) = (1, 1, 1, 1)
		_DissolvePower("Dissolve Power", float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
		Cull Off

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float2 noiseUV : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 noiseUV : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            
            float4 _MainTex_ST;

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _MaskTex;

			float _NoiseScale;
			float _Speed;

			float4 _Colour;
			float _DissolvePower;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.noiseUV = TRANSFORM_TEX(v.noiseUV, _NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_NoiseTex, i.noiseUV * _NoiseScale + float2(0, _Time.y * _Speed));		// Change the vertical offset of the texture to make it look like it's moving
				
			

                // apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				col *= tex2D(_MaskTex, i.uv);
				fixed4 dissolve = tex2D(_NoiseTex, i.noiseUV * _NoiseScale + float2(0.005, _Time.y * (_Speed + 0.005)));	// Add more detail and motion
				dissolve = pow(dissolve, _DissolvePower);																	// Reduce the strength
				col *= dissolve;
				float a = col.r;
				col *= _Colour;
				
				col.a = a;
                return col;
            }
            ENDCG
        }
    }
}
