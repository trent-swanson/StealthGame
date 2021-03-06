﻿Shader "Custom/Outline" {
	Properties {
		_OutlineColor("Outline Color", Color) = (255,125,41,255)
		_OutlineWidth("Outline Width", Range(1.0,5.0)) = 1
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : POSITION;
		float3 normal : NORMAL;
	};

	float _OutlineWidth;
	float4 _OutlineColor;

	v2f vert(appdata v) {
		v.vertex.xyz *= _OutlineWidth;

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		return o;
	}

	ENDCG

	SubShader {
		Tags{ "Queue" = "Transparent+1" }
		Tags { "RenderType"="Opaque" }
		LOD 200

		pass //Render on top
		{
			Cull Off
			ZWrite Off

			ZTest Always

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 frag(v2f i) : SV_Target {
				return _OutlineColor;
			}

			ENDCG
		}

		pass //Render the outline
		{
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}

			ENDCG
		}

		ZWrite On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

