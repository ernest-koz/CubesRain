using UnityEngine;

public class CubeSpawner : Spawner<Cube>
{
    [Header("References")]
    [SerializeField] private BombSpawner _bombSpawner;

    private void OnEnable()
    {
        SourcePool.Returned += OnCubeReturned;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SourcePool.Returned -= OnCubeReturned;
    }

    private void OnValidate()
    {
        if (_bombSpawner == null)
        {
            Debug.LogError($"BombSpawner is not assigned on {gameObject.name}.", gameObject);
        }
    }

    private void OnCubeReturned(Cube cube)
    {
        _bombSpawner.SpawnBomb(cube.transform.position);
    }
}
