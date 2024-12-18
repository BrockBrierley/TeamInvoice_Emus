using Unity.Netcode.Components;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
