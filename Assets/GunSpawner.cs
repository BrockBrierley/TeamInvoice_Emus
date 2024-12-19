using UnityEngine;
using Unity.Netcode;
public class GunSpawner : MonoBehaviour
{

    [SerializeField] private GameObject gunPrefab;
    [SerializeField] private Transform gunAttachPoint;

    private void Start()
    {
        GameObject gun = Instantiate(gunPrefab, gunAttachPoint.position, gunAttachPoint.rotation, gunAttachPoint);
        gun.transform.SetParent(gunAttachPoint, false);
    }
}
