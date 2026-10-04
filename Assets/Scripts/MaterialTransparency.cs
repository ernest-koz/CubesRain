using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialTransparency
{
    public const int OpaqueMode = 0;
    public const int FadeMode = 2;

    private const string AlphaTestKeyword = "_ALPHATEST_ON";
    private const string AlphaPremultiplyKeyword = "_ALPHAPREMULTIPLY_ON";
    private const string AlphaBlendKeyword = "_ALPHABLEND_ON";
    private const int ShaderDefaultQueue = -1;

    private static readonly int s_modeProperty = Shader.PropertyToID("_Mode");
    private static readonly int s_srcBlendProperty = Shader.PropertyToID("_SrcBlend");
    private static readonly int s_dstBlendProperty = Shader.PropertyToID("_DstBlend");
    private static readonly int s_zwriteProperty = Shader.PropertyToID("_ZWrite");

    public static void SetFadeMode(Material material)
    {
        SetMode(material, FadeMode, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
    }

    public static void SetOpaqueMode(Material material)
    {
        SetMode(material, OpaqueMode, BlendMode.One, BlendMode.Zero);
    }

    public static void SetColor(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        material.color = color;
    }

    private static void SetMode(Material material, int mode, BlendMode sourceBlend, BlendMode destinationBlend)
    {
        bool isFade = mode == FadeMode;

        material.SetFloat(s_modeProperty, mode);
        material.SetFloat(s_srcBlendProperty, (float)sourceBlend);
        material.SetFloat(s_dstBlendProperty, (float)destinationBlend);
        material.SetFloat(s_zwriteProperty, isFade ? 0f : 1f);
        material.DisableKeyword(AlphaTestKeyword);
        material.DisableKeyword(AlphaPremultiplyKeyword);

        if (isFade)
        {
            material.EnableKeyword(AlphaBlendKeyword);
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            material.DisableKeyword(AlphaBlendKeyword);
            material.renderQueue = ShaderDefaultQueue;
        }
    }
}
