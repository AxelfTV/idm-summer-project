Shader "Custom/ModelCloud_Instancing"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex("Noise Tex",3D)= "white" {}
        _NoiseScale("Noise Scale",Float)=1.0
        _Color ("Color Tint", Color) = (1,1,1,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _StylishShadow ("Stylish Shadow", Float) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0.2,0.2,0.2,1)
        _ShadowColor2("ShadowColor2",color)=(0.5,0.5,0.5,0.5)
        _CloudSpeed("Cloud Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" 
            "Queue"="Transparent" }
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

            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "../ShaderTools/NoiseLib.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                 uint instanceID : SV_InstanceID; 
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                uint instanceID : SV_InstanceID;
            };
       void setup(){}
                sampler3D _NoiseTex;

              CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _ShadowThreshold;
                float4 _ShadowColor;
                float4 _ShadowColor2;
                float _StylishShadow;//shadow edge 
                float _NoiseScale;
                float  _CloudSpeed;
            CBUFFER_END 

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);


            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            struct clouds
            {
                float offset;
                float clipSize;
                float3  position;
                float3 scale;
                float4 rotation; // Quaternion for rotation
            };
            StructuredBuffer<clouds> _clouds;

            #endif
            float3 ApplyRotation(float3 pos, float4 q)
            {
                return pos + 2.0 * cross(q.xyz, cross(q.xyz, pos) + q.w * pos);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv ;

                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                clouds cloud = _clouds[IN.instanceID];
                float offset = cloud.offset;
                float3 localpos= IN.positionOS.xyz*cloud.scale;
                localpos = ApplyRotation(localpos, cloud.rotation);
                localpos+=offset * IN.normalOS;
                OUT.positionWS = TransformObjectToWorld(localpos + cloud.position);
                OUT.instanceID = IN.instanceID;
                #endif
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
             //   OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.shadowCoord = float4(1,1,1,1);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
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

                //sample noise
                float3 noiseUV = IN.positionWS * _NoiseScale;
                noiseUV.x += _Time.y* _CloudSpeed; // Add time for animation
                float noise=tex3D(_NoiseTex,noiseUV).x;

                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
              //  return half4(1,1,1,1);
                float clipSize= _clouds[IN.instanceID].clipSize;
                float dither = perlinNoise(IN.uv);
                clip(noise-pow(clipSize,0.5));
               // return half4(clipSize.xxx,1);
                #endif

              //  return half4(noise.xxx,1);
                // Toon shadow
                half NdotL = saturate(dot(IN.normalWS, mainLight.direction));
                half shadowAtten = mainLight.shadowAttenuation;

               // half toonStep1 = NdotL * shadowAtten*noise > _ShadowThreshold ? 1.0: 0.0;
              //  half toonStep2 = NdotL * shadowAtten*noise>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
            half toonStep1 = NdotL *noise > _ShadowThreshold ? 1.0: 0.0;
                half toonStep2 = NdotL *noise>_ShadowThreshold + _StylishShadow ? 1.0 : 0.0;
                float4 shadowColor=lerp(_ShadowColor, _ShadowColor2, NdotL *noise);
                half3 color = lerp(shadowColor.rgb*albedo*totalLight, albedo * totalLight, toonStep2);

                return half4(color, 1.0);
            }
            ENDHLSL
        }

        // 阴影投射

    //     UsePass "Universal Render Pipeline/Lit/DepthOnly"
     //   UsePass "Universal Render Pipeline/Lit/DepthNormals"
    //    UsePass "Universal Render Pipeline/Lit/ShadowCaster"

    }
    FallBack "Universal Render Pipeline/Lit"
}