using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner<T> : MonoBehaviour where T : PoolableObject
{
    [Header("Spawn Area")]
    [SerializeField] private float _spawnRangeX = 8f;
    [SerializeField] private float _spawnRangeZ = 8f;
    [SerializeField] private float _spawnHeight = 15f;

    [Header("Spawn Rate")]
    [SerializeField, Min(0.1f)] private float _spawnInterval = 0.5f;
    [SerializeField, Min(1)] private int _objectsPerBatch = 3;

    [Header("Limits")]
    [SerializeField, Min(1)] private int _maxTotalSpawned = 1000;

    [Header("References")]
    [SerializeField] private Pool<T> _pool;

    private Coroutine _spawnCoroutine;
    private WaitForSeconds _spawnWait;

    public event Action Spawned;

    public int TotalSpawned { get; private set; }

    public Pool<T> SourcePool => _pool;

    protected virtual bool ShouldSpawnContinuously => true;

    private void Awake()
    {
        _spawnWait = new WaitForSeconds(_spawnInterval);
    }

    protected virtual void OnEnable()
    {
        if (ShouldSpawnContinuously == false)
        {
            return;
        }

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    protected virtual void OnDisable()
    {
        if (_spawnCoroutine == null)
        {
            return;
        }

        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    protected virtual void OnValidate()
    {
        if (_pool == null)
        {
            Debug.LogError($"Pool is not assigned on {gameObject.name}.", gameObject);
        }
    }

    protected void SpawnAt(Vector3 position, Quaternion rotation)
    {
        if (TotalSpawned >= _maxTotalSpawned)
        {
            return;
        }

        T spawned = _pool.Get(position, rotation);

        if (spawned == null)
        {
            return;
        }

        TotalSpawned++;
        Spawned?.Invoke();
    }

    private Vector3 GetRandomSpawnPosition()
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
            {
                SpawnAt(GetRandomSpawnPosition(), Random.rotation);
            }

            yield return _spawnWait;
        }
    }
}
