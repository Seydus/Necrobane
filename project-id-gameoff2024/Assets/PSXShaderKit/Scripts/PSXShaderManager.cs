using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSXShaderKit
{
    [ExecuteAlways]
    public class PSXShaderManager : MonoBehaviour
    {
        enum PSXTextureWarpingMode
        {
            Stable = 0,
            Accurate = 1
        }
        enum PSXVertexWobbleMode
        {
            ViewSpace = 0,
            ClipSpace = 1
        }

        enum PSXFlatShadingMode
        {
            AverageLight = 0,
            CenterLight = 1
        }

        enum PSXTriangleSortMode
        {
            Off = 0,
            CenterDepth = 1,
            ClosestVertexDepth = 2,
            ViewCenterDistance = 3,
            ViewClosestVertexDistance = 4,
            Custom = 10
        }

        [Header("Texture Warping")]
        [Tooltip("Controls the strength of texture warping caused by perspective-incorrect UV sampling.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _TextureWarpingFactor = 1.0f;
        [Tooltip("Stable replicates the effect inaccurately, but the distortion is less extreme at close distances.")]
        [SerializeField]
        private PSXTextureWarpingMode _TextureWarpingMode = PSXTextureWarpingMode.Stable;

        [Header("Vertex Wobble")]
        [Tooltip("The size of the grid that vertices will snap to. Smaller means more wobbling.")]
        [SerializeField]
        private float _VertexGridResolution = 100.0f;
        [Tooltip("ViewSpace makes it so that the effect is decreased with distance to the object. ClipSpace is more accurate.")]
        [SerializeField]
        private PSXVertexWobbleMode _VertexWobbleMode = PSXVertexWobbleMode.ViewSpace;

        [Header("Vertex Lighting")]
        [Tooltip("Use custom vertex lighting with linear attenuation")]
        [SerializeField]
        private bool _UseRetroVertexLighting = true;
        [Range(0.0f, 1.0f)]
        [Tooltip("How much the lighting is affected by the angle of surfaces relative to the light. 0 means the lighting intensity is entirely distance based. (ie, surfaces looking away from the light are still lit)")]
        [SerializeField]
        private float _RetroLightingNormalFactor = 0.0f;
        [Range(0.0f, 0.9999f)]
        [Tooltip("How far into the light's radius the falloff should start. Higher values make the light look brighter and the falloff more harsh.")]
        [SerializeField]
        private float _RetroLightFalloffStart = 0.0f;
        [Tooltip("Whether the flat shading light averages the light of all vertices or calculates a new lighting value from the center of the triangle. You can enable flat shading per-material.")]
        [SerializeField]
        private PSXFlatShadingMode _FlatShadingMode = PSXFlatShadingMode.AverageLight;

        [Header("Triangle Sorting")]
        [SerializeField]
        [Tooltip("The PS1 had no depth buffer and manually sorted every triangle front to back. This behavior is simulated by giving one custom depth value to each entire triangle, but might introduce too many visual bugs if your geometry isn't made for it. You can affect the sorting by changing Custom Depth Offset on your materials.\n-Depth options are best for mixing psx and non-psx shaders. \n-View Distance options are more stable.\n-You can edit the custom sort mode in PSX-Utils.cginc.")]
        private PSXTriangleSortMode _TriangleSortMode = PSXTriangleSortMode.Off;
        [SerializeField]
        [Tooltip("View the custom depth output by each triangle to emulate triangle sorting.")]
        private bool _DepthDebugView = false;

        void Start()
        {
            UpdateValues();
        }

        void OnValidate()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            Shader.SetGlobalFloat("_PSX_GridSize", _VertexGridResolution);
            Shader.SetGlobalFloat("_PSX_DepthDebug", _DepthDebugView ? 1.0f : 0.0f);
            Shader.SetGlobalFloat("_PSX_LightingNormalFactor", _RetroLightingNormalFactor);
            Shader.SetGlobalFloat("_PSX_TextureWarpingFactor", _TextureWarpingFactor);
            Shader.SetGlobalFloat("_PSX_LightFalloffPercent", _RetroLightFalloffStart);
            Shader.SetGlobalFloat("_PSX_FlatShadingMode", _FlatShadingMode == PSXFlatShadingMode.AverageLight ? 0 : 1);
            Shader.SetGlobalFloat("_PSX_TextureWarpingMode", _TextureWarpingMode == PSXTextureWarpingMode.Stable ? 0 : 1);
            Shader.SetGlobalFloat("_PSX_VertexWobbleMode", _VertexWobbleMode == PSXVertexWobbleMode.ViewSpace ? 0 : 1);

            if (_UseRetroVertexLighting)
            {
                Shader.EnableKeyword("PSX_ENABLE_CUSTOM_VERTEX_LIGHTING");
            }
            else
            {
                Shader.DisableKeyword("PSX_ENABLE_CUSTOM_VERTEX_LIGHTING");
            }
            
            if (_FlatShadingMode == PSXFlatShadingMode.CenterLight)
            {
                Shader.EnableKeyword("PSX_FLAT_SHADING_MODE_CENTER");
            }
            else
            {
                Shader.DisableKeyword("PSX_FLAT_SHADING_MODE_CENTER");
            }
            
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_OFF");
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_CENTER_Z");
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_CLOSEST_Z");
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_CENTER_VIEWDIST");
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_CLOSEST_VIEWDIST");
            Shader.DisableKeyword("PSX_TRIANGLE_SORT_CUSTOM");

            switch (_TriangleSortMode)
            {
                case PSXTriangleSortMode.Off:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_OFF");
                    break;
                case PSXTriangleSortMode.CenterDepth:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_CENTER_Z");
                    break;
                case PSXTriangleSortMode.ClosestVertexDepth:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_CLOSEST_Z");
                    break;
                case PSXTriangleSortMode.ViewCenterDistance:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_CENTER_VIEWDIST");
                    break;
                case PSXTriangleSortMode.ViewClosestVertexDistance:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_CLOSEST_VIEWDIST");
                    break;
                case PSXTriangleSortMode.Custom:
                    Shader.EnableKeyword("PSX_TRIANGLE_SORT_CUSTOM");
                    break;
            }
        }
    }
}
