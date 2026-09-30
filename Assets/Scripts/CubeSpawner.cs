using UnityEngine;

public class CubeSpawner : Spawner<Cube>
{
    [Header("References")]
    [SerializeField] private BombSpawner _bombSpawner;

    protected override void Start()
    {
        base.Start();

        if (_pool != null && _bombSpawner != null)
            _pool.Returned += OnCubeReturned;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_pool != null)
            _pool.Returned -= OnCubeReturned;
    }

    private void OnCubeReturned(Cube cube)
    {
        if (cube == null || _bombSpawner == null)
            return;

        _bombSpawner.SpawnBomb(cube.transform.position);
    }
}
