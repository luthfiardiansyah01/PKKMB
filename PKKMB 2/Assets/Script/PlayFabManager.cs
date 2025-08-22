using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    private string currentSessionId;
    // Start is called before the first frame update
    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        LoginHAHAHAHAHAHA();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password Minimal 6 Karakter";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void createSession()
    {
        var updateRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "deviceSession", currentSessionId }
            }
        };

        PlayFabClientAPI.UpdateUserData(updateRequest,
            updateResult => Debug.Log("Session saved."),
            error => Debug.LogError("Failed to save session: " + error.GenerateErrorReport()));
    }


    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = $"{emailInput.text }@gmail.com",
            Password = passwordInput.text,
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void LoginHAHAHAHAHAHA()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = "kucing@gmail.com",
            Password = "123456",
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "438B3"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent";
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Register and logged in!";
        createSession();
        SceneManager.LoadScene("Story Menu");
    }

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged In";
        Debug.Log("Successful Login");
        createSession();
        SceneManager.LoadScene("Story Menu");
    }
}
