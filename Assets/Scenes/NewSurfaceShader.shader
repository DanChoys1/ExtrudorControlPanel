Shader "Custom/TextureBlend"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BlendTex ("Blend Texture", 2D) = "white" {}
        _Blend ("Blend Amount", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _MainTex;
        sampler2D _BlendTex;
        float _Blend;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 color1 = tex2D(_MainTex, IN.uv_MainTex);
            half4 color2 = tex2D(_BlendTex, IN.uv_MainTex);
            o.Albedo = lerp(color1.rgb, color2.rgb, _Blend);
            o.Alpha = lerp(color1.a, color2.a, _Blend);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
