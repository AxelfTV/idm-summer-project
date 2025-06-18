using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHoldable : MonoBehaviour, IHoldable
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>().Grab(this);
        }
    }
    public bool Grab()
    {
        return true;
    }
    public void Throw()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
