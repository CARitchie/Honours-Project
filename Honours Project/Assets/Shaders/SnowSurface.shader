
// Adapted from https://youtu.be/_NfxMMzYwgo

Shader "My Shaders/Snow Surface"
{
    Properties
    {
		_Tess("Tessellation", Range(1,32)) = 4
        _SnowColor ("Snow Color", Color) = (1,1,1,1)
        _SnowTex ("Snow Texture", 2D) = "white" {}
		_GroundColor("Ground Color", Color) = (1,1,1,1)
		_GroundTex("Ground Texture", 2D) = "white" {}
		_DispTex("Disp Texture", 2D) = "black" {}
		_Displacement("Displacement", Range(0, 5.0)) = 0.3
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Origin("Origin",vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessDistance

        #pragma target 4.6
		#include "Tessellation.cginc"

		struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
		};

		float _Tess;

		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = 10.0;
			float maxDist = 25.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
		}

		sampler2D _DispTex;
		float _Displacement;
		float3 _Origin;

		// Adapted form https://en.wikipedia.org/wiki/UV_mapping
		float2 SphereToSquare(float3 pos) {
			pos = normalize(pos);
			float u = 0.5 + (atan2(pos.x, pos.z) / (2 * 3.14159));
			float v = 0.5 + (asin(pos.y) / 3.14159);
			return float2((1 - u) + 0.25, v);
		}

		void disp(inout appdata v)
		{
			float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
			float d = tex2Dlod(_DispTex, float4(SphereToSquare(worldPos - _Origin),0,0)).r * _Displacement;

			float3 dir = normalize(worldPos - _Origin);
			v.vertex.xyz -= dir * d;
			v.vertex.xyz += dir * _Displacement;
		}

        sampler2D _SnowTex;
		fixed4 _SnowColor;
		sampler2D _GroundTex;
		fixed4 _GroundColor;

		

        struct Input
        {
            float2 uv_SnowTex;
            float2 uv_GroundTex;
            float2 uv_DispTex;
			float3 worldPos;
        };

        half _Glossiness;    

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			half amount = tex2D(_DispTex, SphereToSquare(normalize(IN.worldPos - _Origin))).r;
			fixed4 c = lerp(tex2D(_SnowTex, IN.uv_SnowTex) * _SnowColor, tex2D(_GroundTex, IN.uv_GroundTex) * _GroundColor, amount);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
