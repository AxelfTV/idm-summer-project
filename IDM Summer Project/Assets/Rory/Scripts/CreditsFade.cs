using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsFade : MonoBehaviour
{
    [SerializeField] TMP_Text creditsText1;
    [SerializeField] TMP_Text creditsText2;
    [SerializeField] float timeUntilStart;
    [SerializeField] float lifespan;
    [SerializeField] float fadeTime;

    bool fadeIn;

    string[][] names = new string[][]
    {
        new string[] {"Voice of May - Asling Healy", "Project Manager & Narrative Lead - Eve Brady"},
        new string[] {"2D Artist & UI/UX Designer - Diana Petrova", "Technical & 3D Artist - Jiayin Wen"},
        new string[] {"Unity Developer & Game Designer - Rory Lyons", "Game Designer & Unity Developer - Ming Hei Chan"},
    };
    // Start is called before the first frame update
    void Start()
    {
        creditsText1.color = new Color(1,1,1,0);
        creditsText2.color = new Color(1, 1, 1, 0);
        StartCoroutine(Timeline());
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn && creditsText1.color.a < 1)
        {
            creditsText1.color += new Color(0, 0, 0, 1 * Time.deltaTime / fadeTime);
            creditsText2.color += new Color(0, 0, 0, 1 * Time.deltaTime / fadeTime);
        }
        else if (!fadeIn && creditsText1.color.a > 0) 
        {
            creditsText1.color -= new Color(0, 0, 0, 1 * Time.deltaTime / fadeTime);
            creditsText2.color -= new Color(0, 0, 0, 1 * Time.deltaTime / fadeTime);
        } 
    }
    IEnumerator Timeline()
    {
        yield return new WaitForSeconds(timeUntilStart);
        
        for (int i = 0;i < names.GetLength(0); i++)
        {
            creditsText1.text = names[i][0];
            creditsText2.text = names[i][1];
            fadeIn = true;
            yield return new WaitForSeconds(fadeTime);
            yield return new WaitForSeconds(lifespan);
            fadeIn = false;
            yield return new WaitForSeconds(fadeTime);
        }
        
        fadeIn = false;
    }
}
