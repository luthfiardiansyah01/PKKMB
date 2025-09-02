using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ProfileManager : MonoBehaviour
{
    [Header("UI Input Fields")]
    public TMP_InputField emailInput;
    public TMP_InputField fullnameInput;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput; // Hanya untuk mengubah, bukan menampilkan
    public TMP_InputField majorInput;
    public TMP_InputField facultyInput;
    public TMP_InputField groupNumberInput;

    [Header("UI Display Text")]
    public TMP_Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        // Nonaktifkan interaksi untuk field yang tidak boleh diubah
        SetFieldsAsReadOnly();

        // Ambil data profil dari PlayFab saat scene dimulai
        GetPlayerProfileData();
    }

    void SetFieldsAsReadOnly()
    {
        // Kolom ini diambil dari PlayFab dan tidak untuk diubah oleh pengguna di halaman ini
        emailInput.readOnly = true;
        fullnameInput.readOnly = true;
        majorInput.readOnly = true;
        facultyInput.readOnly = true;
        groupNumberInput.readOnly = true;

        // Atur placeholder untuk password
        passwordInput.placeholder.GetComponent<TMP_Text>().text = "Kosongkan jika tidak ingin ganti password";
    }

    public void GetPlayerProfileData()
    {
        messageText.text = "Mengambil data...";

        // 1. Ambil Account Info (Username, Email)
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), OnGetProfileSuccess, OnError);

        // 2. Ambil Custom User Data (Fullname, Major, Faculty, etc.)
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnError);
    }

    void OnGetProfileSuccess(GetPlayerProfileResult result)
    {
        usernameInput.text = result.PlayerProfile.DisplayName;
        
        // Email didapat dari GetAccountInfo, tapi untuk profil biasanya cukup DisplayName
        // Jika butuh email, panggil GetAccountInfoRequest
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), accountInfo => {
            emailInput.text = accountInfo.AccountInfo.PrivateInfo.Email;
        }, OnError);

        messageText.text = ""; // Hapus pesan loading
    }

    void OnGetUserDataSuccess(GetUserDataResult result)
    {
        if (result.Data != null)
        {
            // Tampilkan data di setiap input field jika ada
            if (result.Data.ContainsKey("Fullname")) fullnameInput.text = result.Data["Fullname"].Value;
            if (result.Data.ContainsKey("Major")) majorInput.text = result.Data["Major"].Value;
            if (result.Data.ContainsKey("Faculty")) facultyInput.text = result.Data["Faculty"].Value;
            if (result.Data.ContainsKey("GroupNumber")) groupNumberInput.text = result.Data["GroupNumber"].Value;
        }
        messageText.text = ""; // Hapus pesan loading
    }

    public void SaveProfileButton()
    {
        messageText.text = "Menyimpan...";

        // 1. Update Username (Display Name)
        var updateDisplayNameRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = usernameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(updateDisplayNameRequest, OnUpdateDisplayNameSuccess, OnError);

        // 2. Update Password (jika diisi)
        if (!string.IsNullOrEmpty(passwordInput.text))
        {
            if (passwordInput.text.Length < 6)
            {
                messageText.text = "Password baru minimal 6 karakter.";
                return;
            }
            var updatePasswordRequest = new UpdateUserTitleDisplayNameRequest
            {
                // Note: API untuk update password biasanya memerlukan verifikasi email
                // Untuk kesederhanaan, kita hanya contohkan update data lain.
                // Lihat bagian keamanan password di bawah.
            };
            // Sebaiknya gunakan fitur "Forgot Password" PlayFab untuk alur reset password.
        }
    }

    void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        messageText.text = "Data berhasil disimpan!";
        Debug.Log("Display name updated successfully.");
    }

    public void LogOutButton()
    {
        // Menghapus semua kredensial yang tersimpan di lokal
        PlayFabClientAPI.ForgetAllCredentials();
        // Kembali ke scene login (ganti "LoginScene" dengan nama scene login Anda)
        SceneManager.LoadScene("LoginScene");
    }

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.LogError(error.GenerateErrorReport());
    }
    
}