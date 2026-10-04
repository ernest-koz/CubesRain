using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CubeAppearance : MonoBehaviour
{
    private static readonly int s_colorProperty = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void Apply(Color color)
    {
        _propertyBlock.SetColor(s_colorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
