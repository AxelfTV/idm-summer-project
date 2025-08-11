Shader "Unlit/TreeLeaves"
{
Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2("Color2",Color)=(1,1,1,1)
        _Color3("Color3",Color)=(1,1,1,1)
        _GlossyColor("GlossyColor",Color)=(0,0,0,1)
        _GlossyAmount("GlossyAmount",float)=0
        _HeightFactor("Height Factor", float) = 1
        _HeightMove("Height Move",float) = 0
        _HeightTex("Height Texture", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _OutLineColor("OutlineColor",Color)=(0,0,0,1)
        _OutLineBlend("OutlineBlend",Range(0,1)) = 1  

        //wind
        _WindDir("WindDir",Vector)=(0,0,0,0)
        _WindSpeed("WindSpeed",float)=1
        _WindStrength("WindStrength",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Geometry" "UniversalMaterialType"="Lit" "DecalMeshForwardEmissive" = "True" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            ZTest LEqual
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float4 screenUV : TEXCOORD4; 
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color0;
                float4 _Color1;
                float4 _Color2;
                float4 _Color3;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;
                float4 _OutLineColor;
                float _OutLineBlend;
                //height mix
                float _HeightFactor;
                float _HeightMove;
                //HDR
                float3 _GlossyColor;
               float _GlossyAmount;
                //wind
                float2 _WindDir;
                float _WindSpeed;
                float _WindStrength;

                float4 _HeightTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalLineTex);
            SAMPLER(sampler_NormalLineTex);
            TEXTURE2D(_HeightTex); SAMPLER(sampler_HeightTex);
            
            TEXTURE2D(_DepthTex);
            SAMPLER(sampler_DepthTex);


            float4 MapMaskToFourColors(float mask, float4 c0, float4 c1, float4 c2, float4 c3)
            {
                float3 colors[4] = { c0.rgb, c1.rgb, c2.rgb, c3.rgb };
                float alpha[4]   = { c0.a,   c1.a,   c2.a,   c3.a   };

                float scaled = mask * 3.0;
                int idx = (int)floor(scaled);
                float t = frac(scaled);

                idx = clamp(idx, 0, 2);

                float3 rgb = lerp(colors[idx], colors[idx+1], t);
                float a    = lerp(alpha[idx], alpha[idx+1], t);

                return float4(rgb, a);
            }




            Varyings vert(Attributes IN)
            {
                Varyings OUT;

               
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz); 

                float Windnoise=FBMvalueNoise(OUT.positionWS.xz+_WindDir*_WindSpeed*_Time.y)*_WindStrength;
                OUT.positionWS.xz+=Windnoise;

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 positionOS= TransformWorldToObject(IN.positionWS);
                float heightMap=positionOS.y*_HeightFactor+_HeightMove;

                float heightTex= SAMPLE_TEXTURE2D(_HeightTex, sampler_HeightTex, IN.positionWS.yz*_HeightTex_ST.xy+_HeightTex_ST.zw).r;
                float Grassmask =saturate( heightMap*heightTex);
                
                float3 BaseColor= lerp(_Color0.rgb, _Color2.rgb, Grassmask);
                
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                clip( albedo -0.9);
                albedo*=BaseColor;

                Light mainLight = GetMainLight(IN.shadowCoord);

               //point light support
                half3 totalLight = mainLight.color.rgb * mainLight.distanceAttenuation;
                #if defined(_ADDITIONAL_LIGHTS)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < lightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, IN.positionWS);
                    totalLight += addLight.color.rgb * addLight.distanceAttenuation;
                }
                #endif

                // Toon shadow
                half NdotL = saturate(dot(IN.normalWS, mainLight.direction));
                half shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);

                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
               
               float4 MapColor= MapMaskToFourColors(NdotL*shadowAtten,_Color0,_Color1,_Color2,_Color3);
                return MapColor;
                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL*shadowAtten);
             
                half3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2); 
                baseColor+=_GlossyColor*_GlossyAmount;
                return half4(baseColor, 1.0);
            }
            ENDHLSL
        }
Pass
{
    Name "DepthOnly"
    Tags { "LightMode"="DepthOnly" }
    ZWrite On
    ColorMask 0
    HLSLPROGRAM
    #pragma vertex vert
    #pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        CBUFFER_START(UnityPerMaterial)
        float  _GrowFactor;
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
        // 顶点动画逻辑
        float GrowHeightMix=-IN.positionOS.y*0.1;
        IN.positionOS.xz = IN.positionOS.xz*(saturate(GrowHeightMix+_GrowFactor*16));  
        OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
        return OUT;
    }

    float frag(Varyings IN) : SV_Depth
    {
        return IN.positionCS.z / IN.positionCS.w;
    }
    ENDHLSL
} 


Pass
{
    Name "DepthNormals"
    Tags { "LightMode"="DepthNormals" }
    ZWrite On
    Cull Off
    ColorMask RGBA
    HLSLPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
     #include "../ShaderTools/NoiseLib.hlsl"


    CBUFFER_START(UnityPerMaterial)
                 //wind
                float2 _WindDir;
                float _WindSpeed;
                float _WindStrength;
    CBUFFER_END
    TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
    struct Attributes
    {
        float4 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 normalWS : TEXCOORD0;
        float2 uv:TEXCOORD1;
    };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;
        float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz); 
        float Windnoise=FBMvalueNoise(positionWS.xz+_WindDir*_WindSpeed*_Time.y)*_WindStrength;
        positionWS.xz+=Windnoise;

        OUT.positionCS = TransformWorldToHClip(positionWS);
        OUT.uv=IN.uv;
        OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
        return OUT;
    }

    float4 frag(Varyings IN) : SV_Target
    {
        half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
        clip( albedo -0.9);
        float3 normal = normalize(IN.normalWS);
        return float4(albedo,1);
        return float4(normal * 0.5 + 0.5, 1.0);
    }
    ENDHLSL
}

   UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}
