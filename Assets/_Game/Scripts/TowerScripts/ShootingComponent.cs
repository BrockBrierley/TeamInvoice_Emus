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
            // //runs the LOS check
            // if (HasLineOfSight(target))
            // {
            //     if (useBurstFire)
            //     {
            //         StartCoroutine(FireBurst(target));
            //     }
            //     else
            //     {
            //         Shoot(target);
            //     }
            //     fireCooldown = 1f / fireRate;
            // }
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


  // //will probably need to update the raycast check in the future to only check for specific
  // //obstruction layers as the project builds in complexity, for now its only checking if the 
  // //raycast hits anything other than the target/enemy
  // private bool HasLineOfSight(Transform target)
  // {
  //     Vector3 direction = target.position - firePoint.position;
  //     float distance = direction.magnitude;
  //
  //     //debug line to see the raycast in action
  //     Debug.DrawLine(firePoint.position, target.position, Color.red);
  //
  //     if (Physics.Raycast(firePoint.position,direction, out RaycastHit hit, distance))
  //     {
  //         //checks to see if racast hit target directly
  //         if(hit.transform == target)
  //         {
  //             return true;
  //         }
  //         else
  //         {
  //             //raycast hit something other than the enemy
  //             return false;
  //         }
  //
  //     }
  //         //no obstructions or hits, should be a clear LOS
  //         return true;
  // }
  //
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
