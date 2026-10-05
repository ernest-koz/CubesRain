using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlatformDetector), typeof(CubeAppearance), typeof(LifetimeTimer))]
public class Cube : PoolableObject
{
    private const float FallbackLifetimeMultiplier = 2f;

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float _minLifetime = 2f;
    [SerializeField] private float _maxLifetime = 5f;

    private Rigidbody _rigidbody;
    private PlatformDetector _detector;
    private CubeAppearance _appearance;
    private LifetimeTimer _lifetime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _detector = GetComponent<PlatformDetector>();
        _appearance = GetComponent<CubeAppearance>();
        _lifetime = GetComponent<LifetimeTimer>();
    }

    private void OnEnable()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _detector.ResetState();

        _detector.PlatformTouched += OnPlatformTouched;
        _lifetime.Elapsed += OnLifetimeElapsed;

        _lifetime.Run(_maxLifetime * FallbackLifetimeMultiplier);
    }

    private void OnDisable()
    {
        _detector.PlatformTouched -= OnPlatformTouched;
        _lifetime.Elapsed -= OnLifetimeElapsed;

        _lifetime.Stop();
    }

    private void OnValidate()
    {
        if (_minLifetime > _maxLifetime)
        {
            _maxLifetime = _minLifetime;
        }
    }

    private void OnPlatformTouched()
    {
        _appearance.Apply(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));
        _lifetime.Run(Random.Range(_minLifetime, _maxLifetime));
    }

    private void OnLifetimeElapsed()
    {
        RaiseReturnRequested();
    }
}
