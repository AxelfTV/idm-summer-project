using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseUI;

    PlayerInput controls;

    bool pausePressed;
    bool paused;

    private void Awake()
    {
        pauseUI.SetActive(false);
        controls = new PlayerInput();

        controls.Player.Pause.performed += ctx => pausePressed = true;
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void LateUpdate()
    {
        pausePressed = false;
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        if (pausePressed)
        {
            if (paused)
            {
                Time.timeScale = 1.0f;
                paused = false;

            }
            else
            {
                Time.timeScale = 0.0f;
                paused = true;
            }
            pauseUI.SetActive(paused);
        }
    }
}
