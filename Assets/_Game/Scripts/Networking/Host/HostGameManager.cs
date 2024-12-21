using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private const int MaxConnections = 5;
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;

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
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("My Lobby", MaxConnections, lobbyOptions);

            lobbyId = lobby.Id;
            HostSingleton._instance.StartCoroutine(ServerHeartbeatLobby(15));
        }
        catch(LobbyServiceException lobbyServiceFailure)
        {
            Debug.Log(lobbyServiceFailure);
            return;
        }



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
