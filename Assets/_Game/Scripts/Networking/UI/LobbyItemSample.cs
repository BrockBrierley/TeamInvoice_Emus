using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItemSample : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayerText;

    private LobbiesListSample lobbyList;
    private Lobby lobbyRef;

    public void Initialise (LobbiesListSample lobbiesList, Lobby lobby)
    {
        lobbyList = lobbiesList;
        lobbyRef = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayerText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers} Players";
    }

    public void Join()
    {
        lobbyList.JoinAsync(lobbyRef);
    }
}
