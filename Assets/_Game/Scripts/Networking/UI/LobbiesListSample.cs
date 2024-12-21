using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesListSample : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItemSample lobbyItemPrefab;

    private bool isJoining = false;
    private bool isRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;
        isRefreshing = true;

        QueryLobbiesOptions options = new QueryLobbiesOptions();

        try
        {

            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                //check if available slots is greater than (GT) 0
            new QueryFilter
                (
                field: QueryFilter.FieldOptions.AvailableSlots,
                op: QueryFilter.OpOptions.GT,
                value: "0"
                ),
            //check if the lobby is currently locked, lobby is EQ (equal to) 0
            new QueryFilter
                (
                field: QueryFilter.FieldOptions.IsLocked,
                op: QueryFilter.OpOptions.EQ,
                value: "0"
                )

            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItemSample item = Instantiate(lobbyItemPrefab, lobbyItemParent);
                item.Initialise(this, lobby);
            }
        }
        catch(LobbyServiceException lobbyException)
        {
            Debug.Log(lobbyException);
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton._instance.clientGameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException lobbyException)
        {
            Debug.Log(lobbyException);
        }
        isJoining = false;
    }
}
