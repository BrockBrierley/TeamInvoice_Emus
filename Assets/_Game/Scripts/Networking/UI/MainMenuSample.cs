using TMPro;
using UnityEngine;

public class MainMenuSample : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    public async void StartHost()
    {
        await HostSingleton._instance.hostGameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton._instance.clientGameManager.StartClientAsync(joinCodeField.text);
    }
}
