using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    List<GameObject> entities = new List<GameObject>();
    [SerializeField] float power;
    [SerializeField] float period;

    [SerializeField] Animator geyserAnimator;

    public GameObject geyserIdleSFX;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ActivateTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ActivateTimer()
    {
        yield return new WaitForSeconds(period/2);
        geyserIdleSFX.SetActive(true);
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
