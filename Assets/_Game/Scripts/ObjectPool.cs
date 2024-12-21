using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int intitialPoolSize = 10;

    //this stores pre instantiated objects for reuse
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        //pre instantiate objects
        for (int i = 0; i < intitialPoolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        //reuse an object form the pool if available
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive (true);
            return obj;
        }

        //if no objects are available, create a new one
        GameObject newObj = Instantiate(prefab);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        //deactivate the object and add it back to the pool
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
