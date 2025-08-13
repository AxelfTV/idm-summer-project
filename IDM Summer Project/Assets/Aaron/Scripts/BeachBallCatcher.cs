using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeachBallCatcher : MonoBehaviour
{
    public TMP_Text scoreText;  
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);

            score++;
            scoreText.text = score.ToString();
        }
    }
}
