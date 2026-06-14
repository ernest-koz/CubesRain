using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Падающий куб с таймером жизни.
/// При первом касании платформы меняет цвет на случайный и запускает таймер возврата в пул.
/// Коммуникация с Pool осуществляется через событие <see cref="ReturnRequested"/> —
/// куб не хранит прямую ссылку на пул.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Renderer))]
public class Cube : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color _color = new Color(0.7f, 0.7f, 0.7f, 1f);

    [Header("Lifetime")]
    [SerializeField] private float _minLifetime = 2f;
    [SerializeField] private float _maxLifetime = 5f;

    /// <summary>
    /// Событие запроса на возврат в пул.
    /// Pool подписывается при создании куба — так Cube не хранит прямую ссылку на Pool.
    /// </summary>
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

        // При каждом появлении из пула куб красится в цвет из инспектора.
        // Цвет един для всех падающих кубов; задаётся в префабе через SerializeField.
        ApplyColor(_color);

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // Fallback: если куб никогда не коснётся платформы,
        // он всё равно вернётся в пул через _maxLifetime * 2 секунд.
        StartFallbackReturn();
    }

    private void OnDestroy()
    {
        // Очищаем подписки, чтобы избежать утечек при ручном уничтожении куба
        ReturnRequested = null;
    }

    /// <summary>
    /// Сбрасывает состояние куба при возврате в пул.
    /// </summary>
    public void ResetState()
    {
        StopAllCoroutines();
        _returnCoroutine = null;
    }

    /// <summary>
    /// Запускает fallback-корутину, которая вернёт куб в пул,
    /// если он так и не коснулся платформы.
    /// </summary>
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouchedPlatform)
            return;

        if (collision.gameObject.TryGetComponent<Platform>(out _) == false)
            return;

        _hasTouchedPlatform = true;

        // При первом касании платформы — случайный цвет и таймер возврата в пул
        ApplyColor(UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f));

        float lifetime = UnityEngine.Random.Range(_minLifetime, _maxLifetime);
        StartCoroutine(ReturnToPoolAfterDelay(lifetime));
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

        // Вместо прямого вызова pool.Return(this) — событие.
        // Пул подписан и обработает возврат.
        ReturnRequested?.Invoke(this);
    }
}
