Shader "GameTextures/GT_Metallic"
{
	Properties
	{
		_UVTileOffset("UV Tile / Offset", Vector) = (1,1,0,0)
		[NoScaleOffset]_Opacity("Opacity", 2D) = "white" {}
		[NoScaleOffset]_BaseColor("Base Color", 2D) = "white" {}
		_BaseColorTint("Base Color Tint", Color) = (1,1,1,1)
		_Cutoff( "Mask Clip Value", Float ) = 1
		[NoScaleOffset]_Metallic("Metallic", 2D) = "black" {}
		[Toggle(_INVERT_ROUGHNESS_ON)] _Invert_Roughness("Invert_Roughness", Float) = 0
		_RoughnessMultiplier("Roughness Multiplier", Range( 0 , 10)) = 1
		[NoScaleOffset]_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_AmbientOcclusionMultiplier("Ambient Occlusion Multiplier", Range( 0 , 10)) = 1
		[NoScaleOffset]_Height("Height", 2D) = "white" {}
		[NoScaleOffset][Normal]_Normal("Normal", 2D) = "white" {}
		[Toggle(_INVERT_NORMAL_ON)] _Invert_Normal("Invert_Normal", Float) = 0
		[NoScaleOffset]_Emissive("Emissive", 2D) = "black" {}
		_EmissiveMultiplier("Emissive Multiplier", Range( 0 , 10)) = 1
		[Toggle(_USE_POM_ON)] _Use_POM("Use_POM", Float) = 0
		_POMHeight("POM Height", Range( 0 , 1)) = 0
		_CurvatureU("Curvature U", Range( 0 , 100)) = 0
		_CurvatureV("Curvature V", Range( 0 , 30)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[Header(Parallax Occlusion Mapping)]
		_CurvFix("Curvature Bias", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _INVERT_NORMAL_ON
		#pragma shader_feature _USE_POM_ON
		#pragma shader_feature _INVERT_ROUGHNESS_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif

		struct Input
		{
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float3 worldNormal;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _UVTileOffset;
		uniform sampler2D _Height;
		uniform float _POMHeight;
		uniform float _CurvFix;
		uniform float _CurvatureU;
		uniform float _CurvatureV;
		uniform float4 _Height_ST;
		uniform float4 _BaseColorTint;
		uniform sampler2D _BaseColor;
		uniform sampler2D _Emissive;
		uniform float _EmissiveMultiplier;
		uniform float _RoughnessMultiplier;
		uniform sampler2D _Metallic;
		uniform sampler2D _AmbientOcclusion;
		uniform float _AmbientOcclusionMultiplier;
		uniform sampler2D _Opacity;
		uniform float _Cutoff = 1;


		inline float2 POM( sampler2D heightMap, float2 uvs, float2 dx, float2 dy, float3 normalWorld, float3 viewWorld, float3 viewDirTan, int minSamples, int maxSamples, float parallax, float refPlane, float2 tilling, float2 curv, int index )
		{
			float3 result = 0;
			int stepIndex = 0;
			int numSteps = ( int )lerp( (float)maxSamples, (float)minSamples, saturate( dot( normalWorld, viewWorld ) ) );
			float layerHeight = 1.0 / numSteps;
			float2 plane = parallax * ( viewDirTan.xy / viewDirTan.z );
			uvs += refPlane * plane;
			float2 deltaTex = -plane * layerHeight;
			float2 prevTexOffset = 0;
			float prevRayZ = 1.0f;
			float prevHeight = 0.0f;
			float2 currTexOffset = deltaTex;
			float currRayZ = 1.0f - layerHeight;
			float currHeight = 0.0f;
			float intersection = 0;
			float2 finalTexOffset = 0;
			while ( stepIndex < numSteps + 1 )
			{
				result.z = dot( curv, currTexOffset * currTexOffset );
				currHeight = tex2Dgrad( heightMap, uvs + currTexOffset, dx, dy ).r * ( 1 - result.z );
				if ( currHeight > currRayZ )
				{
					stepIndex = numSteps + 1;
				}
				else
				{
					stepIndex++;
					prevTexOffset = currTexOffset;
					prevRayZ = currRayZ;
					prevHeight = currHeight;
					currTexOffset += deltaTex;
					currRayZ -= layerHeight * ( 1 - result.z ) * (1+_CurvFix);
				}
			}
			int sectionSteps = 10;
			int sectionIndex = 0;
			float newZ = 0;
			float newHeight = 0;
			while ( sectionIndex < sectionSteps )
			{
				intersection = ( prevHeight - prevRayZ ) / ( prevHeight - currHeight + currRayZ - prevRayZ );
				finalTexOffset = prevTexOffset + intersection * deltaTex;
				newZ = prevRayZ - intersection * layerHeight;
				newHeight = tex2Dgrad( heightMap, uvs + finalTexOffset, dx, dy ).r;
				if ( newHeight > newZ )
				{
					currTexOffset = finalTexOffset;
					currHeight = newHeight;
					currRayZ = newZ;
					deltaTex = intersection * deltaTex;
					layerHeight = intersection * layerHeight;
				}
				else
				{
					prevTexOffset = finalTexOffset;
					prevHeight = newHeight;
					prevRayZ = newZ;
					deltaTex = ( 1 - intersection ) * deltaTex;
					layerHeight = ( 1 - intersection ) * layerHeight;
				}
				sectionIndex++;
			}
			#ifdef UNITY_PASS_SHADOWCASTER
			if ( unity_LightShadowBias.z == 0.0 )
			{
			#endif
				if ( result.z > 1 )
					clip( -1 );
			#ifdef UNITY_PASS_SHADOWCASTER
			}
			#endif
			return uvs + finalTexOffset;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_appendXY = (float2(_UVTileOffset.x , _UVTileOffset.y));
			float2 uv_appendZW = (float2(_UVTileOffset.z , _UVTileOffset.w));
			float2 uv_TexCoord = i.uv_texcoord * uv_appendXY + uv_appendZW;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 uv_curvature = (float2(_CurvatureU , _CurvatureV));
			float2 pom_offset = POM( _Height, uv_TexCoord, ddx(uv_TexCoord), ddy(uv_TexCoord), ase_worldNormal, ase_worldViewDir, i.viewDir, 16, 16, _POMHeight, 0, _Height_ST.xy, uv_curvature, 0 );
			float2 pom_uvs = pom_offset;
			#ifdef _USE_POM_ON
				float pom_switch = 1.0;
			#else
				float pom_switch = 0.0;
			#endif
			float2 lerp_pom = lerp( uv_TexCoord , pom_uvs , pom_switch);
			float3 tex2D_normal = UnpackNormal( tex2D( _Normal, lerp_pom, ddx( uv_TexCoord ), ddy( uv_TexCoord ) ) );
			#ifdef _INVERT_NORMAL_ON
				float3 invert_norm_switch = ( float3(1,-1,1) * tex2D_normal );
			#else
				float3 invert_norm_switch = tex2D_normal;
			#endif
			o.Normal = invert_norm_switch;
			o.Albedo = ( _BaseColorTint * tex2D( _BaseColor, lerp_pom ) ).rgb;
			float4 tex2D_emissive = tex2D( _Emissive, lerp_pom );
			o.Emission = tex2D_emissive.rgb;
			o.Metallic = ( tex2D_emissive * _EmissiveMultiplier ).r;
			float4 tex2D_metallic = tex2D( _Metallic, lerp_pom );
			#ifdef _INVERT_ROUGHNESS_ON
				float invert_roughness_switch = ( 1.0 - tex2D_metallic.a );
			#else
				float invert_roughness_switch = tex2D_metallic.a;
			#endif
			o.Smoothness = ( _RoughnessMultiplier * invert_roughness_switch );
			o.Occlusion = ( tex2D( _AmbientOcclusion, lerp_pom ) / _AmbientOcclusionMultiplier ).r;
			o.Alpha = 1;
			clip( tex2D( _Opacity, lerp_pom ).r - _Cutoff );
		}

		ENDCG

		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 
		ENDCG
        
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = IN.tSpace0.xyz * worldViewDir.x + IN.tSpace1.xyz * worldViewDir.y + IN.tSpace2.xyz * worldViewDir.z;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}