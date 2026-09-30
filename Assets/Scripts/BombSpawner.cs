using UnityEngine;

public class BombSpawner : Spawner<Bomb>
{
    protected override bool SpawnContinuously => false;

    public void SpawnBomb(Vector3 position)
    {
        SpawnAt(position, Random.rotation);
    }
}
