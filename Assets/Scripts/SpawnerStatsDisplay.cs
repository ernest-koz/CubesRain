using UnityEngine;
using UnityEngine.UI;

public class SpawnerStatsDisplay : MonoBehaviour
{
    private const int TextFontSize = 30;
    private const int CanvasSortingOrder = 100;
    private const string CanvasName = "StatsCanvas";
    private const string CubeStatsName = "Cube Stats";
    private const string BombStatsName = "Bomb Stats";
    private const string LegacyFontResource = "LegacyRuntime.ttf";
    private const string CubeLabel = "Кубы";
    private const string BombLabel = "Бомбы";

    private static readonly Vector2 TopLeftAnchor = new Vector2(0f, 1f);
    private static readonly Vector2 TopLeftPivot = new Vector2(0f, 1f);
    private static readonly Vector2 CubeTextOffset = new Vector2(20f, -20f);
    private static readonly Vector2 BombTextOffset = new Vector2(20f, -64f);
    private static readonly Vector2 TextSize = new Vector2(1100f, 40f);

    [Header("References")]
    [SerializeField] private CubeSpawner _cubeSpawner;
    [SerializeField] private BombSpawner _bombSpawner;

    private Text _cubeText;
    private Text _bombText;

    private void Awake()
    {
        Canvas canvas = CreateCanvas();
        _cubeText = CreateText(canvas, CubeStatsName, CubeTextOffset);
        _bombText = CreateText(canvas, BombStatsName, BombTextOffset);
    }

    private void OnEnable()
    {
        _cubeSpawner.Spawned += Render;
        _cubeSpawner.SourcePool.Returned += OnCubeReturned;
        _bombSpawner.Spawned += Render;
        _bombSpawner.SourcePool.Returned += OnBombReturned;
    }

    private void Start()
    {
        Render();
    }

    private void OnDisable()
    {
        _cubeSpawner.Spawned -= Render;
        _cubeSpawner.SourcePool.Returned -= OnCubeReturned;
        _bombSpawner.Spawned -= Render;
        _bombSpawner.SourcePool.Returned -= OnBombReturned;
    }

    private void OnValidate()
    {
        if (_cubeSpawner == null)
        {
            Debug.LogError($"CubeSpawner is not assigned on {gameObject.name}.", gameObject);
        }

        if (_bombSpawner == null)
        {
            Debug.LogError($"BombSpawner is not assigned on {gameObject.name}.", gameObject);
        }
    }

    private void Render()
    {
        _cubeText.text = Format(CubeLabel, _cubeSpawner);
        _bombText.text = Format(BombLabel, _bombSpawner);
    }

    private void OnCubeReturned(Cube cube)
    {
        Render();
    }

    private void OnBombReturned(Bomb bomb)
    {
        Render();
    }

    private string Format<T>(string label, Spawner<T> spawner) where T : PoolableObject
    {
        Pool<T> pool = spawner.SourcePool;

        return $"{label} — заспавнено: {spawner.TotalSpawned} | создано: {pool.CreatedCount} | активно: {pool.ActiveCount}";
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(CanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = CanvasSortingOrder;
        return canvas;
    }

    private Text CreateText(Canvas canvas, string objectName, Vector2 anchoredPosition)
    {
        GameObject textObject = new GameObject(objectName, typeof(Text));
        textObject.transform.SetParent(canvas.transform, false);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>(LegacyFontResource);
        text.fontSize = TextFontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = TopLeftAnchor;
        textRect.anchorMax = TopLeftAnchor;
        textRect.pivot = TopLeftPivot;
        textRect.anchoredPosition = anchoredPosition;
        textRect.sizeDelta = TextSize;

        return text;
    }
}
