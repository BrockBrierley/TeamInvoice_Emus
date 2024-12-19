using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public static HostSingleton _instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            instance = FindAnyObjectByType<HostSingleton>();
            if (instance == null)
            {
                Debug.Log("No host singleton in the scene");
                return null;
            }
            return instance;
        }
    }

    public HostGameManager hostGameManager { get; private set; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        hostGameManager = new HostGameManager();
    }
}
