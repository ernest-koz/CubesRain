using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _upwardsModifier = 0.3f;

    public void Explode(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, _radius);

        foreach (Collider hit in hits)
        {
            Rigidbody target = hit.attachedRigidbody;

            if (target != null && target.isKinematic == false)
                target.AddExplosionForce(_force, center, _radius, _upwardsModifier, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
