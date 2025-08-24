using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    [SerializeField] GameObject image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        image.transform.rotation = image.transform.rotation * Quaternion.Euler(0, 60 * Time.deltaTime, 0);
    }
    void Pickup()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().OnPolaroid();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }
}
