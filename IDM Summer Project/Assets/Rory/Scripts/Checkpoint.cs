using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool active = false;
    GameManager gameManager;
    [SerializeField] public CameraTrigger camTrig;
    [SerializeField] GameObject indicator;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if(gameManager == null) Destroy(gameObject);
        indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !active)
        {
            active = true;
            gameManager.currentCheckPoint = this;
        }
    }
}
