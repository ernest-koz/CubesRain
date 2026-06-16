using UnityEngine;

internal static class ColorHelper
{
    internal static readonly int ColorProperty = Shader.PropertyToID("_Color");

    internal static void SetRendererColor(Renderer renderer, Color color)
    {
        if (renderer == null)
            return;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor(ColorProperty, color);
        renderer.SetPropertyBlock(block);
    }
}
