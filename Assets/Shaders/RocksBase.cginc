#ifndef ROCKS_BASE_INCLUDE
#define ROCKS_BASE_INCLUDE

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "Autolight.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2g
{
    float2 uv : TEXCOORD0;
    UNITY_FOG_COORDS(2)
    float4 vertex : SV_POSITION;
};

struct g2f
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD4;
    float3 normal : TEXCOORD3;
    UNITY_FOG_COORDS(2)

#if UNITY_PASS_SHADOWCASTER

#else
        unityShadowCoord4 _ShadowCoord : TEXCOORD1;
#endif

};

float _RandomTranslation;
float _Rotation;
float _Cutoff;
float _Scale;
sampler2D _Noise;
float4 _Noise_ST;

v2g vert(appdata v)
{
    v2g o;
    o.vertex = v.vertex;
    o.uv = v.uv;
    UNITY_TRANSFER_FOG(o, o.vertex);
    return o;
}

g2f createVertex(float4 root, float3 position, float2 uv, float3 forward, float3x3 transformMatrix) {
    g2f o;
    position = mul(transformMatrix, position);
    o.uv = TRANSFORM_TEX(uv, _Noise);
    o.pos = UnityObjectToClipPos(root + float4(position, 1));
    o.worldPos = mul(unity_ObjectToWorld, position).xyz;
    o.normal = mul(transformMatrix, forward);

    #if UNITY_PASS_SHADOWCASTER
        // Applying the bias prevents artifacts from appearing on the surface.
        o.pos = UnityApplyLinearShadowBias(o.pos);
    #else
        o._ShadowCoord = ComputeScreenPos(o.pos);
        TRANSFER_SHADOW(o);
    #endif
    
    UNITY_TRANSFER_FOG(o, o.pos);

    return o;
}

void AppendTriangle(inout TriangleStream<g2f> triStream, float4 root, float2 uv, float3x3 transform, float3 t0, float3 t1, float3 t2) {

    t0 *= _Scale;
    t1 *= _Scale;
    t2 *= _Scale;

    float3 dir = cross(t1 - t0, t2 - t0);
    float3 normal = normalize(dir);
    triStream.Append(createVertex(root, t0, uv, normal, transform));
    triStream.Append(createVertex(root, t1, uv, normal, transform));
    triStream.Append(createVertex(root, t2, uv, normal, transform));
    triStream.RestartStrip();
}

float rand(float3 co)
{
    return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

float3x3 AngleAxis3x3(float angle, float3 axis)
{
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
    );
}


void Icosahedron(point v2g IN[1], inout TriangleStream<g2f> triStream) {

    float4 root = IN[0].vertex;
    float2 uv = IN[0].uv;

    float r1 = rand(float3(root.x, root.y, root.z)) * 2 - 1;
    float r2 = rand(float3(root.y, root.z, root.x)) * 2 - 1;
    float noise = tex2Dlod(_Noise, float4(uv.x, uv.y, 0, 0));

    //float rotation = _Rotation * 0.01745329252;
    float rotation = r1 * 360;
    root += float4(r1 * _RandomTranslation, 0, r2 * _RandomTranslation, 0);
    float3x3 transform = AngleAxis3x3(rotation, float3(0, 1, 0));

    float t = 1.61803398875; // (1 + sqrt(5)) / 2
    float s = 0.52573111211; // 1 / sqrt(1 + t * t)

    s *= 1 - noise;

    if (s < _Cutoff)
        return;

    float3 v0 = s * float3(t, 1, 0);
    float3 v1 = s * float3(-t, 1, 0);
    float3 v2 = s * float3(t, -1, 0);
    float3 v3 = s * float3(-t, -1, 0);
    float3 v4 = s * float3(1, 0, t);
    float3 v5 = s * float3(1, 0, -t);
    
    float3 v6 = s * float3(-1, 0, t);
    float3 v7 = s * float3(-1, 0, -t);
    float3 v8 = s * float3(0, t, 1);
    float3 v9 = s * float3(0, -t, 1);
    float3 v10 = s * float3(0, t, -1);
    float3 v11 = s * float3(0, -t, -1);

    AppendTriangle(triStream, root, uv, transform,
        v0, v8, v4
    );

    AppendTriangle(triStream, root, uv, transform,
        v0, v5, v10
    );

    AppendTriangle(triStream, root, uv, transform,
        v2, v4, v9
    );

    AppendTriangle(triStream, root, uv, transform,
        v2, v11, v5
    );

    AppendTriangle(triStream, root, uv, transform,
        v1, v6, v8
    );

    AppendTriangle(triStream, root, uv, transform,
        v1, v10, v7
    );

    AppendTriangle(triStream, root, uv, transform,
        v3, v9, v6
    );

    AppendTriangle(triStream, root, uv, transform,
        v3, v7, v11
    );

    AppendTriangle(triStream, root, uv, transform,
        v0, v10, v8
    );

    AppendTriangle(triStream, root, uv, transform,
        v1, v8, v10
    );

    AppendTriangle(triStream, root, uv, transform,
        v2, v9, v11
    );

    AppendTriangle(triStream, root, uv, transform,
        v3, v11, v9
    );

    AppendTriangle(triStream, root, uv, transform,
        v4, v2, v0
    );

    AppendTriangle(triStream, root, uv, transform,
        v5, v0, v2
    );

    AppendTriangle(triStream, root, uv, transform,
        v6, v1, v3
    );

    AppendTriangle(triStream, root, uv, transform,
        v7, v3, v1
    );

    AppendTriangle(triStream, root, uv, transform,
        v8, v6, v4
    );

    AppendTriangle(triStream, root, uv, transform,
        v9, v4, v6
    );

    AppendTriangle(triStream, root, uv, transform,
        v10, v5, v7
    );
    
    AppendTriangle(triStream, root, uv, transform,
        v11, v7, v5
    );
}

void Cube(point v2g IN[1], inout TriangleStream<g2f> triStream) {
    float4 root = IN[0].vertex;
    float2 uv = IN[0].uv;
    float3x3 transform = AngleAxis3x3(_Rotation * 0.01745329252, float3(0, 1, 0));
    //FRONT
    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, +0.5, +0.5), 
        float3(-0.5, +0.5, +0.5), 
        float3(+0.5, -0.5, +0.5)
    );
    
    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, -0.5, +0.5),
        float3(-0.5, +0.5, +0.5),
        float3(-0.5, -0.5, +0.5)
    );


    //BACK
    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, +0.5, -0.5),
        float3(+0.5, +0.5, -0.5),
        float3(+0.5, -0.5, -0.5)
    );

    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, +0.5, -0.5),
        float3(+0.5, -0.5, -0.5),
        float3(-0.5, -0.5, -0.5)
    );

    //TOP
    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, +0.5, +0.5),
        float3(+0.5, +0.5, +0.5),
        float3(+0.5, +0.5, -0.5)
    );

    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, +0.5, +0.5),
        float3(+0.5, +0.5, -0.5),
        float3(-0.5, +0.5, -0.5)
    );

    //BOTTOM
    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, -0.5, +0.5),
        float3(-0.5, -0.5, +0.5),
        float3(+0.5, -0.5, -0.5)
    );

    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, -0.5, -0.5),
        float3(-0.5, -0.5, +0.5),
        float3(-0.5, -0.5, -0.5)
    );


    //LEFT
    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, -0.5, +0.5),
        float3(-0.5, +0.5, +0.5),
        float3(-0.5, +0.5, -0.5)
    );

    AppendTriangle(triStream, root, uv, transform,
        float3(-0.5, -0.5, +0.5),
        float3(-0.5, +0.5, -0.5),
        float3(-0.5, -0.5, -0.5)
    );


    //RIGHT
    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, +0.5, +0.5),
        float3(+0.5, -0.5, +0.5),
        float3(+0.5, +0.5, -0.5)
    );

    AppendTriangle(triStream, root, uv, transform,
        float3(+0.5, +0.5, -0.5),
        float3(+0.5, -0.5, +0.5),
        float3(+0.5, -0.5, -0.5)
    );
}


[maxvertexcount(60)]
void geom(point v2g IN[1], inout TriangleStream<g2f> triStream) {

    Icosahedron(IN, triStream);
}

#endif