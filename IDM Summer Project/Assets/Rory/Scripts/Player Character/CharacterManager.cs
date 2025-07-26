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

        if (stats.input.Interact()) 
        {
            if (CanHold())
            {
                IHoldable grababble = GetGrabable();
                if (grababble != null)
                {
                    Grab(grababble);
                }
            }
            else 
            {
				Throw();
			}   
        }
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
    public void Geyser(float power)
    {
        state = new CharacterBounce(stats, power);
        state.Enter();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bouncy"))
        {
            Debug.Log("Bouncy");
            state = new CharacterBounce(stats, stats.sheep.stats.bouncePower);
            state.Enter();
        }
        else if (other.CompareTag("GlideRing") && stats.sheep.HoldingSheep()) 
        {
			Debug.Log("Glide Ring");
            transform.position = other.transform.position;
			state = new CharacterGlideBoost(stats, other.transform.forward);
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
    IHoldable GetGrabable() 
    {
        float raycastOffset = 0.5f;
        float sphereRadius = 1f;
        Debug.Log("Trying to grab");
        RaycastHit hit;
        bool ifHit = Physics.SphereCast(transform.position - transform.forward * raycastOffset, sphereRadius,transform.forward, out hit, stats.grabRange + raycastOffset, LayerMask.GetMask("Grabbable"));
        if (ifHit) 
        {
            Debug.Log("Entity Hit");
            IHoldable toHold;
            if (hit.collider.gameObject.TryGetComponent<IHoldable>(out toHold)) 
            {
                return toHold;
            }
        }
        return null;
    }
}
