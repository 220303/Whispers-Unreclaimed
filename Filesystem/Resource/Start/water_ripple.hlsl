sampler2D input : register(s0);

float2 center; // 鼠标扰动中心（0~1）
float time; // 时间
float amplitude; // 波幅
float frequency; // 频率
float speed; // 速度
float aspect; // 宽高比（新增）

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 dir = uv - center;
    dir.x *= aspect; // 修正横向距离，保证dist为圆形
    float dist = length(dir);

    //设置ripple (attenuation是为了设置ripple产生的)
    float attenuation = exp(-dist * 18); // 衰减系数，8可以调整
    float ripple = sin(dist * frequency - time * speed) * amplitude * attenuation;

    // 反向修正offset，保证扰动方向正确
    float2 offset = dir * ripple;
    offset.x /= aspect;

    return tex2D(input, uv + offset);
}
