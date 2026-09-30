using System;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public event Action<PoolableObject> ReturnRequested;

    protected virtual void OnDestroy()
    {
        ReturnRequested = null;
    }

    public virtual void ResetState()
    {
    }

    protected void RaiseReturnRequested()
    {
        ReturnRequested?.Invoke(this);
    }
}
