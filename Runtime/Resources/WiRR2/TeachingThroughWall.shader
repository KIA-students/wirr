Shader "WiRR/TeachingThroughWall"
{
    Properties { _Color ("Kolor", Color) = (1,0,0,1) }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Attributes { float4 vertex : POSITION; };
            struct Varyings { float4 position : SV_POSITION; };
            float4 _Color;
            Varyings vert(Attributes input) { Varyings output; output.position = UnityObjectToClipPos(input.vertex); return output; }
            float4 frag(Varyings input) : SV_Target { return _Color; }
            ENDHLSL
        }
    }
}
