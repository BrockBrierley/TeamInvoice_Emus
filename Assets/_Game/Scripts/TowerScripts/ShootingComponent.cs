using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootingComponent : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ObjectPool projectilePool;
    [SerializeField] private float fireRate;
    [SerializeField] private bool useBurstFire;
    [SerializeField] private int burstCount;
    [SerializeField] private float burstInterval;

    private float fireCooldown;

    public void HandleShooting(Transform target)
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown < 0)
        {            
            if (useBurstFire)
            {
                StartCoroutine(FireBurst(target));
            }
            else
            {
                Shoot(target);
            }
            fireCooldown = 1f / fireRate;
        }
    }


    private void Shoot(Transform target)
    {
        GameObject projectile = projectilePool.GetObject();
        projectile.transform.position = firePoint.position;
        projectile.transform.LookAt(target.position);

        //if i need any projectile specific setup put it here

        TowerProjectile projectileScript = projectile.GetComponent<TowerProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(projectilePool);
        }
    }

    private IEnumerator FireBurst(Transform target)
    {
        for (int i = 0; i < burstCount; i++)
        {
            Shoot(target);
            yield return new WaitForSeconds(burstInterval);
        }
    }
}
