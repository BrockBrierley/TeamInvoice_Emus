using UnityEngine;

public class RotationComponent : MonoBehaviour
{

    [SerializeField] private Transform towerRotator;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float searchAngle = 45f;
    private bool isRotatingLeft = true;
    private float currentAngle = 0f;

    public void RotateTowards(Transform target)
    {
        Vector3 direction = target.position - towerRotator.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        towerRotator.rotation = Quaternion.Slerp(
            towerRotator.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed);
    }

    public void Search()
    {
        float rotationStep = rotationSpeed *5 * Time.deltaTime;
        float rotationDirection = isRotatingLeft ? -1f : 1f;

        towerRotator.Rotate(Vector3.up, rotationStep * rotationDirection);
        currentAngle += rotationStep;

        if (currentAngle >= searchAngle)
        {
            // Switch direction
            isRotatingLeft = !isRotatingLeft;
            // Reset the angle tracker
            currentAngle = 0f;
        }
    }
}
