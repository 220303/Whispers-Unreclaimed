sampler2D input : register(s0);

float2 center; // ����Ŷ����ģ�0~1��
float time; // ʱ��
float amplitude; // ����
float frequency; // Ƶ��
float speed; // �ٶ�
float aspect; // ��߱ȣ�������

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 dir = uv - center;
    dir.x *= aspect; // ����������룬��֤distΪԲ��
    float dist = length(dir);

    //����ripple (attenuation��Ϊ������ripple������)
    float attenuation = exp(-dist * 18); // ˥��ϵ����8���Ե���
    float ripple = sin(dist * frequency - time * speed) * amplitude * attenuation;

    // ��������offset����֤�Ŷ�������ȷ
    float2 offset = dir * ripple;
    offset.x /= aspect;

    return tex2D(input, uv + offset);
}
