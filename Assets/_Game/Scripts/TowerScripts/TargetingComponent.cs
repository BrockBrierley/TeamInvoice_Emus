using System.Collections.Generic;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private bool targetFurthest;
    [SerializeField] private Transform firePoint;


    private bool isSearching = false;
    // exposes this for other components
    public bool IsSearching => isSearching;

    private SphereCollider detectionCollider;
    private readonly List<Transform> enemiesInRange = new();
    public Transform CurrentTarget { get; private set; }


    private void Awake()
    {
        // Find or add the SphereCollider
        detectionCollider = GetComponent<SphereCollider>();
        if (detectionCollider == null)
        {
            detectionCollider = gameObject.AddComponent<SphereCollider>();
        }

        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    private void OnValidate()
    {
        // Update the collider's radius when changes are made in the inspector
        if (detectionCollider == null)
        {
            detectionCollider = GetComponent<SphereCollider>();
            if (detectionCollider == null)
            {
                detectionCollider = gameObject.AddComponent<SphereCollider>();
            }
        }

        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            enemiesInRange.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            enemiesInRange.Remove(other.transform);
        }
    }

    public void UpdateTarget()
    {


        if (enemiesInRange.Count == 0)
        {
            CurrentTarget = null;
            //no enemies in detection sphere, wont enter searching "state"
            isSearching = false;
            return;
        }

        float bestDistance = targetFurthest ? 0f : Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            bool hasLineOfSight = HasLineOfSight(enemy);

            if (hasLineOfSight)
            {
                if (targetFurthest ? distance > bestDistance : distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = enemy;
                }
            }
        }
        if (bestTarget != null)
        {
            CurrentTarget = bestTarget;
            //finds a target and stops searching
            isSearching = false;
        }
        else
        {
            CurrentTarget = null;
            isSearching = true;
        }
       // CurrentTarget = bestTarget;
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector3 direction = target.position - firePoint.position;
        float distance = direction.magnitude;

        //debug line to see the raycast in action
        Debug.DrawLine(firePoint.position, target.position, Color.blue);

        //ex;udes the projectile layer
        int layerMask = ~LayerMask.GetMask("Projectile");

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, distance, layerMask))
        {
            if (hit.transform == target)
            {
                return true;
            }
            else
            {               
                return false;
            }
            
        }
        return true;
    }
}
