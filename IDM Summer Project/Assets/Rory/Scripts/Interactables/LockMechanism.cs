using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockMechanism : MonoBehaviour
{
    [SerializeField] private MonoBehaviour unlockableObjectComponent;

    protected IUnlockableObject unlockableObject => (IUnlockableObject)unlockableObjectComponent;

    protected void Lock()
    {
        unlockableObject.Lock();
    }
    protected void Unlock()
    {
        unlockableObject.Unlock();
    }
    private void OnValidate()
    {
        if (unlockableObjectComponent != null && !(unlockableObjectComponent is IUnlockableObject))
        {
            Debug.LogWarning($"{unlockableObjectComponent.name} does not implement IUnlockableObject");
            unlockableObjectComponent = null;
        }
    }
}
