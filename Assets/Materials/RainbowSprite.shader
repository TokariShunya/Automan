Shader "Custom/RainbowSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Float) = 2.0
        _RainbowOffset ("Hue Offset", Float) = 0.0
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 localUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _EmissionStrength;
            float _RainbowOffset;

            // HSV to RGB (approximate)
            fixed3 HSVtoRGB(float h)
            {
                h = frac(h);
                float r = abs(h * 6 - 3) - 1;
                float g = 2 - abs(h * 6 - 2);
                float b = 2 - abs(h * 6 - 4);
                return saturate(float3(r, g, b));
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                // íÜêSÇ™(0,0)Ç…Ç»ÇÈÇÊÇ§Ç… [-1, 1] îÕàÕÇ…ïœä∑
                o.localUV = v.texcoord * 2.0 - 1.0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.texcoord) * _Color;

                float2 dir = i.localUV;
                float angle = atan2(dir.y, dir.x); // [-ÉŒ, ÉŒ]
                float normalizedAngle = (angle / (2 * UNITY_PI)) + 0.5; // [0,1]
                float hue = frac(normalizedAngle + _RainbowOffset);

                fixed3 rainbow = HSVtoRGB(hue);
                fixed4 finalColor = baseColor;
                finalColor.rgb += rainbow * _EmissionStrength * baseColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
