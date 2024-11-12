Shader "PSX/Unlit"
{
    Properties
    {
        _Color("Color (RGBA)", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
        _ObjectDithering("Per-Object Dithering Enable", Range(0,1)) = 1
        _CustomDepthOffset("Custom Depth Offset", Float) = 0
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
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile PSX_TRIANGLE_SORT_OFF PSX_TRIANGLE_SORT_CENTER_Z PSX_TRIANGLE_SORT_CLOSEST_Z PSX_TRIANGLE_SORT_CENTER_VIEWDIST PSX_TRIANGLE_SORT_CLOSEST_VIEWDIST PSX_TRIANGLE_SORT_CUSTOM

            #include "UnityCG.cginc"
            #include "PSX-Utils.cginc"

            #include "PSX-ShaderSrc.cginc"

        ENDCG
        }
    }
        Fallback "PSX/Lite/Unlit"
}