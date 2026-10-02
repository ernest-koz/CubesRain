using UnityEngine;

public class BombSpawner : Spawner<Bomb>
{
    protected override bool ShouldSpawnContinuously => false;

    public void SpawnBomb(Vector3 position)
    {
        SpawnAt(position, Random.rotation);
    }
}
