using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class Cube : PoolableObject
{
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    [Header("Appearance")]
    [SerializeField] private Color _color = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Lifetime")]
    [SerializeField] private float _minLifetime = 2f;
    [SerializeField] private float _maxLifetime = 5f;

    private MaterialPropertyBlock _propertyBlock;
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
    }

    private void OnEnable()
    {
        _hasTouchedPlatform = false;

        ApplyColor(_color);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _fallbackReturnCoroutine = StartCoroutine(FallbackReturnAfterDelay(_maxLifetime * 2f));
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

        if (_fallbackReturnCoroutine != null)
        {
            StopCoroutine(_fallbackReturnCoroutine);
            _fallbackReturnCoroutine = null;
        }

        ApplyColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));
        _returnToPoolCoroutine = StartCoroutine(ReturnToPoolAfterDelay(Random.Range(_minLifetime, _maxLifetime)));
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
        if (_fallbackReturnCoroutine != null)
        {
            StopCoroutine(_fallbackReturnCoroutine);
            _fallbackReturnCoroutine = null;
        }

        if (_returnToPoolCoroutine != null)
        {
            StopCoroutine(_returnToPoolCoroutine);
            _returnToPoolCoroutine = null;
        }
    }

    private void ApplyColor(Color color)
    {
        _propertyBlock.SetColor(ColorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private IEnumerator FallbackReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        RaiseReturnRequested();
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        RaiseReturnRequested();
    }
}
