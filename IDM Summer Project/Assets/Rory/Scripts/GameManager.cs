using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Checkpoint currentCheckPoint = null;
    [SerializeField] Animator fade;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
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
        if (fade != null) fade.Play("Fade Out Level");//Fade out
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
