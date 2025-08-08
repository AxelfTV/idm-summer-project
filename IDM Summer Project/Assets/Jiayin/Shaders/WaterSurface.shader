Shader "Unlit/WaterSurface"
{
Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaterSurface("WaterSurface",2D)= "white" {}
        _Color ("FresnelColor", Color) = (1,1,1,1)
        _WaterColor("WaterColor", Color) = (0,0,1,1)
        _WaterGradient ("Water Gradient", 2D) = "white" {}
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _FadeDistance("Fade Distance", Float) = 10.0
        _WeaveStrength("Weave Strength",float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "UniversalMaterialType"="Lit" "DecalMeshForwardEmissive" = "True" }
        //LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            ZTest LEqual
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
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
                float4 _Color;
                float3 _WaterColor;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
                float _FadeDistance;
                float _WeaveStrength;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_WaterGradient); SAMPLER(sampler_WaterGradient);
            TEXTURE2D(_WaterSurface); SAMPLER(sampler_WaterSurface);
//            TEXTURE2D(_CameraDepthTexture);       SAMPLER(sampler_CameraDepthTexture);

            // float4 ComputeScreenPos(float4 positionCS,float ProjectionSign)
            // {
            //     float4 screenPos = positionCS*0.5f;
            //     screenPos.xy = (screenPos.x,screenPos.y*ProjectionSign) + screenPos.w; // Convert to [0,1] range
            //     screenPos.zw=positionCS.zw;
            //     return screenPos;
            // }
            // float SampleSceneDepth(float2 uv)
            // {
            //     // Sample the scene depth texture
            //     float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture,uv).r;
            //     return depth;
            // }

            // float GetDepthFade(float3 worldPos, float fadeDistance)
            // {
            //     float4 positionCS = TransformWorldToHClip(worldPos);
            //     float4 ScreenPosition = ComputeScreenPos(TransformWorldToHClip(worldPos));
            //     float EyeDepth = LinearEyeDepth(SampleSceneDepth(ScreenPosition.xy/ScreenPosition.w),_ZBufferParams);
            // //    return ScreenPosition.y/ ScreenPosition.w;
            //     return EyeDepth;
            //     return saturate((EyeDepth - positionCS.w) / fadeDistance);
            // }
            float2 DistortUV_float(float2 UV, float Amount)
            {
                float time = _Time.y;

                UV.y += Amount * 0.01 * (sin(UV.x * 3.5 + time * 0.35) + sin(UV.x * 4.8 + time * 1.05) + sin(UV.x * 7.3 + time * 0.45)) / 3.0;
                UV.x += Amount * 0.12 * (sin(UV.y * 4.0 + time * 0.50) + sin(UV.y * 6.8 + time * 0.75) + sin(UV.y * 11.3 + time * 0.2)) / 3.0;
                UV.y += Amount * 0.12 * (sin(UV.x * 4.2 + time * 0.64) + sin(UV.x * 6.3 + time * 1.65) + sin(UV.x * 8.2 + time * 0.45)) / 3.0;
                return UV;
            }

            float3 GerstnerWave(float3 position, float steepness, float wavelength, float speed, float direction, inout float3 tangent, inout float3 binormal)
{
    direction = direction * 2 - 1;
    float2 d = normalize(float2(cos(3.14 * direction), sin(3.14 * direction)));
    float k = 2 * 3.14 / wavelength;                                           
    float f = k * (dot(d, position.xz) - speed * _Time.y);
    float a = steepness / k;

    tangent += float3(
    -d.x * d.x * (steepness * sin(f)),
    d.x * (steepness * cos(f)),
    -d.x * d.y * (steepness * sin(f))
    );

    binormal += float3(
    -d.x * d.y * (steepness * sin(f)),
    d.y * (steepness * cos(f)),
    -d.y * d.y * (steepness * sin(f))
    );

    return float3(
    d.x * (a * cos(f)),
    a * sin(f),
    d.y * (a * cos(f))
    );
}

float3 GerstnerWaves_float(float3 position, float steepness, float wavelength, float speed, float4 directions)
{
    float3 Offset = 0;
    float3 tangent = float3(1, 0, 0);
    float3 binormal = float3(0, 0, 1);

    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.x, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.y, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.z, tangent, binormal);
    Offset += GerstnerWave(position, steepness, wavelength, speed, directions.w, tangent, binormal);

  //  normal = normalize(cross(binormal, tangent));
    return Offset;
    //TBN = transpose(float3x3(tangent, binormal, normal));
}
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionWS += GerstnerWaves_float(OUT.positionWS, 0.1*_WeaveStrength, 0.5*_WeaveStrength, 0.1*_WeaveStrength, float4(0.25, 0.5, 0.75, 1.0));
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.screenUV=ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {

                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.positionWS);
      
                float2 uv=IN.screenUV.xy/IN.screenUV.w;
                float wavenoise= FBMvalueNoise(IN.uv*50+_Time.y*0.5)*2-1;
                float depth=SampleSceneDepth(uv+wavenoise*0.01);
                float eyedepth=LinearEyeDepth(depth,_ZBufferParams);
          //      float3 DepthWorld=(IN.positionWS-_WorldSpaceCameraPos.xyz)/IN.positionCS.w *eyedepth+_WorldSpaceCameraPos;
                if(abs(IN.screenUV.w-depth)<0.1)
                {
                     depth = SampleSceneDepth(uv);
                     eyedepth=LinearEyeDepth(depth,_ZBufferParams);
                }
              //  return float4(eyedepth.xxx,1); 
                float Waterdepth=1-saturate((eyedepth-IN.screenUV.w)/_FadeDistance);
                //mix water color
                float4 waterColor = SAMPLE_TEXTURE2D(_WaterGradient, sampler_WaterGradient, float2(floor(Waterdepth*10/2)/5, 0.5));
               
                float3 WaterSurface = SAMPLE_TEXTURE2D(_WaterSurface, sampler_WaterSurface, DistortUV_float(IN.uv*10,1)).rgb;
                
                waterColor.rgb = lerp(waterColor.rgb,_WaterColor , step(0.05,WaterSurface.b));

                float fomes=step(0.4,Waterdepth*(WaterSurface.r+WaterSurface.g));
                waterColor.rgb +=fomes;
                 //fresnel
                float fresnel = pow(1.0 - saturate(dot(IN.normalWS, viewDir)), 8); 
                waterColor.rgb = lerp(waterColor.rgb, _Color.rgb, fresnel);
                return half4(waterColor.rgb ,1);
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb*_Color;
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
              //  half shadowAtten = mainLight.shadowAttenuation;
                half shadowAtten=MainLightRealtimeShadow(IN.shadowCoord);
                //  return half4(NdotL * shadowAtten.xxx,1);
                half toonStep1 = NdotL * shadowAtten > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL * shadowAtten>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
                          

                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL * shadowAtten);
                half3 baseColor = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2);
            
                ApplyDecalToBaseColor(IN.positionCS, baseColor);
                return half4(baseColor, 1.0);
            }
            ENDHLSL
        }

        // 阴影投射
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}
