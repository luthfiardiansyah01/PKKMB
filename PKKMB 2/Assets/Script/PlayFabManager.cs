using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject personalInfoPanel;
    public GameObject waitingVerifPanel;
    public GameObject emailVerifAgainPanel;

    [Header("Login Fields")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    [Header("Register Fields")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField confirmPasswordInput;

    [Header("Personal Info Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField fullnameInput;
    public TMP_InputField majorInput;
    public TMP_InputField facultyInput;
    public TMP_InputField groupNumberInput;

    [Header("Message Display")]
    public TMP_Text loginMessageText;
    public TMP_Text registerMessageText;
    public TMP_Text personalInfoMessageText;

    [Header("input Email Verification Again")]
    public TMP_InputField emailVeriftoSend;

    private string currentSessionId;
    private string tempEmail;
    private string tempPassword;
    private string _playFabId;
    private bool isRegist = false;
    private Coroutine _verifCheckCoroutine;

    [Header("Reset Password Fields")]
    public TMP_InputField resetEmailInput;

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        ShowLoginPanel();
    }

    #region Panel Navigation
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        personalInfoPanel.SetActive(false);
        waitingVerifPanel.SetActive(false);
        emailVerifAgainPanel.SetActive(false);
        ClearMessage();
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        personalInfoPanel.SetActive(false);
        waitingVerifPanel.SetActive(false);
        emailVerifAgainPanel.SetActive(false);
        ClearMessage();
    }

    public void ShowPersonalInfoPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        personalInfoPanel.SetActive(true);
        waitingVerifPanel.SetActive(false);
        emailVerifAgainPanel.SetActive(false);
        ClearMessage();
    }

    public void ShowWaitingVerifPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        personalInfoPanel.SetActive(false);
        waitingVerifPanel.SetActive(true);
        emailVerifAgainPanel.SetActive(false);
        ClearMessage();
    }

    public void ShowEmailVerifAgainPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        personalInfoPanel.SetActive(false);
        waitingVerifPanel.SetActive(false);
        emailVerifAgainPanel.SetActive(true);
        ClearMessage();
    }
    #endregion

    #region Button Functions
    public void ProceedToPersonalInfoButton()
    {
        ClearMessage();
        if (registerPasswordInput.text.Length < 6)
        {
            registerMessageText.text = "Password minimal 6 karakter.";
            return;
        }
        if (registerPasswordInput.text != confirmPasswordInput.text)
        {
            registerMessageText.text = "Password dan konfirmasi tidak cocok.";
            return;
        }

        tempEmail = registerEmailInput.text;
        tempPassword = registerPasswordInput.text;

        isRegist = true;
        ShowPersonalInfoPanel();
    }

    public void SignUpButton()
    {
        ClearMessage();
        var request = new RegisterPlayFabUserRequest
        {
            Email = tempEmail,
            Password = tempPassword,
            Username = usernameInput.text,
            DisplayName = usernameInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void LoginButton()
    {
        ClearMessage();
        var request = new LoginWithEmailAddressRequest
        {
            Email = loginEmailInput.text,
            Password = loginPasswordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = resetEmailInput.text,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    public void ResendVerificationEmailButton()
    {
        if (!string.IsNullOrEmpty(emailVeriftoSend.text))
        {
            AddOrUpdateContactEmail(emailVeriftoSend.text);
        }
        else
        {
            Debug.LogError("Email tidak ditemukan. Silakan masukkan email.");
        }
    }
    #endregion

    #region PlayFab Callbacks
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        personalInfoMessageText.text = "Registrasi berhasil! Menyimpan data...";
        Debug.Log("Registration successful. Now updating user data.");

        _playFabId = result.PlayFabId;

        UpdateUserCustomData();

        GetPlayerProfileAndSetEmail(tempEmail);
    }

    void OnLoginSuccess(LoginResult result)
    {
        loginMessageText.text = "Logged In!";
        Debug.Log("Successful Login");
        _playFabId = result.PlayFabId;
        CreateSession();

        // Cek email verifikasi setelah login
        isRegist = false;
        GetPlayerProfileAndCheckEmailStatus();
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        loginMessageText.text = "Email untuk reset password telah dikirim.";
    }

    void OnDataUpdated(UpdateUserDataResult result)
    {
        Debug.Log("User data updated successfully.");
    }

    void OnError(PlayFabError error)
    {
        if (error.HttpCode == 409)
        {
            registerMessageText.text = "Email sudah digunakan. Gunakan email lain.";
            ShowRegisterPanel();
        }
        else
        {
            if (loginPanel.activeSelf)
            {
                loginMessageText.text = error.ErrorMessage;
            }
            else if (registerPanel.activeSelf)
            {
                registerMessageText.text = error.ErrorMessage;
            }
            else if (personalInfoPanel.activeSelf)
            {
                personalInfoMessageText.text = error.ErrorMessage;
            }
        }
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Helper Functions
    void UpdateUserCustomData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Username", usernameInput.text },
                { "Fullname", fullnameInput.text },
                { "Major", majorInput.text },
                { "Faculty", facultyInput.text },
                { "GroupNumber", groupNumberInput.text }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataUpdated, OnError);
    }

    void CreateSession()
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

    void GetPlayerProfileAndSetEmail(string emailAddress)
    {
        var request = new GetPlayerProfileRequest
        {
            PlayFabId = _playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowContactEmailAddresses = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, result =>
        {
            if (result.PlayerProfile.ContactEmailAddresses != null &&
                result.PlayerProfile.ContactEmailAddresses.Count > 0)
            {
                Debug.Log("Akun sudah punya contact email, tidak perlu update.");
                // Jika ini dari flow register, lanjutkan ke verifikasi panel
                if (isRegist)
                {
                    ShowWaitingVerifPanel();
                    _verifCheckCoroutine = StartCoroutine(CheckEmailStatusRepeat(5));
                }
            }
            else
            {
                AddOrUpdateContactEmail(emailAddress);
            }
        }, OnError);
    }

    void AddOrUpdateContactEmail(string emailAddress)
    {
        var request = new AddOrUpdateContactEmailRequest
        {
            EmailAddress = emailAddress
        };
        PlayFabClientAPI.AddOrUpdateContactEmail(request, OnEmailUpdateSuccess, OnError);
    }

    void OnEmailUpdateSuccess(AddOrUpdateContactEmailResult result)
    {
        Debug.Log("Contact email berhasil ditambahkan. Email verifikasi akan dikirim.");
        ShowWaitingVerifPanel();

        // Mulai coroutine pengecekan berulang
        if (_verifCheckCoroutine != null) StopCoroutine(_verifCheckCoroutine);
        _verifCheckCoroutine = StartCoroutine(CheckEmailStatusRepeat(5));
    }

    private IEnumerator CheckEmailStatusRepeat(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Mulai memeriksa status verifikasi email...");

            GetPlayerProfileAndCheckEmailStatus();
        }
    }

    private void GetPlayerProfileAndCheckEmailStatus()
    {
        var request = new GetPlayerProfileRequest
        {
            PlayFabId = _playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowContactEmailAddresses = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, OnGetPlayerProfileSuccess, OnError);
    }

    private void OnGetPlayerProfileSuccess(GetPlayerProfileResult result)
    {
        if (result.PlayerProfile.ContactEmailAddresses != null && result.PlayerProfile.ContactEmailAddresses.Count > 0)
        {
            var contactEmail = result.PlayerProfile.ContactEmailAddresses[0];

            if (contactEmail.VerificationStatus == EmailVerificationStatus.Confirmed)
            {
                Debug.Log("Email " + contactEmail.EmailAddress + " telah diverifikasi.");
                if (_verifCheckCoroutine != null) StopCoroutine(_verifCheckCoroutine);

                if (isRegist)
                {
                    SceneManager.LoadScene("Story Menu");
                }
                else
                {
                    SceneManager.LoadScene("Gameplay");
                }
            }
            else
            {
                Debug.Log("Email " + contactEmail.EmailAddress + " belum diverifikasi. Menunggu konfirmasi.");
                ShowWaitingVerifPanel();
            }
        }
        else
        {
            Debug.Log("Tidak ada email kontak yang ditemukan di profil pemain.");
            if (_verifCheckCoroutine != null) StopCoroutine(_verifCheckCoroutine);
            ShowEmailVerifAgainPanel();
        }
    }

    void ClearMessage()
    {
        if (loginMessageText != null) loginMessageText.text = "";
        if (registerMessageText != null) registerMessageText.text = "";
        if (personalInfoMessageText != null) personalInfoMessageText.text = "";
    }
    #endregion
}