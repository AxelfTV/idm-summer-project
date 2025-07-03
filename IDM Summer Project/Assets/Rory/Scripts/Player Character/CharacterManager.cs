using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    CharacterStats stats;
    CharacterState state;

    IHoldable holding = null;

    private void Awake()
    {
        //ignore collisions with sheep
        Physics.IgnoreLayerCollision(6, 7);

        stats = GetComponent<CharacterStats>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        state = new CharacterIdle(stats);
        state.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterState newState = state.NewState();
        if (newState != null) 
        { 
            state = newState;
            state.Enter();
        }

        if (stats.input.Interact()) Throw();
    }
    private void FixedUpdate()
    {
        state.Update();

        if (holding != null) 
        {
            GameObject holdObj = holding.GetGameObject();
            if (!holdObj.activeSelf)
            {
                Drop(); 
            }
            else
            {
                holdObj.transform.position = stats.holdPosition.position;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bouncy"))
        {
            Debug.Log("Bouncy");
            state = new CharacterBounce(stats);
            state.Enter();
        }
    }
    public void Grab(IHoldable item)
    {
        if (!CanHold()) return;
        if (item.Grab()) holding = item;
    }
    void Throw()
    {
        if(holding != null) holding.Throw(transform.forward);
        holding = null;
    }
    public bool SheepToHold(IHoldable sheep)
    {
        if (!CanHold()) return false;
        holding = sheep;
        return true;
    }
    public void Drop()
    {
        holding = null;
    }
    bool CanHold()
    {
        return holding == null;
    }
}
