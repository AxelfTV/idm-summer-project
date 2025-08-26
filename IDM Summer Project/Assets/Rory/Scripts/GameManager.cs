using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private PlayerInput controls;
    public Checkpoint currentCheckPoint = null;
    public EventReference polaroidSound;
    [SerializeField] Animator fade;
    [SerializeField] GameObject polaroidUI;
    GameObject player;

    bool polaroidCollected;
    bool jumpPressed;

    private void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Jump.performed += ctx =>
        {
            jumpPressed = true;
        };
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        player = GameObject.FindGameObjectWithTag("Player");
        polaroidUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (jumpPressed && polaroidCollected) StartCoroutine(EndLevelSequence());

        jumpPressed = false;
    }
    public void OnPolaroid()
    {
        StartCoroutine(PolaroidSequence());
    }
    public void LevelRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnPlayerDeath()
    {
        StartCoroutine(PlayerDeathSequence()); 
    }
    IEnumerator PlayerDeathSequence()
    {
        if (fade != null) fade.Play("Fade Out Level");//Fade out
        yield return new WaitForSeconds(1);
        if (currentCheckPoint != null)
        {
            player.transform.position = currentCheckPoint.transform.position;
            CameraTrigger.ResetAllCams();
            currentCheckPoint.camTrig.SwapCams();
            if (fade != null) fade.Play("Fade In Level");//Fade in
        }
        else
        {
            LevelRestart();
        }
    }
    IEnumerator PolaroidSequence()
    {
        player.GetComponent<InputHandler>().enabled = false;
        //fade in polaroid Ui;
        yield return new WaitForSeconds(0.5f);
        polaroidUI.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        RuntimeManager.PlayOneShot(polaroidSound);
        yield return new WaitForSeconds(1);
        polaroidCollected = true;
    }
    IEnumerator EndLevelSequence()
    {
        if (fade != null) fade.Play("Fade Out Level");//Fade out
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
