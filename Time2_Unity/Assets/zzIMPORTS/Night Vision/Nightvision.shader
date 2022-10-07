// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Nightvision"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0, 1, 0, 1)
		//Vignette
		_VignetteColor ("Vignette Color", Color) = (0, 0, 0, 1)
		_VignetteTex ("Vignette", 2D) = "white" {}
		_VignetteRadius ("Vignette Radius", float) = 1.0
		_VignetteSharpness("Vignette Sharpness", float) = 1.0
		//Noise
		_NoiseTex ("Noise", 2D) = "white" {}
		_NoisePower("Noise Power", float) = 1.0
		_NoiseTileX ("Noise Tile X", float) = 1.0
		_NoiseTileY ("Noise Tile Y", float) = 1.0
		_NoiseOffsetX ("Noise Offset X", float) = 0.0
		_NoiseOffsetY("Noise Offset Y", float) = 0.0
		//Power
		_Power ("Power", float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile Vignette_Off Vignette_Texture Vignette_Procedural
			#pragma multi_compile Noise_Off Noise_Texture Noise_Procedural
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 _Color;
			sampler2D _MainTex;
			sampler2D _VignetteTex;
			sampler2D _NoiseTex;

			fixed4 _VignetteColor;
			float _VignetteRadius;
			float _VignetteSharpness;

			float _NoisePower;
			half _NoiseTileX;
			half _NoiseTileY;
			fixed _NoiseOffsetX;
			fixed _NoiseOffsetY;

			float _Power;

			//Vignette
			float vignettePower(float2 pos)
			{
				return saturate(pow(length(pos) * (1 / (_VignetteRadius / 2)), _VignetteSharpness));
			}
			half4 ApplyVignette(fixed4 col, fixed2 uv)
			{
				half4 newCol = col;
			#if Vignette_Texture
				half4 vig = tex2D(_VignetteTex, uv);
				newCol = lerp(col, vig * _VignetteColor, vig.a * _VignetteColor.a);
			#elif Vignette_Procedural
				float vig = vignettePower(uv - fixed2(0.5, 0.5));
				newCol = lerp(col, _VignetteColor, vig * _VignetteColor.a);
			#endif
				return newCol;
			}
			//Noise
			float rand(float2 pos)
 			{
     			return frac(sin(dot(pos.xy ,float2(135.194, 339.343))) * 9124.53);
 			}
			half4 ApplyNoise(fixed4 col, fixed2 uv)
			{
				half4 newCol = col;
			#if Noise_Texture
				half2 NoiseTile = half2(_NoiseTileX, _NoiseTileY);
				fixed2 NoiseOffset = fixed2(_NoiseOffsetX, _NoiseOffsetY);

				uv = uv * NoiseTile;
				fixed noise = saturate((1 - dot(tex2D(_NoiseTex, uv + NoiseOffset), fixed3(0.21, 0.72, 0.07))) * _NoisePower);
				fixed colGR = dot(col, fixed3(0.21, 0.72, 0.07));
				newCol = lerp(col, fixed4(0, 0, 0, 1), noise * (1 - colGR));
			#elif Noise_Procedural
				fixed2 NoiseOffset = fixed2(_NoiseOffsetX, _NoiseOffsetY);

				fixed noise = saturate((1 - rand(uv + NoiseOffset)) * _NoisePower);
				fixed colGR = dot(col, fixed3(0.21, 0.72, 0.07));
				newCol = lerp(col, fixed4(0, 0, 0, 1), noise * (1 - colGR));
			#endif
				return newCol;
			}

			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				col = mul(dot(col, fixed3(0.21, 0.72, 0.07)) * _Power, _Color)/*fixed4(0, dot(col, fixed3(0.21, 0.72, 0.07)) * _Power, 0, col.a)*/;
				col = ApplyNoise(col, i.uv);
				col = ApplyVignette(col, i.uv);

				return col;
			}
			ENDCG
		}
	}
}