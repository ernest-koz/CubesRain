using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class Cube : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color _color = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Lifetime")]
    [SerializeField] private float _minLifetime = 2f;
    [SerializeField] private float _maxLifetime = 5f;

    public event Action<Cube> ReturnRequested;

    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private MaterialPropertyBlock _propertyBlock;
    private bool _hasTouchedPlatform;
    private Coroutine _returnCoroutine;

    private void Awake()
    {
        if (TryGetComponent(out _renderer) == false)
            Debug.LogError($"Renderer component missing on {name}.", gameObject);

        if (TryGetComponent(out _rigidbody) == false)
            Debug.LogError($"Rigidbody component missing on {name}.", gameObject);

        _propertyBlock = new MaterialPropertyBlock();

        if (_minLifetime > _maxLifetime)
            _maxLifetime = _minLifetime;
    }

    private void OnEnable()
    {
        if (_renderer == null || _rigidbody == null)
            return;

        _hasTouchedPlatform = false;

        ApplyColor(_color);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        StartFallbackReturn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouchedPlatform)
            return;

        if (collision.gameObject.TryGetComponent<Platform>(out _) == false)
            return;

        _hasTouchedPlatform = true;

        ApplyColor(UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));

        float lifetime = UnityEngine.Random.Range(_minLifetime, _maxLifetime);
        StartCoroutine(ReturnToPoolAfterDelay(lifetime));
    }

    private void OnDestroy()
    {
        ReturnRequested = null;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        _returnCoroutine = null;
    }

    private void StartFallbackReturn()
    {
        float fallbackDelay = _maxLifetime * 2f;
        _returnCoroutine = StartCoroutine(FallbackReturnAfterDelay(fallbackDelay));
    }

    private IEnumerator FallbackReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_hasTouchedPlatform == false)
            ReturnRequested?.Invoke(this);
    }

    private void ApplyColor(Color color)
    {
        if (_renderer == null || _propertyBlock == null)
            return;

        _propertyBlock.SetColor(ColorHelper.ColorProperty, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ReturnRequested?.Invoke(this);
    }
}
