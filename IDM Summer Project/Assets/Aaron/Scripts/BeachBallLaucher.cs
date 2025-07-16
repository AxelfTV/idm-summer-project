using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBallLaucher : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 50f;
    public bool isInTriggerZone;
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
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
        }

        activeProjectiles.Add(projectile);
        shot = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInTriggerZone = true; 
        }
    }
}