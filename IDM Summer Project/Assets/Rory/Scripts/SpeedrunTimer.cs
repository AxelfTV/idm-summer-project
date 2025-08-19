using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    public float speedrunTime;

    public static SpeedrunTimer instance;
    bool active;
    public void StartTimer()
    {
        active = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(active) speedrunTime += Time.deltaTime;
    }
}
