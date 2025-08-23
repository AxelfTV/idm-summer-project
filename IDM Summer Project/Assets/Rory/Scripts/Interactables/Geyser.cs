using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    List<GameObject> entities = new List<GameObject>();
    [SerializeField] float power;
    [SerializeField] float period;
    [SerializeField] float maxStartDelay;

    [SerializeField] Animator geyserAnimator;

    public GameObject geyserIdleSFX;
    float startDelay;
    // Start is called before the first frame update
    void Start()
    {
        startDelay = maxStartDelay * Random.value;
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator StartDelay() 
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(ActivateTimer());
    }
    IEnumerator ActivateTimer()
    {
        yield return new WaitForSeconds(period/2);
        geyserIdleSFX.SetActive(true);
        geyserAnimator.SetTrigger("canReady");
        yield return new WaitForSeconds(period);
        geyserIdleSFX.SetActive(false);
        geyserAnimator.SetTrigger("canJet");
        Activate();
        StartCoroutine(ActivateTimer());

    }
    void Activate()
    {
        foreach (var entity in entities)
        {
            entity.GetComponent<CharacterManager>().Geyser(power);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entities.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entities.Remove(other.gameObject);
        }
    }
}
