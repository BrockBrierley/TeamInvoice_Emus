using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
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
    [SerializeField] private float detectionRadius = 15f;

    [Header("Burst Attack Settings")]
    [SerializeField] private bool useBurstFire = false;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstInterval = 0.2f;

    private SphereCollider detectionCollider;
    private Transform currentTarget;
    private float fireCooldown;
    private readonly List<Transform> enemiesInRange = new();


    private void Awake()
    {
        //get sphere collider and set its radius
        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider != null && detectionCollider.isTrigger) 
        {
            detectionCollider.radius = detectionRadius;
        }
        else
        {
            Debug.LogError("No sphere collider found or its not set as a trigger");
        }
    }

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
        // Calculate the center of the target based on its bounds
        Vector3 targetPosition = GetTargetCenter(currentTarget);
       
        Debug.DrawLine(towerRotator.position, targetPosition, Color.green);
        Vector3 direction = targetPosition - towerRotator.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerRotator.rotation = Quaternion.Slerp(towerRotator.rotation, lookRotation, Time.deltaTime * rotationSpeed);

       //  Vector3 direction = currentTarget.position - towerRotator.position;
       //  Quaternion lookRotation = Quaternion.LookRotation(direction);
       //  towerRotator.rotation = Quaternion.Slerp(towerRotator.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    // Helper method to calculate the center of the target
    private Vector3 GetTargetCenter(Transform target)
    {
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider != null)
        {
            // Get the center of the collider
            return targetCollider.bounds.center;
        }
        else
        {
            // Fallback to transform position if no collider is found
            return target.position;
        }
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
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // Helper method to check if a GameObject is in the specified LayerMask
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return (layerMask.value & (1 << obj.layer)) != 0;
    }

    private void OnValidate()
    {
        // Update the radius in the Editor when the value changes
        if (detectionCollider == null) detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider != null)
        {
            detectionCollider.radius = detectionRadius;
        }
    }
}
