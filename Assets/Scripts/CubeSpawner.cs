using System.Collections;
using UnityEngine;

/// <summary>
/// Спавнит кубы в случайных позициях над платформой.
/// Ссылка на Pool задаётся в инспекторе.
/// </summary>
public class CubeSpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private float _spawnRangeX = 8f;
    [SerializeField] private float _spawnRangeZ = 8f;
    [SerializeField] private float _spawnHeight = 15f;

    [Header("Spawn Rate")]
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private int _cubesPerBatch = 3;

    [Header("Limits")]
    [SerializeField] private int _maxTotalSpawned = 1000;

    [Header("References")]
    [SerializeField] private Pool _pool;

    private int _totalSpawned;
    private Coroutine _spawnCoroutine;

    private void OnDisable()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void Start()
    {
        if (_pool == null)
        {
            Debug.LogError($"Pool not assigned in inspector. Assign Pool reference.", gameObject);
            return;
        }

        _cubesPerBatch = Mathf.Max(1, _cubesPerBatch);
        _spawnInterval = Mathf.Max(0.1f, _spawnInterval);
        _maxTotalSpawned = Mathf.Max(1, _maxTotalSpawned);

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (_totalSpawned < _maxTotalSpawned)
        {
            int remaining = _maxTotalSpawned - _totalSpawned;
            int batchSize = Mathf.Min(_cubesPerBatch, remaining);

            for (int i = 0; i < batchSize; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(-_spawnRangeX, _spawnRangeX),
                    _spawnHeight,
                    Random.Range(-_spawnRangeZ, _spawnRangeZ));

                Cube cube = _pool.Get(position, Random.rotation);

                // Pool.Get() может вернуть null, если пул исчерпан
                if (cube != null)
                    _totalSpawned++;
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }
}
