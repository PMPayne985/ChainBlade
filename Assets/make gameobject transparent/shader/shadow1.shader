Shader "Custom/Player"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Tex("Tex", 2D) = "white" {}
		_BackTex("BackTex", 2d) = "white" {}
		_MaskColor("Mask Color", Color) = (1,1,1,0.5)
		_BackIntensity("BackIntensity", Range(0.0, 1.0)) = 0.2
		_FrontIntensity("FrontIntensity", Range(0.0, 1.0)) = 0.2
	}
		SubShader
		{
			// Project Shadow
			Pass
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }

				CGPROGRAM

				#pragma vertex vertShadow
				#pragma fragment fragShadow
				#pragma multi_compile_shadowcaster

				#include "UnityCG.cginc"

				struct v2f
				{
					V2F_SHADOW_CASTER;
				};

				v2f vertShadow(appdata_base v)
				{
					v2f o;
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
					return o;
				}

				fixed4 fragShadow(v2f i) : SV_Target
				{
					SHADOW_CASTER_FRAGMENT(i);
				}

				ENDCG
			}

			// Draw Back
			Pass
			{
				Name "DrawBack"
				Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				ZWRITE Off
				Cull Front

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile SHADOWS_SCREEN

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct a2f
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 worldNormal : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
					SHADOW_COORDS(2)
					float2 uv : TEXCOORD3;
					fixed3 diffuse : COLOR0;
					fixed3 ambient : COLOR1;
				};

				fixed4 _TintColor;
				sampler2D _Tex;
				float4 _Tex_ST;
				float _BackIntensity;

				v2f vert(a2f v)
				{
					v2f o = (v2f)0;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _Tex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
					fixed3 normalDir = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
					o.diffuse = _LightColor0.rgb * ((dot(lightDir,normalDir) + 1) * 0.5);
					o.ambient = UNITY_LIGHTMODEL_AMBIENT;
					TRANSFER_SHADOW(o);
					return o;
				}

				fixed4 frag(v2f i) : SV_TARGET
				{
					//fixed shadow = SHADOW_ATTENUATION(i);
					UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
					fixed3 albedo = tex2D(_Tex, i.uv) * _TintColor.rgb;
					fixed3 diffuse = i.diffuse * albedo * _BackIntensity;
					atten = max(0.5, atten);
					return fixed4(i.ambient + diffuse * atten, _TintColor.a);
				}

				ENDCG
			}
					// Draw Front
					Pass
					{
						Name "DrawFront"
						Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
						Blend SrcAlpha OneMinusSrcAlpha
						ZWRITE Off

						CGPROGRAM

						#pragma vertex vert
						#pragma fragment frag
						#pragma multi_compile SHADOWS_SCREEN

						#include "UnityCG.cginc"
						#include "Lighting.cginc"
						#include "AutoLight.cginc"

						struct a2f
						{
							float4 vertex : POSITION;
							float2 uv : TEXCOORD0;
							float3 normal : NORMAL;
						};

						struct v2f
						{
							float4 pos : SV_POSITION;
							float3 worldNormal : TEXCOORD0;
							float3 worldPos : TEXCOORD1;
							SHADOW_COORDS(2)
							float2 uv : TEXCOORD3;
							fixed3 diffuse : COLOR0;
							fixed3 ambient : COLOR1;
						};

						fixed4 _TintColor;
						sampler2D _Tex;
						float4 _Tex_ST;
						float _FrontIntensity;

						v2f vert(a2f v)
						{
							v2f o = (v2f)0;
							o.pos = UnityObjectToClipPos(v.vertex);
							o.uv = TRANSFORM_TEX(v.uv, _Tex);
							o.worldNormal = UnityObjectToWorldNormal(v.normal);
							o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
							fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
							fixed3 normalDir = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
							o.diffuse = _LightColor0.rgb * ((dot(lightDir,normalDir) + 1) * 0.5);
							o.ambient = UNITY_LIGHTMODEL_AMBIENT;
							TRANSFER_SHADOW(o);
							return o;
						}

						fixed4 frag(v2f i) : SV_TARGET
						{
							//fixed shadow = SHADOW_ATTENUATION(i);
							UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
							fixed3 albedo = tex2D(_Tex, i.uv) * _TintColor.rgb;
							fixed3 diffuse = i.diffuse * albedo * _FrontIntensity;
							atten = max(0.5, atten);
							return fixed4(i.ambient + diffuse * atten, _TintColor.a);
						}

						ENDCG
					}
					// Draw Mask
					Pass
					{
						Name "DrawMask"
						ZTest Greater
						ZWrite Off
						Blend SrcAlpha OneMinusSrcAlpha

						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag

						#include "UnityCG.cginc"

						struct appdata
						{
							float4 vertex : POSITION;
						};

						struct v2f
						{
							float4 vertex : SV_POSITION;
						};

						float4 _MaskColor;

						v2f vert(appdata v)
						{
							v2f o = (v2f)0;
							o.vertex = UnityObjectToClipPos(v.vertex);
							return o;
						}

						fixed4 frag(v2f i) : SV_Target
						{
							return _MaskColor;
						}
						ENDCG
					}
		}
			FallBack "Diffuse"
}
