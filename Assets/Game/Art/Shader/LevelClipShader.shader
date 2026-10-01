Shader "Custom/LevelClipShader" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows addshadow
		#pragma target 3.0
		#pragma multi_compile_instancing

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;

		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP (fixed4, _Color)
#define _Color_arr Props
			UNITY_DEFINE_INSTANCED_PROP (float4, _ClipDirection)
#define _ClipDirection_arr Props
			UNITY_DEFINE_INSTANCED_PROP (float, _ClipLimit)
#define _ClipLimit_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 dir = UNITY_ACCESS_INSTANCED_PROP(_ClipDirection_arr, _ClipDirection).xy;
			float limit = UNITY_ACCESS_INSTANCED_PROP(_ClipLimit_arr, _ClipLimit);
			
			float direction = dot(float2(1,1), dir);
			clip(direction * (limit - direction * dot(IN.worldPos.xy, dir)));

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
