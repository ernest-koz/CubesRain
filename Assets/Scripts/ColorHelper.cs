using UnityEngine;

/// <summary>
/// Статический хелпер для работы с цветом через MaterialPropertyBlock.
/// Используется для одноразовой установки цвета (например, при Procedural-создании платформ).
/// Для высокочастотных вызовов (пул объектов) рекомендуется кэшировать MaterialPropertyBlock.
/// </summary>
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
