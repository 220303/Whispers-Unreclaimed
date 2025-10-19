Texture2D tex : register(t0);
SamplerState samp : register(s0);

cbuffer Params : register(b0)
{
    float2 center;
    float time;
    float amplitude;
    float frequency;
    float speed;
    float aspect;
}

struct PS_INPUT
{
    float2 uv : TEXCOORD0;
};

float4 main(PS_INPUT input) : SV_Target
{
    float2 uv = input.uv;

    float2 dir = uv - center;
    dir.x *= aspect;
    float dist = length(dir);

    float attenuation = exp(-dist * 18);
    float ripple = sin(dist * frequency - time * speed) * amplitude * attenuation;

    float2 offset = dir * ripple;
    offset.x /= aspect;

    return tex.Sample(samp, uv + offset);
}
