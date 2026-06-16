using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private Cube _prefab;
    [SerializeField] private int _initialSize = 20;
    [SerializeField] private int _maxSize = 1000;

    private Queue<Cube> _available = new Queue<Cube>();
    private HashSet<Cube> _active = new HashSet<Cube>();
    private bool _initialized;

    private void Awake()
    {
        if (_initialized)
            return;

        if (_prefab == null)
        {
            Debug.LogError($"Prefab not assigned. Assign a Cube prefab in the inspector.", gameObject);
            return;
        }

        int size = Mathf.Min(_initialSize, _maxSize);
        for (int i = 0; i < size; i++)
            CreateNewCube();

        _initialized = true;
    }

    private void OnDestroy()
    {
        foreach (Cube cube in _active)
            CleanupCube(cube);

        foreach (Cube cube in _available)
            CleanupCube(cube);

        _active.Clear();
        _available.Clear();
    }

    public Cube Get(Vector3 position, Quaternion rotation)
    {
        if (_available.Count == 0 && _active.Count < _maxSize)
            CreateNewCube();

        if (_available.Count == 0)
        {
            Debug.LogWarning($"Pool exhausted. Max size ({_maxSize}) reached.", gameObject);
            return null;
        }

        Cube cube = _available.Dequeue();
        Transform t = cube.transform;
        t.SetParent(null);
        t.SetPositionAndRotation(position, rotation);
        cube.gameObject.SetActive(true);
        _active.Add(cube);
        return cube;
    }

    public void Return(Cube cube)
    {
        if (_active.Remove(cube) == false)
            return;

        cube.ResetState();
        cube.gameObject.SetActive(false);
        cube.transform.SetParent(transform);
        _available.Enqueue(cube);
    }

    private void CreateNewCube()
    {
        Cube cube = Instantiate(_prefab, transform);
        cube.ReturnRequested += OnCubeReturnRequested;
        cube.gameObject.SetActive(false);
        _available.Enqueue(cube);
    }

    private void OnCubeReturnRequested(Cube cube)
    {
        Return(cube);
    }

    private void CleanupCube(Cube cube)
    {
        if (cube == null)
            return;

        if (Application.isPlaying == false)
            return;

        try
        {
            cube.ReturnRequested -= OnCubeReturnRequested;
        }
        catch (MissingReferenceException)
        {
        }

        if (cube != null && cube.gameObject != null)
            Destroy(cube.gameObject);
    }
}
