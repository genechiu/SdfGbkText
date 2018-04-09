Shader "SDF/GBK/Text" {
	Properties {
		_MainTex("Font Texture",2D)="white"{}
		[Toggle(OUTLINE_ON)]_OutlineOn("Outline On",Int)=0
		_OutlineColor("Outline Color",Color)=(0,0,0,1)
		_OutlineWidth("Outline Width",float)=1
	}
	
	SubShader {
		
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		Lighting Off
		Cull Off
		ZTest Always
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature __ OUTLINE_ON
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 position:POSITION;
				fixed4 color:COLOR;
				float2 texcoord0:TEXCOORD0;
				float2 texcoord1:TEXCOORD1;
			};
			
			struct v2f {
				float4 vertex:SV_POSITION;
				fixed4 channel:COLOR;
				fixed4 faceColor:COLOR1;
				fixed4 outlineColor:COLOR2;
				float2 texcoord:TEXCOORD0;
				float4 param:TEXCOORD1;
			};
			
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _OutlineColor;
			uniform float _OutlineWidth;
			
			v2f vert(appdata_t input){
				float2 pixelSize=float2(1,1)/abs(mul((float2x2)UNITY_MATRIX_P,_ScreenParams.xy));
				float scale=rsqrt(dot(pixelSize,pixelSize))*abs(input.texcoord1.y)*4.5;
				float bias=(0.5-input.texcoord1.x*0.125)*scale-0.5;
				
				fixed4 channel=saturate(ceil(input.color-0.5));
				fixed4 faceColor=saturate((channel*0.51-input.color)/-0.49);
				fixed4 outlineColor=_OutlineColor;
				outlineColor.a*=faceColor.a;
				
				v2f output={
					UnityObjectToClipPos(input.position),
					channel,
					faceColor,
					outlineColor,
					input.texcoord0,
					half4(scale,bias,bias+_OutlineWidth*0.0125,bias-_OutlineWidth*0.9875),
				};
				
				return output;
			}
			
			fixed4 frag(v2f input):SV_Target{
				fixed4 col=tex2D(_MainTex,input.texcoord.xy)*input.channel;
				float d=(col.a+col.r+col.g+col.b)*input.param.x;
#if OUTLINE_ON
				col=lerp(input.outlineColor,input.faceColor,saturate(d-input.param.z))*saturate(d-input.param.w);
#else
				col=input.faceColor*saturate(d-input.param.y);
#endif
				return col;
			}
			ENDCG
		}
	}
}
