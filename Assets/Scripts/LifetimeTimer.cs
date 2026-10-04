using System;
using System.Collections;
using UnityEngine;

public class LifetimeTimer : MonoBehaviour
{
    private Coroutine _routine;

    public event Action Elapsed;

    public void Run(float lifetime)
    {
        Stop();
        _routine = StartCoroutine(RunRoutine(lifetime));
    }

    public void Stop()
    {
        if (_routine == null)
        {
            return;
        }

        StopCoroutine(_routine);
        _routine = null;
    }

    private IEnumerator RunRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        _routine = null;
        Elapsed?.Invoke();
    }
}
