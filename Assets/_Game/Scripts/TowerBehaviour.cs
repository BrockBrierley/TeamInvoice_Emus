using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    //[Header("Turret Settings")]
   // [SerializeField] private float rotationSpeed = 5f;
    public float rotationSpeed = 5f;
    public Transform turretHead;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 1f;

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
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy entered detection range: " + other.name);
            enemiesInRange.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
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
        Vector3 direction = currentTarget.position - turretHead.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // Add projectile-specific logic here (e.g., apply force or set direction)
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
