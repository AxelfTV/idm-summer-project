using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    public bool Grab();
    public void Throw();
    public GameObject GetGameObject();
}
