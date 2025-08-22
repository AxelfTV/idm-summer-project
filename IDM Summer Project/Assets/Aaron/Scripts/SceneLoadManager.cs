using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private PlayerInput controls;

    bool jumpPressed;
    private void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Jump.performed += ctx =>
        {
            jumpPressed = true;
        };
    }
    private void Start()
    {
        Cursor.visible = true;
    }
    private void Update()
    {
        if (jumpPressed) NextScene();
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
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
