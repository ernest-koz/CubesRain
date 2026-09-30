using System.Collections;
using UnityEngine;

public abstract class Spawner<T> : MonoBehaviour where T : PoolableObject
{
    [Header("Spawn Area")]
    [SerializeField] protected float _spawnRangeX = 8f;
    [SerializeField] protected float _spawnRangeZ = 8f;
    [SerializeField] protected float _spawnHeight = 15f;

    [Header("Spawn Rate")]
    [SerializeField] protected float _spawnInterval = 0.5f;
    [SerializeField] protected int _objectsPerBatch = 3;

    [Header("Limits")]
    [SerializeField] protected int _maxTotalSpawned = 1000;

    [Header("References")]
    [SerializeField] protected Pool<T> _pool;

    private Coroutine _spawnCoroutine;
    private WaitForSeconds _spawnCachedWait;

    public int TotalSpawned { get; private set; }
    public Pool<T> SourcePool => _pool;

    protected virtual bool SpawnContinuously => true;

    protected virtual void Start()
    {
        if (_pool == null)
        {
            Debug.LogError($"Pool not assigned in inspector. Assign Pool reference.", gameObject);
            return;
        }

        _objectsPerBatch = Mathf.Max(1, _objectsPerBatch);
        _spawnInterval = Mathf.Max(0.1f, _spawnInterval);
        _maxTotalSpawned = Mathf.Max(1, _maxTotalSpawned);

        _spawnCachedWait = new WaitForSeconds(_spawnInterval);

        if (SpawnContinuously)
            _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    protected virtual void OnDisable()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    protected virtual T SpawnAt(Vector3 position, Quaternion rotation)
    {
        if (_pool == null || TotalSpawned >= _maxTotalSpawned)
            return null;

        T spawned = _pool.Get(position, rotation);

        if (spawned != null)
            TotalSpawned++;

        return spawned;
    }

    protected Vector3 GetRandomSpawnPosition()
    {
        return new Vector3(
            Random.Range(-_spawnRangeX, _spawnRangeX),
            _spawnHeight,
            Random.Range(-_spawnRangeZ, _spawnRangeZ));
    }

    private IEnumerator SpawnRoutine()
    {
        while (TotalSpawned < _maxTotalSpawned)
        {
            int remaining = _maxTotalSpawned - TotalSpawned;
            int batchSize = Mathf.Min(_objectsPerBatch, remaining);

            for (int i = 0; i < batchSize; i++)
                SpawnAt(GetRandomSpawnPosition(), Random.rotation);

            yield return _spawnCachedWait;
        }
    }
}
