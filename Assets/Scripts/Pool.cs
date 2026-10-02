using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : PoolableObject
{
    [SerializeField] private T _prefab;
    [SerializeField, Min(0)] private int _initialSize = 20;
    [SerializeField, Min(1)] private int _maxSize = 1000;

    private readonly Queue<T> _available = new Queue<T>();
    private readonly HashSet<T> _active = new HashSet<T>();
    private bool _hasReportedExhaustion;

    public event Action<T> Returned;

    public int CreatedCount { get; private set; }

    public int ActiveCount => _active.Count;

    private void Awake()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            Grow();
        }
    }

    private void OnDestroy()
    {
        foreach (T poolable in _active)
        {
            CleanupObject(poolable);
        }

        foreach (T poolable in _available)
        {
            CleanupObject(poolable);
        }

        _active.Clear();
        _available.Clear();
    }

    private void OnValidate()
    {
        if (_prefab == null)
        {
            Debug.LogError($"Prefab is not assigned on {gameObject.name}.", gameObject);
        }

        if (_initialSize > _maxSize)
        {
            _initialSize = _maxSize;
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        if (_available.Count == 0)
        {
            Grow();
        }

        if (_available.Count == 0)
        {
            ReportExhaustion();
            return null;
        }

        _hasReportedExhaustion = false;

        T poolable = _available.Dequeue();
        Transform instanceTransform = poolable.transform;
        instanceTransform.SetParent(null);
        instanceTransform.SetPositionAndRotation(position, rotation);
        poolable.gameObject.SetActive(true);
        _active.Add(poolable);
        return poolable;
    }

    public void Return(T poolable)
    {
        if (_active.Remove(poolable) == false)
        {
            return;
        }

        poolable.ResetState();
        poolable.gameObject.SetActive(false);
        poolable.transform.SetParent(transform);
        _available.Enqueue(poolable);

        Returned?.Invoke(poolable);
    }

    private void Grow()
    {
        if (_active.Count + _available.Count >= _maxSize)
        {
            return;
        }

        T poolable = Instantiate(_prefab, transform);
        poolable.ReturnRequested += OnReturnRequested;
        poolable.gameObject.SetActive(false);
        _available.Enqueue(poolable);
        CreatedCount++;
    }

    private void ReportExhaustion()
    {
        if (_hasReportedExhaustion)
        {
            return;
        }

        _hasReportedExhaustion = true;
        Debug.LogWarning($"Pool exhausted. Maximum size ({_maxSize}) reached.", gameObject);
    }

    private void OnReturnRequested(PoolableObject poolable)
    {
        Return((T)poolable);
    }

    private void CleanupObject(T poolable)
    {
        if (poolable == null)
        {
            return;
        }

        poolable.ReturnRequested -= OnReturnRequested;

        if (Application.isPlaying == false)
        {
            return;
        }

        Destroy(poolable.gameObject);
    }
}
