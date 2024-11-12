Shader "PSX/Lite/Vertex Lit Cutout"
{
	Properties
	{
		_Color("Color (RGBA)", Color) = (1, 1, 1, 1)
        _EmissionColor("Emission Color (RGBA)", Color) = (0,0,0,0)
        _CubemapColor("Cubemap Color (RGBA)", Color) = (0,0,0,0)
        _MainTex("Texture", 2D) = "white" {}
        _EmissiveTex("Emissive", 2D) = "black" {}
        _Cubemap("Cubemap", Cube) = "" {}
		_ReflectionMap("Reflection Map", 2D) = "white" {}
		_ObjectDithering("Per-Object Dithering Enable", Range(0,1)) = 1
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.1
	}
	SubShader
	{
		Tags {"RenderType" = "Opaque" }
		ZWrite On
		LOD 100

		Pass
		{
			Tags { "LightMode" = "VertexLM" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile __ PSX_ENABLE_CUSTOM_VERTEX_LIGHTING

			#define PSX_TRIANGLE_SORT_OFF
			#include "UnityCG.cginc"
			#include "PSX-Utils.cginc"

			samplerCUBE _Cubemap;
			sampler2D _ReflectionMap;
			float4 _CubemapColor;
			float _Cutoff;
			
            #define PSX_VERTEX_LIT
			#define PSX_CUBEMAP _Cubemap
			#define PSX_CUBEMAP_COLOR _CubemapColor
			#define PSX_CUTOUT_VAL _Cutoff
			
			#include "PSX-ShaderSrc-lite.cginc"

		ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "Vertex" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
            #pragma multi_compile __ PSX_ENABLE_CUSTOM_VERTEX_LIGHTING

			#include "UnityCG.cginc"
			#include "PSX-Utils.cginc"

			samplerCUBE _Cubemap;
			sampler2D _ReflectionMap;
			float4 _CubemapColor;
			float _Cutoff;
			
            #define PSX_VERTEX_LIT
			#define PSX_CUBEMAP _Cubemap
			#define PSX_CUBEMAP_COLOR _CubemapColor
			#define PSX_CUTOUT_VAL _Cutoff
			
			#include "PSX-ShaderSrc-lite.cginc"

			ENDCG
		}
	}
		Fallback "PSX/Lite/Unlit Transparent"
}