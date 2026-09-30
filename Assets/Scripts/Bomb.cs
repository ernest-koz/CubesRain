using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Explosion))]
public class Bomb : PoolableObject
{
    [Header("Detonation")]
    [SerializeField] private float _minDetonationDelay = 2f;
    [SerializeField] private float _maxDetonationDelay = 5f;

    private static readonly Color AppearanceColor = new Color(0f, 0f, 0f, 1f);

    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private Explosion _explosion;
    private Material _material;

    private void Awake()
    {
        if (TryGetComponent(out _renderer) == false)
            Debug.LogError($"Renderer component missing on {name}.", gameObject);

        if (TryGetComponent(out _rigidbody) == false)
            Debug.LogError($"Rigidbody component missing on {name}.", gameObject);

        if (TryGetComponent(out _explosion) == false)
            Debug.LogError($"Explosion component missing on {name}.", gameObject);

        if (_renderer != null)
        {
            _material = _renderer.material;
            ApplyAppearance(MaterialHelper.OpaqueMode);
        }

        if (_minDetonationDelay > _maxDetonationDelay)
            _maxDetonationDelay = _minDetonationDelay;
    }

    private void OnEnable()
    {
        if (_rigidbody == null || _material == null)
            return;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        ApplyAppearance(MaterialHelper.OpaqueMode);

        float delay = Random.Range(_minDetonationDelay, _maxDetonationDelay);
        StartCoroutine(DetonateAfterDelay(delay));
    }

    public override void ResetState()
    {
        StopAllCoroutines();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_material != null)
            Destroy(_material);
    }

    private IEnumerator DetonateAfterDelay(float delay)
    {
        ApplyAppearance(MaterialHelper.FadeMode);

        for (float elapsed = 0f; elapsed < delay; elapsed += Time.deltaTime)
        {
            float alpha = Mathf.Clamp01(1f - elapsed / delay);
            Color color = AppearanceColor;
            color.a = alpha;
            MaterialHelper.SetColor(_material, color);
            yield return null;
        }

        if (_explosion != null)
            _explosion.Explode(transform.position);

        RaiseReturnRequested();
    }

    private void ApplyAppearance(int renderMode)
    {
        MaterialHelper.SetRenderMode(_material, renderMode);
        MaterialHelper.SetColor(_material, AppearanceColor);
    }
}
