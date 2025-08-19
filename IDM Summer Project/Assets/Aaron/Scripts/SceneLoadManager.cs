using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }
    public void NextScene()
    {
        if(SpeedrunTimer.instance != null) SpeedrunTimer.instance.StartTimer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
