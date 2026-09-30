using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Blast")]
    [SerializeField] private float _force = 15f;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _upwardsModifier = 0.3f;
    [SerializeField] private LayerMask _scatteringLayers = ~0;

    [Header("Effect")]
    [SerializeField] private ParticleSystem _explosionEffectPrefab;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public void Explode(Vector3 center)
    {
        ApplyForces(center);
        PlayEffectAt(center);
    }

    private void ApplyForces(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, _radius, _scatteringLayers);

        foreach (Collider hit in hits)
        {
            Rigidbody target = hit.attachedRigidbody;

            if (target == null)
            {
                continue;
            }

            if (target.isKinematic)
            {
                continue;
            }

            target.AddExplosionForce(_force, center, _radius, _upwardsModifier, ForceMode.Impulse);
        }
    }

    private void PlayEffectAt(Vector3 position)
    {
        if (_explosionEffectPrefab == null)
        {
            return;
        }

        ParticleSystem effect = Instantiate(_explosionEffectPrefab, position, Quaternion.identity);
        ParticleSystem.MainModule main = effect.main;
        float totalDuration = main.duration + main.startLifetime.constantMax;

        effect.Play();
        Destroy(effect.gameObject, totalDuration);
    }
}
