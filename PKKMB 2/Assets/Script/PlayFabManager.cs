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
    public GameObject successPanel; // Panel yang aktif setelah login/register berhasil

    // Start is called before the first frame update
    void Start()
    {
        ResetLoginUI(); // Kosongkan input dan pesan saat pertama dijalankan
    }

    public void ResetLoginUI()
    {
        emailInput.text = "";
        passwordInput.text = "";
        messageText.text = "";
    }

    public void RegisterButton()
    {
        messageText.text = "";

        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            messageText.text = "Email dan Password harus diisi.";
            return;
        }

        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password minimal 6 karakter.";
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

    public void LoginButton()
    {
        messageText.text = "";

        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            messageText.text = "Email dan Password harus diisi.";
            return;
        }

        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    public void ResetPasswordButton()
    {
        messageText.text = "";

        if (string.IsNullOrEmpty(emailInput.text))
        {
            messageText.text = "Masukkan email untuk reset password.";
            return;
        }

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "438B3" // Ganti dengan Title ID kamu
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Email reset password telah dikirim.";
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Registrasi berhasil!";
        if (successPanel != null)
        {
            successPanel.SetActive(true); // Aktifkan panel jika tersedia
        }
    }

    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Login berhasil!";
        if (successPanel != null)
        {
            SceneManager.LoadScene("Story Menu"); // Aktifkan panel jika tersedia
        }
        Debug.Log("Login berhasil");
    }

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }
}
