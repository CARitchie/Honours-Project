// Adapted from https://www.ronja-tutorials.com/post/012-fresnel/

Shader "Custom/Fresnel"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EmissionTex("Emission Tex", 2D) = "black"{}
		[HDR] _EmissionColour("Emission Colour", Color) = (1, 1, 1, 1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[HDR] _FresnelColour("Fresnel Colour", Color) = (1, 1, 1, 1)
		[PowerSlider(4)] _FresnelExponent("Fresnel Exponent", Range(0.25, 4)) = 1
		_FresnelStrength("Fresnel Strength", float) = 1
		_Frequency("Frequency", float) = 1
		_Speed("Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _EmissionTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
			float4 screenPos;
			INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _EmissionColour;
		fixed4 _FresnelColour;

		float _FresnelStrength;
		float _FresnelExponent;
		float _Frequency;
		float offset;
		float _Speed;

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
            o.Alpha = c.a;

			fixed4 emission = tex2D(_EmissionTex, IN.uv_MainTex) * _EmissionColour;

			//get the dot product between the normal and the direction
			float fresnel = dot(IN.worldNormal, IN.viewDir);

			fresnel = 1 - saturate(fresnel);

			fresnel = pow(fresnel, _FresnelExponent) * _FresnelStrength;

			float2 coords = IN.screenPos.xy / IN.screenPos.w;
			
			float extraLines = saturate(sin(coords.y * _Frequency + _Time.y * _Speed));

			if (extraLines > 0.8) {
				fresnel += pow(((extraLines - 0.8) / 0.2) * 0.4,3);
			}

			//apply the fresnel value to the emission
			o.Emission = emission + fresnel * _FresnelColour;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
