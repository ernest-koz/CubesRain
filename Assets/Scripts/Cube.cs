using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class Cube : PoolableObject
{
    private const float FallbackLifetimeMultiplier = 2f;

    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    [Header("Appearance")]
    [SerializeField] private Color _color = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float _minLifetime = 2f;
    [SerializeField] private float _maxLifetime = 5f;

    private MaterialPropertyBlock _propertyBlock;
    private WaitForSeconds _fallbackReturnWait;
    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private bool _hasTouchedPlatform;
    private Coroutine _fallbackReturnCoroutine;
    private Coroutine _returnToPoolCoroutine;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _propertyBlock = new MaterialPropertyBlock();
        _fallbackReturnWait = new WaitForSeconds(_maxLifetime * FallbackLifetimeMultiplier);
    }

    private void OnEnable()
    {
        _hasTouchedPlatform = false;

        ApplyColor(_color);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _fallbackReturnCoroutine = StartCoroutine(ReturnAfterDelay(_fallbackReturnWait));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouchedPlatform)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<Platform>(out _) == false)
        {
            return;
        }

        _hasTouchedPlatform = true;
        StopReturnRoutine(ref _fallbackReturnCoroutine);

        float lifetime = Random.Range(_minLifetime, _maxLifetime);

        ApplyColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));
        _returnToPoolCoroutine = StartCoroutine(ReturnAfterDelay(new WaitForSeconds(lifetime)));
    }

    private void OnValidate()
    {
        if (_minLifetime > _maxLifetime)
        {
            _maxLifetime = _minLifetime;
        }
    }

    public override void ResetState()
    {
        StopReturnRoutine(ref _fallbackReturnCoroutine);
        StopReturnRoutine(ref _returnToPoolCoroutine);
    }

    private void StopReturnRoutine(ref Coroutine returnRoutine)
    {
        if (returnRoutine == null)
        {
            return;
        }

        StopCoroutine(returnRoutine);
        returnRoutine = null;
    }

    private void ApplyColor(Color color)
    {
        _propertyBlock.SetColor(ColorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private IEnumerator ReturnAfterDelay(WaitForSeconds wait)
    {
        yield return wait;

        RaiseReturnRequested();
    }
}
