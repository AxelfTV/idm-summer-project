using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayTestEndScreen : MonoBehaviour
{
    [SerializeField] TMP_Text speedrunText;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;

        if (SpeedrunTimer.instance != null) 
        {
            float speedRunTime = SpeedrunTimer.instance.speedrunTime;
            int minutes = (int)(speedRunTime / 60);
            float seconds = speedRunTime % 60;
            speedrunText.text = "You completed the demo in:\n" + minutes.ToString() + "m " + seconds.ToString("n2") + "s";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.M)) SceneManager.LoadScene(0);
    }
}
