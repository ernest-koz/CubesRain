using UnityEngine;
using UnityEngine.UI;

public abstract class SpawnerStatsDisplay<T> : MonoBehaviour where T : PoolableObject
{
    [SerializeField] private Spawner<T> _spawner;
    [SerializeField] private Text _text;

    protected abstract string Label { get; }

    private void OnEnable()
    {
        _spawner.Spawned += Render;
        _spawner.SourcePool.Returned += OnReturned;
    }

    private void Start()
    {
        Render();
    }

    private void OnDisable()
    {
        _spawner.Spawned -= Render;
        _spawner.SourcePool.Returned -= OnReturned;
    }

    private void OnValidate()
    {
        if (_spawner == null)
        {
            Debug.LogError($"Spawner is not assigned on {gameObject.name}.", gameObject);
        }

        if (_text == null)
        {
            Debug.LogError($"Text is not assigned on {gameObject.name}.", gameObject);
        }
    }

    private void OnReturned(T poolable)
    {
        Render();
    }

    private void Render()
    {
        Pool<T> pool = _spawner.SourcePool;

        _text.text = $"{Label} — заспавнено: {_spawner.TotalSpawned} | создано: {pool.CreatedCount} | активно: {pool.ActiveCount}";
    }
}
