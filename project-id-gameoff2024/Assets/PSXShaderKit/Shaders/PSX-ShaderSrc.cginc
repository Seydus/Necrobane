struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
#if defined(PSX_VERTEX_LIT)||defined(PSX_CUBEMAP)
	float3 normal : NORMAL;
#endif
};

struct v2g
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
#ifdef PSX_VERTEX_LIT
	float3 normal : TEXCOORD1;
#endif
#ifdef PSX_CUBEMAP
	float3 reflectionDir : TEXCOORD2;
#endif
};

struct g2f
{
	float4 affineUV1 : TEXCOORD0;
	float4 vertex : SV_POSITION;
	float customDepth : TEXCOORD1;
#ifdef PSX_VERTEX_LIT
	float4 color : COLOR0;
#endif
	UNITY_FOG_COORDS(2)
#ifdef PSX_VERTEX_LIT
	float4 affineUV2 : TEXCOORD3;
#endif
#ifdef PSX_CUBEMAP
	float3 reflectionDir : TEXCOORD4;
#endif
};

struct fragOut
{
	half4 color : COLOR;
	float depth : DEPTH;
};

fixed4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;

#ifdef PSX_VERTEX_LIT
fixed _FlatShading;
fixed4 _EmissionColor;
sampler2D _EmissiveTex;
float4 _EmissiveTex_ST;
// 0 = average light, 1 = center light
fixed _PSX_FlatShadingMode;
#endif

fixed _PSX_VertexWobbleMode;
float _ObjectDithering;


#if defined(PSX_VERTEX_LIT)

float3 ShadePSXVertexLights (float4 vertex, float3 normal)
{
#ifdef PSX_ENABLE_CUSTOM_VERTEX_LIGHTING
	return ShadePSXVertexLightsFull(vertex, normal, 4, true);
#else
	return ShadeUnityVertexLightsFull(vertex, normal, 4, true);
#endif
    
}

#endif

v2g vert(appdata v)
{
	v2g o;
	o.vertex = v.vertex;
	o.uv = v.uv;

#ifdef PSX_VERTEX_LIT
	o.normal = v.normal;
#endif
	
#ifdef PSX_CUBEMAP
		float3 viewDir = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
		float3 normalDir = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
		o.reflectionDir = reflect(viewDir, normalDir);
#endif

	return o;
}


[maxvertexcount(3)]
void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
{
	float4x4 matrix_mv = UNITY_MATRIX_MV;
	float4x4 matrix_p = UNITY_MATRIX_P;
	
#ifndef PSX_TRIANGLE_SORT_OFF
	float triSortDepth = PSX_TRIANGLE_SORTING_FUNC(IN[0].vertex, IN[1].vertex, IN[2].vertex);
#else
	float triSortDepth = 0;
#endif

	// First pass to prepare data for all the triangles to potentially use later
	g2f o[3];
	for (int i = 0; i < 3; i++)
	{
		o[i].vertex = mul(matrix_mv, IN[i].vertex);
#ifdef PSX_VERTEX_LIT
		fixed3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, IN[i].normal));
		o[i].color.rgb = ShadePSXVertexLights(o[i].vertex, viewNormal);
		o[i].color.a = 1;
#endif

		o[i].affineUV1 = CalculateAffineUV(o[i].vertex, TRANSFORM_TEX(IN[i].uv, _MainTex));
#ifdef PSX_VERTEX_LIT
		o[i].affineUV2 = CalculateAffineUV(o[i].vertex, TRANSFORM_TEX(IN[i].uv, _EmissiveTex));
#endif
		
#ifndef PSX_TRIANGLE_SORT_OFF
		o[i].customDepth = triSortDepth;
#else
		o[i].customDepth = 0;
#endif

#ifdef PSX_CUBEMAP
		o[i].reflectionDir = IN[i].reflectionDir;
#endif
	}

	// Second pass to prepare the rest of the data now that we have pre-cached information for all vertices.
	// Here we can query triangle-wide data or choose to return and not emit the triangle at all.
#ifdef PSX_VERTEX_LIT
	float3 averageLight = (o[0].color + o[1].color + o[2].color) * 0.3333333f;

#ifdef PSX_FLAT_SHADING_MODE_CENTER
	float4 viewSpaceCenter = (o[0].vertex + o[1].vertex + o[2].vertex) * 0.3333333f;
	float3 viewSpaceNormal = normalize(cross(o[1].vertex - o[0].vertex, o[0].vertex - o[2].vertex));
	averageLight = ShadePSXVertexLights(viewSpaceCenter, viewSpaceNormal);
#endif

#endif

	for (i = 0; i < 3; i++)
	{
		float4 viewSnappedVertex = float4(SnapVertexToGrid(o[i].vertex.xyz), o[i].vertex.w);
		viewSnappedVertex = mul(matrix_p, viewSnappedVertex);
		float4 clipSnappedVertex = mul(matrix_p, o[i].vertex);
		clipSnappedVertex.xy = SnapVertexToGrid(clipSnappedVertex).xy;

		o[i].vertex = lerp(viewSnappedVertex, clipSnappedVertex, _PSX_VertexWobbleMode);

#ifdef PSX_VERTEX_LIT
		o[i].color.rgb = lerp(o[i].color.rgb, averageLight, _FlatShading);
#endif

		UNITY_TRANSFER_FOG(o[i], o[i].vertex);
		triStream.Append(o[i]);
	}

	triStream.RestartStrip();
}

#ifndef PSX_TRIANGLE_SORT_OFF
fragOut frag(g2f i, UNITY_VPOS_TYPE screenPos : SV_POSITION)
#else
fixed4 frag(g2f i, UNITY_VPOS_TYPE screenPos : SV_POSITION) : COLOR
#endif
{
	fragOut o;
	o.color = tex2D(_MainTex, i.affineUV1.xy / i.affineUV1.z) * _Color;
	
#ifdef PSX_VERTEX_LIT
	o.color *= i.color;
#endif

#ifdef PSX_CUTOUT_VAL
	clip(o.color.a - PSX_CUTOUT_VAL);
#endif

#ifdef PSX_VERTEX_LIT
	o.color.rgb += tex2D(_EmissiveTex, i.affineUV2.xy / i.affineUV2.z) * _EmissionColor.rgb * _EmissionColor.a;
#endif

#ifdef PSX_CUBEMAP
	o.color.rgb += texCUBE(PSX_CUBEMAP, i.reflectionDir) * PSX_CUBEMAP_COLOR.rgb * PSX_CUBEMAP_COLOR.a * tex2D(_ReflectionMap, i.affineUV1.xy / i.affineUV1.z);
#endif

#ifndef PSX_TRIANGLE_SORT_OFF
	o.depth = i.customDepth;
#else
	o.depth = 0;
#endif

	UNITY_APPLY_FOG(i.fogCoord, o.color);

	o.color = (_PSX_ObjectDithering * _ObjectDithering) > 0.5f ? PSX_DitherColor(o.color, screenPos) : o.color;

#if UNITY_COLORSPACE_GAMMA
	o.color.rgb = lerp(o.color.rgb, pow(o.depth, 1.0f / 2.2f), _PSX_DepthDebug);
#else
	o.color.rgb = lerp(o.color.rgb, o.depth, _PSX_DepthDebug);
#endif

#ifndef PSX_TRIANGLE_SORT_OFF
	return o;
#else
	return o.color;
#endif
}
