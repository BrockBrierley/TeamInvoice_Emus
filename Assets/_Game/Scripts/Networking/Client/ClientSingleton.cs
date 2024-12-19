using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public static ClientSingleton _instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            instance = FindAnyObjectByType<ClientSingleton>();
            if (instance == null)
            {
                Debug.Log("No client singleton in the scene");
                return null;
            }
            return instance;
        }
    }

    public ClientGameManager clientGameManager { get; private set; }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        clientGameManager = new ClientGameManager();

        return await clientGameManager.InitAsync();
    }
}
