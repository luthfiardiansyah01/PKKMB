using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLogin : MonoBehaviour
{
    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "438B3"; // Replace with your real Title ID

        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier, // Acts like a unique player ID for testing
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result => Debug.Log("PlayFab login success! Player ID: " + result.PlayFabId),
            error => Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport()));
    }
}
