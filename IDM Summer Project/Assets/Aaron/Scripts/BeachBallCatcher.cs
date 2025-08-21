using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;

public class BeachBallCatcher : MonoBehaviour
{
    public TMP_Text scoreText;  
    private int score = 0;
    public EventReference celebrateSound;

    public GameObject confettiPrefab;   
    public Transform confettiSpawnPoint; 
    public float confettiLifetime = 3f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);

            score++;
            scoreText.text = score.ToString();
            RuntimeManager.PlayOneShot(celebrateSound);

            GameObject confetti = Instantiate(confettiPrefab, confettiSpawnPoint.position, confettiSpawnPoint.rotation);
            Destroy(confetti, confettiLifetime);
        }
    }
}
