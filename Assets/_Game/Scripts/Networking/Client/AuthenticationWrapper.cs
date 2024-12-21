using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (authState == AuthState.Authenticated)
        {
            return authState;
        }

        if (authState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return authState;
        }

        await SignInAnonymouslyAsync(maxTries);

        return authState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        //Set state as authenticating
        authState = AuthState.Authenticating;

        int tries = 0;
        while (authState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                //await AuthenticationService.Instance.SignInWithSteamAsync()

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException authenticationException)
            {
                Debug.LogError(authenticationException);
                authState = AuthState.Error;
            }
            catch(RequestFailedException RequestFailedException)
            {
                Debug.LogError(RequestFailedException);
                authState = AuthState.Error;
            }

            tries++;
            await Task.Delay(1000);
        }

        if (authState != AuthState.Authenticated)
        {
            Debug.LogWarning("Player failed to authenticate");
            authState = AuthState.TimeOut;
        }
    }
    private static async Task<AuthState> Authenticating()
    {
        while(authState == AuthState.Authenticating || authState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return authState;
    }
}




public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}