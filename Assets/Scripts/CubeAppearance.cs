using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class CubeAppearance : MonoBehaviour
{
    private static readonly int s_colorProperty = Shader.PropertyToID("_Color");

    [Header("Appearance")]
    [SerializeField] private Color _color = new Color(0.7f, 0.7f, 0.7f, 1f);

    private MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        Apply(_color);
    }

    public void Apply(Color color)
    {
        _propertyBlock.SetColor(s_colorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
