Shader "My Shaders/Teleport Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		[HDR] _Colour2 ("Colour 2", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Noise("Noise Texture", 2D) = "white" {}
		_Emission("Emission Texture", 2D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Threshold("Threshold", Range(0.0, 1.0)) = 0
		_Scale("Scale", float) = 3
		_Offset("Offset", float) = 0.1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
		Pass {
			ColorMask 0
		}

		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _Colour2;
		sampler2D _Noise;
		sampler2D _Emission;
		float _Threshold;
		float _Scale;
		float _Offset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

			fixed4 n = tex2D(_Noise, IN.uv_MainTex * _Scale);			// Read value from noise texture
			if (n.r > _Threshold || _Threshold == 0) o.Alpha = 0;		// Pixel should be transparent if noise is greater than threshold
			else o.Alpha = c.a;											// Otherwise transparency should not change

			if (n.r + _Offset > _Threshold) {							// Add a glowing outline around transparent pixels
				o.Albedo = float3(1,1,1);
				o.Emission = _Colour2;
			}
			else {
				o.Emission = tex2D(_Emission, IN.uv_MainTex);
			}
			

        }
        ENDCG
    }
    FallBack "Diffuse"
}
