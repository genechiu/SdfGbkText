Shader "SDF/GBK/Text" {
	Properties {
		_MainTex("Font Texture",2D)="white"{}
		[Toggle(OUTLINE_ON)]_OutlineOn("Outline On",Int)=0
		_OutlineColor("Outline Color",Color)=(0,0,0,1)
		_OutlineWidth("Outline Width",float)=1
		[Toggle(SHADOW_ON)]_ShadowOn("Shadow On",Int)=0
		_ShadowColor("Shadow Color",Color)=(0,0,0,0.5)
		_ShadowOffsetX("Shadow Offset X",Range(-1,1))=1
		_ShadowOffsetY("Shadow Offset Y",Range(-1,1))=-1
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
			#pragma shader_feature __ SHADOW_ON
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
#if SHADOW_ON
				fixed4 shadowColor:COLOR3;
				float2 shadow:TEXCOORD2;
#endif
			};
			
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _OutlineColor;
			uniform float _OutlineWidth;
#if SHADOW_ON
			uniform fixed4 _ShadowColor;
			uniform float _ShadowOffsetX;
			uniform float _ShadowOffsetY;
#endif		
			v2f vert(appdata_t input){
				float2 pixelSize=float2(1,1)/abs(mul((float2x2)UNITY_MATRIX_P,_ScreenParams.xy));
				float scale=rsqrt(dot(pixelSize,pixelSize))*abs(input.texcoord1.y)*4.5;
				float bias=(0.5-input.texcoord1.x*0.05)*scale-0.5;
				
				fixed4 channel=saturate(ceil(input.color-0.5));
				fixed4 faceColor=saturate((channel*0.51-input.color)/-0.49);
				fixed4 outlineColor=_OutlineColor;
				outlineColor.a*=faceColor.a;
				
				v2f output;
				output.vertex=UnityObjectToClipPos(input.position);
				output.channel=channel;
				output.faceColor=faceColor;
				output.outlineColor=outlineColor;
				output.texcoord=input.texcoord0;
				output.param=float4(scale,bias,bias+_OutlineWidth*0.0125,bias-_OutlineWidth*0.9875);
#if SHADOW_ON
				fixed4 shadowColor=_ShadowColor;
				shadowColor.a*=faceColor.a;
				output.shadowColor=shadowColor;
				output.shadow.x=input.texcoord0.x-_ShadowOffsetX*0.001;
				output.shadow.y=input.texcoord0.y-_ShadowOffsetY*0.001;
#endif
				return output;
			}
			
			fixed4 frag(v2f input):SV_Target{
				fixed4 color=tex2D(_MainTex,input.texcoord.xy)*input.channel;
				float alpha=(color.a+color.r+color.g+color.b)*input.param.x;
#if OUTLINE_ON
				color=lerp(input.outlineColor,input.faceColor,saturate(alpha-input.param.z));
				alpha=saturate(alpha-input.param.w);
#else
				color=input.faceColor;
                alpha=saturate(alpha-input.param.y);
#endif
				color*=alpha;
#if SHADOW_ON
				fixed4 shadowColor=tex2D(_MainTex,input.shadow.xy)*input.channel;
				float shadowAlpha=(shadowColor.a+shadowColor.r+shadowColor.g+shadowColor.b)*input.param.x;
				color+=input.shadowColor*saturate(shadowAlpha-input.param.y)*(1-alpha);
#endif
				return color;
			}
			ENDCG
		}
	}
}
