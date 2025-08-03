Shader "Unlit/DrawTrack"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TrackStamp("TrackStamp",2D)="white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
    
        Pass
{
  
    Tags { "LightMode"="UniversalForward" }
    ZWrite On
    ColorMask RGBA
    Cull Off
    HLSLPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
    float3 _DeltaPos;
    CBUFFER_END

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
    };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;
        OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
        return OUT;
    }

    float4 frag(Varyings IN) : SV_Target
    {

        return float4(0,0,0,1);
    }
    ENDHLSL
}

        Pass
{
  
    Tags { "LightMode"="UniversalForward" }
    ZWrite On
    ColorMask RGBA
    Cull Off
    HLSLPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)

    CBUFFER_END

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
    };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;
        OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
        return OUT;
    }

    float4 frag(Varyings IN) : SV_Target
    {
        
        return float4(0,0,0,0);
    }
    ENDHLSL
}
    }
}
