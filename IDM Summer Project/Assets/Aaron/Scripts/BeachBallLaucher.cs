using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BeachBallLaucher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 50f;
    public bool isInTriggerZone;
    public Animator cannonAnimator;
    public EventReference shootSound;
    public Transform cloudVFXSpawnpoint;
    public GameObject cloudVFX;
    private const string shootTrigger = "canShoot";
    private bool shot;
    private bool isWaitingToShoot = false;
    private bool firstShotDone = false;

    private List<GameObject> activeProjectiles = new List<GameObject>();

    void Update()
    {
        if (isInTriggerZone && !shot && !isWaitingToShoot)
        {
            if (!firstShotDone)
            {
                Shoot();
                firstShotDone = true;
            }
            else
            {
                StartCoroutine(DelayedShoot());
            }
        }

        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            if (activeProjectiles[i] == null)
            {
                activeProjectiles.RemoveAt(i);
                shot = false;
            }
        }
    }

    IEnumerator DelayedShoot()
    {
        isWaitingToShoot = true;
        yield return new WaitForSeconds(3f); 
        Shoot();
        isWaitingToShoot = false;
    }


    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }

        activeProjectiles.Add(projectile);
        shot = true;

        cannonAnimator.SetTrigger(shootTrigger);
        RuntimeManager.PlayOneShot(shootSound, firePoint.position);

        GameObject vfxInstance = Instantiate(cloudVFX, cloudVFXSpawnpoint.position, cloudVFXSpawnpoint.rotation);
        Destroy(vfxInstance, 2f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTriggerZone = true; 
        }
    }
}