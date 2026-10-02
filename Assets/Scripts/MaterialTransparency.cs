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

    private static readonly int ModeProperty = Shader.PropertyToID("_Mode");
    private static readonly int SrcBlendProperty = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlendProperty = Shader.PropertyToID("_DstBlend");
    private static readonly int ZWriteProperty = Shader.PropertyToID("_ZWrite");

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

        material.SetFloat(ModeProperty, mode);
        material.SetFloat(SrcBlendProperty, (float)sourceBlend);
        material.SetFloat(DstBlendProperty, (float)destinationBlend);
        material.SetFloat(ZWriteProperty, isFade ? 0f : 1f);
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
