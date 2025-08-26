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
    public string bellAnimationName = "Bell";

    [Header("Woof")]
    public GameObject Woof;
    public GameObject WoofMesh;
    public GameObject WoofClone;

    [Header("Subtitles")]
    public GameObject toggleSubtitle;
    public GameObject bellSub;
    public GameObject baaSub;

    
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
        {
            inputHandlerScript.enabled = false;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("cutsceneRun", true);
            playerAnimator.SetBool("isCutscene", true);
        }

        while (Vector3.Distance(playerObject.transform.position, endLocation.position) > stopDistance)
        {
            // Rotate player to face Woof
            if (Woof != null)
            {
                Vector3 lookDirection = (Woof.transform.position - playerObject.transform.position).normalized;
                lookDirection.y = 0f; // Only rotate on the Y axis
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    playerObject.transform.rotation = Quaternion.Slerp(
                        playerObject.transform.rotation,
                        targetRotation,
                        Time.deltaTime * 5f // rotation speed
                    );
                }
            }

            // Move player towards end location
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                endLocation.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("cutsceneRun", false);
            playerAnimator.SetBool("isCutscene", true);
            playerAnimator.Play(bellAnimationName);
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

    public void freeWolf()
    {
        WoofMesh.SetActive(true);
        WoofClone.SetActive(false);
        if (Woof != null)
        {
            BoxCollider[] colliders = Woof.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider col in colliders)
            {
                col.enabled = true;
            }
        }
    }

    public void lockWolf()
    {
        if (Woof != null)
        {
            BoxCollider[] colliders = Woof.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider col in colliders)
            {
                col.enabled = false;
            }
        }
    }

    public void noBellAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isBell", false);
        }
    }

    public void toggleSubs()
    {
        toggleSubtitle.SetActive(true);
    }

    public void toggleBell()
    {
        bellSub.SetActive(true);
    }

    public void toggleBaa()
    {
        baaSub.SetActive(true);
    }
}