/*
2026-06-09 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
Shader "Custom/Stencil360Background"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("360 Background (Panoramic)", 2D) = "white" {}
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _StencilID("Stencil Read ID", Range(0, 255)) = 255
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-2" "RenderType"="Transparent" }
        
        Pass {
            Cull Front // 360도 배경이므로 메쉬의 안쪽 면이 보여야 합니다.
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_StencilID]
                Comp Equal // AR/VR/MR 연동 핵심
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Opacity;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // 메쉬의 중심에서 정점까지의 방향을 계산하여 360도 매핑에 사용
                o.viewDir = v.vertex.xyz; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 360도 파노라마 이미지를 구형 좌표로 변환하여 샘플링
                float3 dir = normalize(i.viewDir);
                float2 uv;
                uv.x = atan2(dir.z, dir.x) / (2.0 * UNITY_PI) + 0.5;
                uv.y = asin(dir.y) / UNITY_PI + 0.5;
                
                fixed4 col = tex2D(_MainTex, uv);
                col.a *= _Opacity;
                return col;
            }
            ENDCG
        }
    }
}