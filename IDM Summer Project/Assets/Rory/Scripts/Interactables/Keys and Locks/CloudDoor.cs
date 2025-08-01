using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SocialPlatforms;

public class CloudDoor : MonoBehaviour, IUnlockableObject
{
    CloudFadeController fadeController;
    [SerializeField] bool canClose;
    [SerializeField] int toOpen = 1;
    [SerializeField] float timeToFade = 1;
    BoxCollider boxCollider;
    bool locked;
    int locks;
    // Start is called before the first frame update
    void Start()
    {
        fadeController = GetComponent<CloudFadeController>();
        boxCollider = GetComponent<BoxCollider>();

        locks = toOpen;
        locked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!locked && fadeController.fadeValue < 1)
        {
            fadeController.fadeValue += Time.deltaTime / timeToFade;
        }
        else if (!locked)
        {
            fadeController.fadeValue = 1;
            
        }
        if (locked && fadeController.fadeValue > 0)
        {
            fadeController.fadeValue -= Time.deltaTime / timeToFade;
        }
        else if (locked)
        {
            fadeController.fadeValue = 0;
        }
        if(!locked && fadeController.fadeValue == 1) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
    public void Lock()
    {
        locks++;
        if (locks > 0 && canClose) 
        {
            locked = true;
            boxCollider.enabled = true;
        }
    }
    public void Unlock()
    {
        locks--;
        if (locks <= 0)
        {
            locked = false;
            boxCollider.enabled = false;
        }
    }
}
