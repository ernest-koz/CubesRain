using UnityEngine;
using UnityEngine.UI;

public class SpawnerStatsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CubeSpawner _cubeSpawner;
    [SerializeField] private BombSpawner _bombSpawner;

    private Text _cubeStats;
    private Text _bombStats;

    private void Awake()
    {
        Canvas canvas = CreateCanvas();
        _cubeStats = CreateText(canvas, "Cube Stats", new Vector2(20f, -20f));
        _bombStats = CreateText(canvas, "Bomb Stats", new Vector2(20f, -64f));
    }

    private void Update()
    {
        _cubeStats.text = Format("Кубы", _cubeSpawner);
        _bombStats.text = Format("Бомбы", _bombSpawner);
    }

    private string Format<T>(string label, Spawner<T> spawner) where T : PoolableObject
    {
        if (spawner == null || spawner.SourcePool == null)
            return $"{label}: нет данных";

        Pool<T> pool = spawner.SourcePool;

        return $"{label} — заспавнено: {spawner.TotalSpawned} | создано: {pool.CreatedCount} | активно: {pool.ActiveCount}";
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("StatsCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        return canvas;
    }

    private Text CreateText(Canvas canvas, string name, Vector2 anchoredPosition)
    {
        GameObject textObject = new GameObject(name, typeof(Text));
        textObject.transform.SetParent(canvas.transform, false);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 30;
        text.color = Color.white;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(1100f, 40f);

        return text;
    }
}
