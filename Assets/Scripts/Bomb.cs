using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer), typeof(Explosion))]
public class Bomb : PoolableObject
{
    private static readonly Color AppearanceColor = new Color(0f, 0f, 0f, 1f);

    [Header("Detonation")]
    [SerializeField] private float _minDetonationDelay = 2f;
    [SerializeField] private float _maxDetonationDelay = 5f;

    private Material _material;
    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private Explosion _explosion;
    private Coroutine _detonationCoroutine;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _explosion = GetComponent<Explosion>();
        _material = _renderer.material;

        MaterialTransparency.SetOpaqueMode(_material);
        MaterialTransparency.SetColor(_material, AppearanceColor);
    }

    private void OnEnable()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        MaterialTransparency.SetOpaqueMode(_material);
        MaterialTransparency.SetColor(_material, AppearanceColor);

        float detonationDelay = Random.Range(_minDetonationDelay, _maxDetonationDelay);
        _detonationCoroutine = StartCoroutine(DetonateAfterDelay(detonationDelay));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_material == null)
        {
            return;
        }

        Destroy(_material);
    }

    private void OnValidate()
    {
        if (_minDetonationDelay > _maxDetonationDelay)
        {
            _maxDetonationDelay = _minDetonationDelay;
        }
    }

    public override void ResetState()
    {
        if (_detonationCoroutine != null)
        {
            StopCoroutine(_detonationCoroutine);
            _detonationCoroutine = null;
        }
    }

    private static Color GetAppearanceColor(float alpha)
    {
        Color color = AppearanceColor;
        color.a = Mathf.Clamp01(alpha);
        return color;
    }

    private IEnumerator DetonateAfterDelay(float delay)
    {
        MaterialTransparency.SetFadeMode(_material);

        for (float elapsed = 0f; elapsed < delay; elapsed += Time.deltaTime)
        {
            MaterialTransparency.SetColor(_material, GetAppearanceColor(1f - elapsed / delay));
            yield return null;
        }

        MaterialTransparency.SetColor(_material, GetAppearanceColor(0f));
        _explosion.Explode(transform.position);
        RaiseReturnRequested();
    }
}
