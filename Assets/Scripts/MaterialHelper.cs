using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialHelper
{
    public const int OpaqueMode = 0;
    public const int FadeMode = 2;

    public static void SetColor(Material material, Color color)
    {
        if (material != null)
            material.color = color;
    }

    public static void SetRenderMode(Material material, int mode)
    {
        if (material == null)
            return;

        if (mode == FadeMode)
        {
            SetMode(material, FadeMode, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha,
                zWrite: 0, queue: (int)RenderQueue.Transparent, alphaBlendKeyword: true);
        }
        else
        {
            SetMode(material, OpaqueMode, BlendMode.One, BlendMode.Zero,
                zWrite: 1, queue: -1, alphaBlendKeyword: false);
        }
    }

    private static void SetMode(Material material, int mode, BlendMode srcBlend, BlendMode dstBlend,
        int zWrite, int queue, bool alphaBlendKeyword)
    {
        material.SetFloat("_Mode", mode);
        material.SetInt("_SrcBlend", (int)srcBlend);
        material.SetInt("_DstBlend", (int)dstBlend);
        material.SetInt("_ZWrite", zWrite);

        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        if (alphaBlendKeyword)
            material.EnableKeyword("_ALPHABLEND_ON");
        else
            material.DisableKeyword("_ALPHABLEND_ON");

        material.renderQueue = queue;
    }
}
