using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockMechanism : MonoBehaviour
{
    [SerializeField] private MonoBehaviour unlockableObjectComponent;

    protected IUnlockableObject unlockableObject => (IUnlockableObject)unlockableObjectComponent;

    protected void Lock()
    {
        if (unlockableObject != null) unlockableObject.Lock();
    }
    protected void Unlock()
    {
        if(unlockableObject != null) unlockableObject.Unlock();
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
