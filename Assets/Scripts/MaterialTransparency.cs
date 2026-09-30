using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialTransparency
{
    public const int OpaqueMode = 0;
    public const int FadeMode = 2;

    public static void SetFadeMode(Material material)
    {
        SetMode(material, FadeMode, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha, zWrite: 0, fade: true);
    }

    public static void SetOpaqueMode(Material material)
    {
        SetMode(material, OpaqueMode, BlendMode.One, BlendMode.Zero, zWrite: 1, fade: false);
    }

    public static void SetColor(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        material.color = color;
    }

    private static void SetMode(Material material, int mode, BlendMode sourceBlend, BlendMode destinationBlend, int zWrite, bool fade)
    {
        if (material == null)
        {
            return;
        }

        material.SetFloat("_Mode", mode);
        material.SetInt("_SrcBlend", (int)sourceBlend);
        material.SetInt("_DstBlend", (int)destinationBlend);
        material.SetInt("_ZWrite", zWrite);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        if (fade)
        {
            material.EnableKeyword("_ALPHABLEND_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        else
        {
            material.DisableKeyword("_ALPHABLEND_ON");
            material.renderQueue = -1;
        }
    }
}