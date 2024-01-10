Shader "Anchor/Table" {
    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass {
            ZWrite Off
            ZTest On
            ColorMask 0
            Stencil {
                Ref 100
                Comp Always
                Pass Replace
            }
        }
    }
}