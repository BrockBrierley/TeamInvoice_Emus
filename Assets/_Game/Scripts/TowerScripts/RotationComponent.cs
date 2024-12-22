using UnityEngine;

public class RotationComponent : MonoBehaviour
{

    [SerializeField] private Transform towerRotator;
    [SerializeField] private float rotationSpeed;

    public void RotateTowards(Transform target)
    {
        Vector3 direction = target.position - towerRotator.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerRotator.rotation = Quaternion.Slerp(
            towerRotator.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed);
    }
}
