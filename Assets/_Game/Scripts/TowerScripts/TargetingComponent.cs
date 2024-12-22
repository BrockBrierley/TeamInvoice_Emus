using System.Collections.Generic;
using UnityEngine;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRadius;
    [SerializeField] private bool targetFurthest;

    private SphereCollider detectionCollider;
    private readonly List<Transform> enemiesInRange = new();
    public Transform CurrentTarget {  get; private set; }


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
        // Update the collider's radius when changes are made in the Inspector
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
            return;
        }

        float bestDistance = targetFurthest ? 0f : Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);

            if (targetFurthest ? distance > bestDistance : distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = enemy;
            }
        }

        CurrentTarget = bestTarget;
    }
}
