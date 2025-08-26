using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;

public class CharacterManager : MonoBehaviour
{

    public CharacterStats stats;
    public CharacterState state;

    [SerializeField] float levelBottom = -40;

    [NonSerialized] public IHoldable holding = null;

    [SerializeField] DecalProjector shadowProjector;
    [SerializeField] public EventReference throwSound;
    [SerializeField] public EventReference pickupSound;
    [SerializeField] public EventReference boostSound;
    [SerializeField] public EventReference dropSound;
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
            state.Exit();
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

        //Temp kill player when fall
        if (transform.position.y < levelBottom) Die();

        SetShadowDistance();
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
                holdObj.transform.rotation = transform.rotation;
            }
        }
    }
    public void Geyser(float power)
    {
        state.Exit();
        state = new CharacterBounce(stats, power);
        state.Enter();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bouncy"))
        {
            state.Exit();
            state = new CharacterBounce(stats, stats.bouncePower);
            state.Enter();
        }
        else if (other.CompareTag("GlideRing") && stats.sheep.HoldingSheep()) 
        {
			Debug.Log("Glide Ring");
            int direction = Math.Sign(Vector3.Dot(other.transform.position - transform.position, other.transform.parent.forward));
            RuntimeManager.PlayOneShot(boostSound);
            state.Exit();
			state = new CharacterGlideBoost(stats, direction * other.transform.parent.forward);
			state.Enter();
		}
        else if (other.CompareTag("Death"))
        {
            Die();
        }
        else if (other.CompareTag("Drop"))
        {
            Drop();
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
        RuntimeManager.PlayOneShot(throwSound);
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
        if (holding != null) holding.Drop();
        holding = null;
        RuntimeManager.PlayOneShot(dropSound);
    }
    bool CanHold()
    {
        return holding == null;
    }
    IHoldable GetGrabable() 
    {
        float raycastOffset = 0.5f;
        float sphereRadius = 0.9f;
        RaycastHit hit;
        bool ifHit = Physics.SphereCast(transform.position - transform.forward * raycastOffset, sphereRadius,transform.forward, out hit, stats.grabRange + raycastOffset, LayerMask.GetMask("Grabbable"));
        if (ifHit) 
        {
            RuntimeManager.PlayOneShot(pickupSound);
            IHoldable toHold;
            if (hit.collider.gameObject.TryGetComponent<IHoldable>(out toHold)) 
            {
                return toHold;
            }
        }
        return null;
    }
    void Die()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().OnPlayerDeath();
    }
    void SetShadowDistance()
    {
        RaycastHit hit ;
        bool ifHit = Physics.Raycast(transform.position, Vector3.down, out hit, 100, ~3, QueryTriggerInteraction.Ignore);
        float depth;
        if (ifHit)
        {
            depth = Mathf.Max(hit.distance + 0.5f, 1f);
            
        }
        else
        {
            depth = 20;
        }
        shadowProjector.size = new Vector3(shadowProjector.size.x, shadowProjector.size.y, depth);
        shadowProjector.pivot = new Vector3(shadowProjector.pivot.x, shadowProjector.pivot.y, (depth / 2f) - 0.05f);
    }
}
