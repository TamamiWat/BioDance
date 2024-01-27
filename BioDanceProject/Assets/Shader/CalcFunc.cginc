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