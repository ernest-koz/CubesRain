using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : PoolableObject
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initialSize = 20;
    [SerializeField] private int _maxSize = 1000;

    private Queue<T> _available = new Queue<T>();
    private HashSet<T> _active = new HashSet<T>();
    private bool _initialized;

    public event Action<T> Returned;

    public int CreatedCount { get; private set; }
    public int ActiveCount => _active.Count;

    private void Awake()
    {
        if (_initialized)
            return;

        if (_prefab == null)
        {
            Debug.LogError($"Prefab not assigned. Assign a prefab in the inspector.", gameObject);
            return;
        }

        int size = Mathf.Min(_initialSize, _maxSize);
        for (int i = 0; i < size; i++)
            CreateNewObject();

        _initialized = true;
    }

    private void OnDestroy()
    {
        foreach (T poolable in _active)
            CleanupObject(poolable);

        foreach (T poolable in _available)
            CleanupObject(poolable);

        _active.Clear();
        _available.Clear();
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        if (_available.Count == 0 && _active.Count < _maxSize)
            CreateNewObject();

        if (_available.Count == 0)
        {
            Debug.LogWarning($"Pool exhausted. Max size ({_maxSize}) reached.", gameObject);
            return null;
        }

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
            return;

        poolable.ResetState();
        poolable.gameObject.SetActive(false);
        poolable.transform.SetParent(transform);
        _available.Enqueue(poolable);

        Returned?.Invoke(poolable);
    }

    private void CreateNewObject()
    {
        T poolable = Instantiate(_prefab, transform);
        poolable.ReturnRequested += OnReturnRequested;
        poolable.gameObject.SetActive(false);
        _available.Enqueue(poolable);
        CreatedCount++;
    }

    private void OnReturnRequested(PoolableObject poolable)
    {
        Return((T)poolable);
    }

    private void CleanupObject(T poolable)
    {
        if (poolable == null)
            return;

        if (Application.isPlaying == false)
            return;

        try
        {
            poolable.ReturnRequested -= OnReturnRequested;
        }
        catch (MissingReferenceException)
        {
        }

        if (poolable != null && poolable.gameObject != null)
            Destroy(poolable.gameObject);
    }
}
