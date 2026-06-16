using UnityEngine;

public class PlatformRoot : MonoBehaviour
{
    [Header("Main Platform")]
    [SerializeField] private Vector3 _mainSize = new Vector3(20f, 1f, 20f);
    [SerializeField] private Color _mainColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("Tilted Platforms")]
    [SerializeField] private Vector3 _tiltedSize = new Vector3(5f, 0.5f, 5f);
    [SerializeField] private Color _tiltedColor = new Color(0.3f, 0.3f, 0.5f, 1f);
    [SerializeField] private float _tiltAngle = 35f;
    [SerializeField] private float _minHeight = 5f;
    [SerializeField] private float _maxHeight = 12f;
    [SerializeField] private int _platformCount = 6;

    private void Awake()
    {
        CreateMain();
        CreateTilted();
    }

    private void CreateMain()
    {
        GameObject platform = CreatePrimitiveWithPlatform("Main Platform", transform);
        platform.transform.position = Vector3.zero;
        platform.transform.localScale = _mainSize;
        SetRendererColor(platform, _mainColor);
    }

    private void CreateTilted()
    {
        for (int i = 0; i < _platformCount; i++)
        {
            float height = Random.Range(_minHeight, _maxHeight);

            Vector3 position = new Vector3(
                Random.Range(-_mainSize.x * 0.3f, _mainSize.x * 0.3f),
                height,
                Random.Range(-_mainSize.z * 0.3f, _mainSize.z * 0.3f));

            Vector3 euler = new Vector3(
                Random.value > 0.5f ? _tiltAngle : -_tiltAngle,
                Random.Range(0f, 360f),
                Random.value > 0.5f ? _tiltAngle : -_tiltAngle);

            GameObject platform = CreatePrimitiveWithPlatform($"Tilted Platform {i + 1}", transform);
            platform.transform.SetPositionAndRotation(position, Quaternion.Euler(euler));
            platform.transform.localScale = _tiltedSize;
            SetRendererColor(platform, _tiltedColor);
        }
    }

    private static GameObject CreatePrimitiveWithPlatform(string name, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.AddComponent<Platform>();
        return go;
    }

    private static void SetRendererColor(GameObject obj, Color color)
    {
        if (obj.TryGetComponent(out Renderer renderer))
            ColorHelper.SetRendererColor(renderer, color);
    }
}
