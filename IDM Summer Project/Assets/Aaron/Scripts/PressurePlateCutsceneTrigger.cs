using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateCutsceneTrigger : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string animationTriggerName = "canTrigger2";

    [Header("Toggle Movement Off")]
    public GameObject targetObject;
    private InputHandler inputHandlerScript;

    [Header("Toggle Camera")]
    public GameObject gameCamera;

    [Header("Subtitles")]
    public GameObject toggleSubtitle;

    // Start is called before the first frame update
    void Start()
    {
        inputHandlerScript = targetObject.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger(animationTriggerName);
        }
    }

    public void ToggleScript()
    {
        if (inputHandlerScript != null)
        {
            inputHandlerScript.enabled = false;
        }
    }

    public void letPlayerMove()
    {
        if (inputHandlerScript != null)
        {
            inputHandlerScript.enabled = true;
        }
    }

    public void toggleCameraOn()
    {
        gameCamera.SetActive(true);
    }

    public void toggleCameraOff()
    {
        gameCamera.SetActive(false);
    }

    public void toggleSub()
    {
        toggleSubtitle.SetActive(true);
    }

}
