Shader "Hidden/PSX-PostProcess"
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

			float3 _ColorResolution;
			float3 _DitherResolution;
			sampler2D _MainTex;
			float _HighResDitherMatrix;
			float _DitheringScale;

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

			float DitherColorChannel(float color, float ditherOffset, float ditherStep)
			{
				float distance = fmod(color, ditherStep);
				float baseValue = floor(color / ditherStep) * ditherStep;

				float nudgedValue = (distance + ditherOffset * ditherStep) / ditherStep;

				return baseValue + ditherStep * floor(nudgedValue);
			}

			float DitherCol(float color, float ditherOffset, float ditherStep)
			{
				float distance = fmod(color, ditherStep);
				float baseValue = floor(color / ditherStep) * ditherStep;

				return (ditherOffset < distance / ditherStep - 0.001f) ? baseValue + ditherStep : baseValue;
			}

			float GetDitherThreshold(unsigned int2 pixelPosition) 
			{
				const int ditheringMatrix4x4[16] =
				{
					0,  8,  2,  10,
					12, 4,  14, 6,
					3,  11, 1,  9,
					15, 7,  13, 5
				};

				const int ditheringMatrix4x4PS1[16] =
				{
					-4, +0, -3, +1,
					+2, -2, +3, -1,
					-3, +1, -4, +0,
					+3, -1, +2, -2
				};

				const int ditheringMatrix2x2[4] =
				{
					0, 3,
					2, 1
				};

				return _HighResDitherMatrix > 0.25f ? _HighResDitherMatrix > 0.75f
					? ditheringMatrix4x4PS1[fmod(pixelPosition.x, 4) + fmod(pixelPosition.y, 4) * 4] * 0.125f
					: ditheringMatrix4x4[fmod(pixelPosition.x, 4) + fmod(pixelPosition.y, 4) * 4] * 0.0625f
					: ditheringMatrix2x2[fmod(pixelPosition.x, 2) + fmod(pixelPosition.y, 2) * 2] * 0.25f;
			}

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				const float2 pixelPosition = screenPos.xy;
				const float ditherThreshold = GetDitherThreshold(floor(pixelPosition * _DitheringScale));
				const float3 ditherStep = 1.0f / max(_DitherResolution, 1.0f);
				const float3 colorStep = 1.0f / max(_ColorResolution, 1.0f);

				fixed4 col = tex2D(_MainTex, i.uv);
				col.r = DitherCol(col.r, 0.5f, colorStep.r);
				col.g = DitherCol(col.g, 0.5f, colorStep.g);
				col.b = DitherCol(col.b, 0.5f, colorStep.b);

				col.r = DitherColorChannel(col.r, ditherThreshold, ditherStep.r);
				col.g = DitherColorChannel(col.g, ditherThreshold, ditherStep.g);
				col.b = DitherColorChannel(col.b, ditherThreshold, ditherStep.b);

				return col;
			}
			ENDCG
		}
	}
}
