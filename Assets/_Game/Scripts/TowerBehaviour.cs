using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    [Header("Tower Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform towerRotator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private LayerMask enemyLayer;


    [Header("Burst Attack Settings")]
    [SerializeField] private bool useBurstFire = false;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.2f;

    private Transform currentTarget;
    private float fireCooldown;
    private readonly List<Transform> enemiesInRange = new();




    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        if (currentTarget != null)
        {
            RotateTowardsTarget();
            HandleShooting();

        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, enemyLayer))
        {
            Debug.Log("Enemy entered detection range: " + other.name);
            enemiesInRange.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject, enemyLayer))
        {
            enemiesInRange.Remove(other.transform);
        }
    }

    void UpdateTarget()
    {
        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        //finds closest enemy
        float closestDistance = Mathf.Infinity;
        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy;
            }
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = currentTarget.position - towerRotator.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerRotator.rotation = Quaternion.Slerp(towerRotator.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            //checks bool to see if needs to burst fire or not
            if (useBurstFire)
            {
                StartCoroutine(FireBurst());
            }
            else
            {
                Shoot();
            }
            fireCooldown = 1f / fireRate;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Projectile prefab or fire point is not assigned!");
            return;
        }
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(burstInterval);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }

    // Helper method to check if a GameObject is in the specified LayerMask
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }
}
