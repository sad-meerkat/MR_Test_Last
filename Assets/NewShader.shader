/*
2026-06-09 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
Shader "Custom/StencilBackgroundTexture"
{
    Properties
    {
        _MainTex ("Background Texture", 2D) = "white" {} // 이미지를 넣을 수 있는 칸 추가
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _StencilID("Stencil Read ID", Range(0, 255)) = 255
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-2" "RenderType"="Transparent" }
        LOD 200

        Pass {
            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_StencilID]
                Comp Equal // 이 부분이 AR/VR/MR 시스템과 연동되는 핵심입니다.
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // 이미지 좌표 추가
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Opacity;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv); // 이미지에서 색상을 가져옴
                col.a *= _Opacity; // 버튼 조작에 따른 투명도 적용
                return col;
            }
            ENDCG
        }
    }
}