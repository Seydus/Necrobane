Shader "PSX/Lite/Unlit"
{
    Properties
    {
        _Color("Color (RGBA)", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
        _ObjectDithering("Per-Object Dithering Enable", Range(0,1)) = 1
    }
        SubShader
    {
        Tags {"RenderType" = "Opaque" }
        ZWrite On
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #define PSX_TRIANGLE_SORT_OFF
            #include "UnityCG.cginc"
            #include "PSX-Utils.cginc"

            #include "PSX-ShaderSrc-Lite.cginc"

        ENDCG
        }
    }
        Fallback "Unlit/Color"
}