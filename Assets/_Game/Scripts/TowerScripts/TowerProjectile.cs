using System.Threading;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;

    private ObjectPool pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

   // void OnTriggerEnter(Collider other)
   // {
   //     if (other.CompareTag("Enemy"))
   //     {
   //         // Handle damage or enemy behavior here
   //         Destroy(gameObject);
   //     }
   // }

    public void Initialize(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //update collision logic at some point

        //return to the projectile pool
        pool.ReturnObject(gameObject);
    }

    private void OnEnable()
    {
        //returns to pool after lifespan if not already in pool
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        pool.ReturnObject(gameObject);
    }

    private void OnDisable()
    {
        //cancels any scheduled returns when deactivated
        CancelInvoke();
    }
}
