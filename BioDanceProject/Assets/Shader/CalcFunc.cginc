float4x4 ConvertEulerToRotateMatrix(float3 angles)
{
    //X:Pitycos
    float pcos = cos(angles.x);
    float psin = sin(angles.x);
    //Y:Yaw
    float ycos = cos(angles.y);
    float ysin = sin(angles.y);
    //Z:Roll
    float rcos = cos(angles.z);
    float rsin = sin(angles.z);

    return float4x4(
        ycos * rcos + ysin * psin * rsin, -ycos * rsin + ysin * psin * rcos, ysin * pcos, 0,
        pcos * rsin, pcos * rcos, -psin, 0,
        -ysin * rcos + ycos * psin * rsin, ysin * rsin + ycos * psin * rcos, ycos * pcos, 0,
        0, 0, 0, 1
    );
}

float2 random2(float2 st, float seed)
{
    float2 s = float2(dot(st, float2(127.1, 311.7)) + seed, dot(st, float2(269.5, 183.3)) + seed);
    return -1.0 + 2.0 * frac(sin(s) * 43758.5453123);
}

/*float rand(float2 uv, int Seed)
{
    return frac(sin(dot(uv.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
}*/

float gradientNoise(float2 st, float seed)
{
    float2 i = floor(st);
    float2 f = frac(st);

    float2 u = f*f*(3.0-2.0*f);
    return lerp( lerp( dot( random2(i + float2(0.0,0.0), seed ), f - float2(0.0,0.0) ),
                                 dot( random2(i + float2(1.0,0.0), seed ), f - float2(1.0,0.0) ), u.x),
                            lerp( dot( random2(i + float2(0.0,1.0), seed ), f - float2(0.0,1.0) ),
                                 dot( random2(i + float2(1.0,1.0), seed ), f - float2(1.0,1.0) ), u.x), u.y);

}

float rand1(float n)
{
    return frac(sin(n) * 43758.5453);
}

float rand1dynamic(float n, float _Speed, float _RandomSize)
{
    return _RandomSize * frac(sin(n) * 43758.5453 * _Speed * _Time.y);
}
