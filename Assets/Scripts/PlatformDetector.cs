using System;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    private bool _hasTouched;

    public event Action PlatformTouched;

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouched)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<Platform>(out _) == false)
        {
            return;
        }

        _hasTouched = true;
        PlatformTouched?.Invoke();
    }

    public void ResetState()
    {
        _hasTouched = false;
    }
}
