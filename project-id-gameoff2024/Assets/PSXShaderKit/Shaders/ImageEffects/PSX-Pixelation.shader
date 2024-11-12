Shader "Hidden/PSX-Pixelation"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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

			sampler2D _MainTex;
			float _PixelationFactor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
			};

			v2f vert(
				float4 vertex : POSITION, // vertex position input
				float2 uv : TEXCOORD0, // texture coordinate input
				out float4 outpos : SV_POSITION // clip space position output
			)
			{
				v2f o;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				float2 screenResolution = _ScreenParams.xy;
				float2 pixelSize = _ScreenParams.zw - 1;
				float2 pixelScalingFactor = screenResolution * _PixelationFactor;

				float2 pixelOrigin = floor((i.uv) * pixelScalingFactor) / pixelScalingFactor;

				fixed4 col = tex2D(_MainTex, pixelOrigin, float2(0,0), float2(0,0));

				return col;
			}
			ENDCG
		}
	}
}
