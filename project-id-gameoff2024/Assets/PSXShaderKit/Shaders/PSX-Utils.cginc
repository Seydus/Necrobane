/*
Shader Features
    -PSX_VERTEX_LIT
    -PSX_CUTOUT_VAL [float threshold]
	-PSX_CUBEMAP [texCUBE cubemap]
		-PSX_CUBEMAP_COLOR [float intensity]

Shader Customization
    -PSX_TRIANGLE_SORTING_FUNC [float4 v1, float4 v2, float4 v3]

*/

//Globals set by PSXShaderManager.cs
float _PSX_GridSize;
float _PSX_DepthDebug;
float _PSX_LightingNormalFactor;
float _PSX_TextureWarpingFactor;
float _PSX_TextureWarpingMode;

//Material params
float _PSX_ObjectDithering;
float _CustomDepthOffset;
fixed _PSX_LightFalloffPercent;

//Math Utils
float invLerp(float from, float to, float value)
{
	return (value - from) / (to - from);
}

float3 SnapVertexToGrid(float3 vertex)
{
    return _PSX_GridSize < 0.00001f ? vertex : (floor(vertex * _PSX_GridSize) / _PSX_GridSize);
}

float4 CalculateAffineUV(float4 vertex, float2 uv) 
{
	float4x4 matrix_p = UNITY_MATRIX_P;
	
	float affineFactor = _PSX_TextureWarpingMode < 0.5f ? (length(vertex.xyz)) : max(mul(matrix_p, vertex).w, 0.1);
	affineFactor = lerp(1 , affineFactor, _PSX_TextureWarpingFactor);
    return float4(uv * affineFactor, affineFactor, 0);
}

//Dithering
int PSX_GetDitherOffset(int2 pixelPosition)
{
	const uint ditheringMatrix4x4[16] =
	{
		-4, +0, -3, +1,
		+2, -2, +3, -1,
		-3, +1, -4, +0,
		+3, -1, +2, -2
	};

	return ditheringMatrix4x4[pixelPosition.x % 4 + (pixelPosition.y % 4) * 4];
}

fixed4 PSX_DitherColor(float4 color, int2 pixelPosition)
{
	int4 col255 = round(color * 255);
	col255 = (col255 + PSX_GetDitherOffset(pixelPosition.xy)) >> 3;

	return col255 / 31.0f;
}

//Lighting
float3 ShadeUnityVertexLightsFull(float4 viewpos, float3 viewN, int lightCount, bool spotLight)
{
	float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
	for (int i = 0; i < lightCount; i++)
	{
		float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		float lengthSq = dot(toLight, toLight);

		// don't produce NaNs if some vertex position overlaps with the light
		lengthSq = max(lengthSq, 0.000001);

		toLight *= rsqrt(lengthSq);

		float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
		if (spotLight)
		{
			float rho = max(0, dot(toLight, unity_SpotDirection[i].xyz));
			float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
			atten *= saturate(spotAtt);
		}

		float diff = max(0, dot(viewN, toLight));
		lightColor += unity_LightColor[i].rgb * (diff * atten);
	}
	return lightColor;
}

float3 ShadePSXVertexLightsFull(float4 viewpos, float3 viewN, int lightCount, bool spotLight)
{
	fixed3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

	for (int i = 0; i < lightCount; i++)
	{
		fixed3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
		fixed lightDist = length(toLight);
		toLight = normalize(toLight);

		// don't produce NaNs if some vertex position overlaps with the light
		lightDist = max(lightDist, 0.000001);

		float lightRange = sqrt(unity_LightAtten[i].w);
		fixed atten = invLerp(lightRange, lightRange * _PSX_LightFalloffPercent, lightDist);
		if (spotLight)
		{
			float rho = max(0, dot(toLight, unity_SpotDirection[i].xyz));
			float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
			atten *= saturate(spotAtt);
		}

		float diff = max(0, dot(viewN, toLight));
		lightColor += unity_LightColor[i].rgb * saturate(atten) * 0.25 * lerp(1, diff, _PSX_LightingNormalFactor);
	}
	return lightColor;
}


//Triangle sorting functions. Input is 3 object-space verts, output is the custom depth to be used by the entire triangle.
float GetTriangleSortingDepth_CenterDepth(float4 v1, float4 v2, float4 v3)
{
	float4x4 matrix_mv = UNITY_MATRIX_MV;
	float4x4 matrix_p = UNITY_MATRIX_P;

	float4 viewCenter =  mul(matrix_mv, (v1 + v2 + v3) / 3.0f);
	// Move the vertex along its direction from the camera to nudge its depth and affect the sorting priority.
	viewCenter.xyz += normalize(viewCenter.xyz) * _CustomDepthOffset;
	viewCenter.z = min(viewCenter.z, -0.0001);

	float4 clipCenter = mul(matrix_p, viewCenter);
	//Output clip space vertex z/w to simulate how depth is calculated for a regular depth buffer.
	return saturate(clipCenter.z / clipCenter.w);
}

float GetTriangleSortingDepth_ClosestVertexDepth(float4 v1, float4 v2, float4 v3)
{	
	float4x4 matrix_mv = UNITY_MATRIX_MV;
	float4x4 matrix_p = UNITY_MATRIX_P;

	v1 = mul(matrix_mv, v1);
	v2 = mul(matrix_mv, v2);
	v3 = mul(matrix_mv, v3);

	v1.xyz += normalize(v1.xyz) * _CustomDepthOffset;
	v1.z = min(v1.z, -0.0001);
	v2.xyz += normalize(v2.xyz) * _CustomDepthOffset;
	v2.z = min(v2.z, -0.0001);
	v3.xyz += normalize(v3.xyz) * _CustomDepthOffset;
	v3.z = min(v3.z, -0.0001);

	v1 = mul(matrix_p, v1);
	v2 = mul(matrix_p, v2);
	v3 = mul(matrix_p, v3);

	//Clip space w can be negative if the vertex is off-screen and it messes up the calculations.
	//Only consider triangles whose w is positive.
	float depth = 100;
	depth = lerp(depth, min(depth, v1.z/v1.w), step(0, v1.w));
	depth = lerp(depth, min(depth, v2.z/v2.w), step(0, v2.w));
	depth = lerp(depth, min(depth, v3.z/v3.w), step(0, v3.w));
	return saturate(depth);
}

float GetTriangleSortingDepth_LinearClosestVertexDistance(float4 v1, float4 v2, float4 v3)
{
	v1.xyz = UnityObjectToViewPos(v1.xyz);
	v2.xyz = UnityObjectToViewPos(v2.xyz);
	v3.xyz = UnityObjectToViewPos(v3.xyz);

	v1.xyz += normalize(v1.xyz) * _CustomDepthOffset;
	v1.z = min(v1.z, -0.0001);
	v2.xyz += normalize(v2.xyz) * _CustomDepthOffset;
	v2.z = min(v2.z, -0.0001);
	v3.xyz += normalize(v3.xyz) * _CustomDepthOffset;
	v3.z = min(v3.z, -0.0001);

	float depth = 0;
	depth = max(length(v1.xyz), max(length(v2.xyz), length(v3.xyz)));
	return  saturate(1 - depth * _ProjectionParams.w);
}

//This function doesn't try to mimic the value distribution of a regular depth buffer, but still works
//if only PSX shaders are used in your scene.
float GetTriangleSortingDepth_LinearCenterDistance(float4 v1, float4 v2, float4 v3)
{
	float3 center = UnityObjectToViewPos((v1 + v2 + v3).xyz  / 3.0f);
	center.xyz += normalize(center.xyz) * _CustomDepthOffset;
	center.z = min(center.z, -0.0001);

	return saturate(1 - length(center) * _ProjectionParams.w);
}

//Custom template.
float GetTriangleSortingDepth_Custom(float4 v1, float4 v2, float4 v3)
{
    return UnityObjectToClipPos((v1 + v2 + v3) / 3.0f).z;
}

//PSX_TRIANGLE_SORT_CENTER_Z PSX_TRIANGLE_SORT_CLOSEST_Z PSX_TRIANGLE_SORT_CENTER_VIEWDIST PSX_TRIANGLE_SORT_CLOSEST_VIEWDIST PSX_TRIANGLE_SORT_CUSTOM
#ifdef PSX_TRIANGLE_SORT_CENTER_Z
	#define PSX_TRIANGLE_SORTING_FUNC(v1, v2, v3) GetTriangleSortingDepth_CenterDepth(v1, v2, v3)
#elif PSX_TRIANGLE_SORT_CLOSEST_Z
	#define PSX_TRIANGLE_SORTING_FUNC(v1, v2, v3) GetTriangleSortingDepth_ClosestVertexDepth(v1, v2, v3)
#elif PSX_TRIANGLE_SORT_CENTER_VIEWDIST
	#define PSX_TRIANGLE_SORTING_FUNC(v1, v2, v3) GetTriangleSortingDepth_LinearCenterDistance(v1, v2, v3)
#elif PSX_TRIANGLE_SORT_CLOSEST_VIEWDIST
	#define PSX_TRIANGLE_SORTING_FUNC(v1, v2, v3) GetTriangleSortingDepth_LinearClosestVertexDistance(v1, v2, v3)
#else
	#define PSX_TRIANGLE_SORTING_FUNC(v1, v2, v3) GetTriangleSortingDepth_Custom(v1, v2, v3)
#endif