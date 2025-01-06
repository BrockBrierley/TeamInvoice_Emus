using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private const int MaxConnections = 5;
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;

    private NetworkServer networkServer;

    public async void Dispose()
    {
        HostSingleton._instance.StopCoroutine(nameof(ServerHeartbeatLobby));

        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(LobbyServiceException error)
            {
                Debug.Log(error);
            }

            lobbyId = string.Empty;
        }
        networkServer?.Dispose();
    }

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception exception)
        {
            Debug.Log(exception);
            return;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

            //Modify to allow making lobbies private
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {   "JoinCode", 
                    new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };
            string serverOwnerName = PlayerPrefs.GetString(NameSetup.PLAYER_NAME_KEY, "MissingName");
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync($"{serverOwnerName}'s Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;
            HostSingleton._instance.StartCoroutine(ServerHeartbeatLobby(15));
        }
        catch(LobbyServiceException lobbyServiceFailure)
        {
            Debug.Log(lobbyServiceFailure);
            return;
        }


        networkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData
        {
            displayName = PlayerPrefs.GetString(NameSetup.PLAYER_NAME_KEY, "MissingName"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        NetworkManager.Singleton.StartHost();

        //Load Scene
        NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene, LoadSceneMode.Single);
    }


    private IEnumerator ServerHeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        //want to run until the co-rutine is shut down
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
