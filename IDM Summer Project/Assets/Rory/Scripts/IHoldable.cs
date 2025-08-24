using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    public bool Grab();
    public void Throw(Vector3 direction);
    public void Drop();
    public GameObject GetGameObject();
}
