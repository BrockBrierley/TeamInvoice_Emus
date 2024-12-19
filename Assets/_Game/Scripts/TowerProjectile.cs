using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Handle damage or enemy behavior here
            Destroy(gameObject);
        }
    }
}
