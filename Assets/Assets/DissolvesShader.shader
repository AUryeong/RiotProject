Shader "Custom/DissolvesShader"
{
    Properties
    {
        _EC ("Color", Color) = (1,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex("_NoiseTex", 2D) = "white" {}
        _DissolvePower("_DissolvePower", Range(-0.05,1)) = 0
        _Depth("Depth", float) = 0.04
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
           
            #pragma surface surf Standard alpha:fade

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _NoiseTex;

             float _DissolvePower;
             float4 _EC;
             float _Depth;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_NoiseTex;
            };

        

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf (Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 n = tex2D (_NoiseTex, IN.uv_NoiseTex);
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

                o.Albedo = c.rgb;
                if(_DissolvePower == 0)
                    o.Emission = step(n.r, _DissolvePower) * _EC;
                else
                    o.Emission = step(n.r-_Depth, _DissolvePower) * _EC;
                // Metallic and smoothness come from slider variables
                o.Alpha = 1 - step(n.r, _DissolvePower);
            }
            ENDCG
        
        
    }
    FallBack "Diffuse"
}
