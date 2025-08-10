using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCutsceneTrigger : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator; 
    public string animationTriggerName = "canTrigger";

    [Header("Toggle Movement Off")]
    public GameObject targetObject;
    private InputHandler inputHandlerScript;

    [Header("Moving The Player")]
    public GameObject playerObject;
    public Transform endLocation;
    public float moveSpeed = 2f; 
    public float stopDistance = 0.05f; 

    private bool isMoving = false;

    [Header("Animate Player")]
    public Animator playerAnimator;


    void Start()
    {
        inputHandlerScript = targetObject.GetComponent<InputHandler>();
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
        inputHandlerScript.enabled = !inputHandlerScript.enabled;
    }

    private void StartMovingPlayer()
    {
        isMoving = true;
        StartCoroutine(MovePlayerToEndLocation());
    }

    private IEnumerator MovePlayerToEndLocation()
    {
        if (inputHandlerScript != null)
            inputHandlerScript.enabled = false;

        if(playerAnimator != null)
            playerAnimator.SetBool("isRunning", true);

        while (Vector3.Distance(playerObject.transform.position, endLocation.position) > stopDistance)
        {
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                endLocation.position,
                moveSpeed * Time.deltaTime
            );

            yield return null; 
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isCutscene", true);
        }
            
        isMoving = false;
    }

    public void letPlayerMove()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isCutscene", false);

        }

        if (inputHandlerScript != null)
        {
            inputHandlerScript.enabled = true;
        }
    }
}
