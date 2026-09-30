using System;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public event Action<PoolableObject> ReturnRequested;

    protected void RaiseReturnRequested()
    {
        ReturnRequested?.Invoke(this);
    }

    public virtual void ResetState() { }

    protected virtual void OnDestroy()
    {
        ReturnRequested = null;
    }
}
