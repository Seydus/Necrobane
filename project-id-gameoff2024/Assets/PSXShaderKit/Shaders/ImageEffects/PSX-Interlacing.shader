Shader "Hidden/PSX-Interlacing"
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
			sampler2D _PreviousFrame;
			fixed _InterlacedFrameIndex;
			fixed _InterlacingSize;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
			};

			v2f vert(float4 vertex : POSITION, float2 uv : TEXCOORD0, out float4 outpos : SV_POSITION)
			{
				v2f o;
				o.uv = uv;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 previousColor = tex2D(_PreviousFrame, i.uv);

				const int2 pixelPosition = screenPos.xy;
				fixed interlacingAreaCheck = floor(pixelPosition.y / _InterlacingSize) % 2 == round(_InterlacedFrameIndex);
				return lerp(col, previousColor, interlacingAreaCheck);
			}
			ENDCG
		}
	}
}
