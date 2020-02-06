/**
 *--------------------- 
 *作者：bboyxu 
 *来源：CSDN 
 *原文：https://blog.csdn.net/bboyxu/article/details/88952370 
 *版权声明：本文为博主原创文章，转载请附上博文链接！
 */
Shader "ShadersHub/LaserBeam"
{
    Properties
    {
		_MiddleColor("_MiddleColor color", Color) = (1,1,1,1)
		_EdgeColor("Edge color", Color) = (1,0,0,1)
		_Noise("Noise texture", 2D) = "gray"{}
		_NoiseIntensity("Noise intensity", Float) = 1
		_ScrollSpeed("Scroll speed", Float) = 1
		_StartBoost("Start boost", Float) = .5
		_EndBoost("End boost", Float) = .5
		_LineLength("LineLength", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderType" = "Transparent" }
        LOD 100

		//Blend SrcAlpha OneMinusSrcAlpha // 传统透明度
		//Blend One OneMinusSrcAlpha // 预乘透明度
		//Blend One One // 叠加
		//Blend OneMinusDstColor One // 柔和叠加
		//Blend DstColor Zero // 相乘——正片叠底
		//Blend DstColor SrcColor // 两倍相乘

		Blend One OneMinusSrcAlpha
		ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color:COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				float4 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
            };

            sampler2D _Noise;
			float4 _Noise_ST;

			fixed4 _MiddleColor;
			fixed4 _EdgeColor;
			half _NoiseIntensity;
			half _ScrollSpeed;
			half _StartBoost;
			half _EndBoost;

			half _LineLength;

            v2f vert (appdata v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//perlin noise texture
				fixed noise = tex2D(_Noise, (i.worldPos.xy * _Noise_ST.xy + _Time.x * _ScrollSpeed) + _Noise_ST.zw).r;
				noise = noise * 2 - 1;
				i.uv.y += noise * _NoiseIntensity;

				half centerness = 1 - abs(i.uv.y - .5) * 2;
				float middle = step(0.85, centerness);
				middle *= abs(sin(i.uv.x * 8 + _Time.x * _ScrollSpeed));
				float edge = step(0.5, centerness) - middle;
				edge *= abs(sin(i.uv.x * 15 + _Time.x * _ScrollSpeed));
				
				fixed4 col = _MiddleColor * middle + _EdgeColor * edge;
				col.rgb += _StartBoost * smoothstep(1, 0, (i.uv.x * _LineLength));
				col.rgb += _EndBoost * smoothstep(_LineLength - 1, _LineLength, (i.uv.x * _LineLength));

				col *= i.color;

				col.rgb *= col.a;
				col.rgb *= 2;
				col.a *= .5;
				
				return col;
            }
            ENDCG
        }
    }
}