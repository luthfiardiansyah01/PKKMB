using System.Collections.Generic;
using UnityEngine;
using TMPro; // Penting untuk TextMeshPro
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class ScoreDisplay : MonoBehaviour
{
    // --- REVISI: Tambahkan variabel publik untuk menampung referensi ke UI Text ---
    // Ini cara terbaik agar skrip tahu di mana harus menampilkan skor.
    [Header("UI Settings")]
    [Tooltip("Seret GameObject TextMeshPro untuk menampilkan skor ke slot ini.")]
    public TextMeshProUGUI scoreTextDisplay;

    private string currentSessionId;

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        CheckSession();

        // --- TAMBAHAN: Cek apakah pemain sudah login ---
        // Jika sudah, panggil fungsi untuk mengambil skor.
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            FetchAndDisplayScore();
            Debug.Log("Poin Tampil");
        }
        else
        {
            Debug.LogWarning("Pemain belum login ke PlayFab. Skor tidak bisa ditampilkan.");
            // Beri nilai default jika belum login
            if (scoreTextDisplay != null)
            {
                scoreTextDisplay.text = "0";
            }
        }
    }

    // --- DIHAPUS: Fungsi Update() kosong tidak diperlukan ---

    void CheckSession()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("deviceSession"))
            {
                string sessionFromServer = result.Data["deviceSession"].Value;
                if (sessionFromServer != currentSessionId)
                {
                    Debug.LogWarning("Sesi tidak valid. User login dari device lain.");
                    SceneManager.LoadScene("Main Menu");
                }
            }
        },
        error => Debug.LogError("Gagal ambil sesi: " + error.GenerateErrorReport()));
    }

    // --- REVISI TOTAL: Fungsi ini diubah total menjadi benar ---
    /// <summary>
    /// Fungsi utama untuk mengambil data 'coin' (poin) dari PlayFab dan menampilkannya.
    /// </summary>
    public void FetchAndDisplayScore()
    {
        // Pengecekan untuk memastikan UI Text sudah terhubung di Inspector
        if (scoreTextDisplay == null)
        {
            Debug.LogError("scoreTextDisplay belum di-set di Inspector!", this.gameObject);
            return;
        }

        Debug.Log("Meminta data skor ('coin') dari PlayFab...");
        var request = new GetUserDataRequest
        {
            // Meminta hanya data 'coin' agar lebih efisien
            Keys = new List<string> { "coin" }
        };

        // Memanggil API PlayFab untuk mengambil data
        PlayFabClientAPI.GetUserData(request, OnFetchSuccess, OnError);
    }

    /// <summary>
    /// Fungsi yang dijalankan jika berhasil mengambil data dari PlayFab.
    /// </summary>
    private void OnFetchSuccess(GetUserDataResult result)
    {
        // Cek apakah data 'coin' ada
        if (result.Data != null && result.Data.ContainsKey("coin"))
        {
            string playerScore = result.Data["coin"].Value;
            scoreTextDisplay.text = playerScore;
            Debug.Log("Skor berhasil ditampilkan: " + playerScore);
        }
        else
        {
            // Jika tidak ada (misal: pemain baru), tampilkan 0
            scoreTextDisplay.text = "0";
            Debug.Log("Pemain belum punya data 'coin'. Menampilkan 0.");
        }
    }

    /// <summary>
    /// Fungsi yang dijalankan jika terjadi error.
    /// </summary>
    private void OnError(PlayFabError error)
    {
        Debug.LogError("Terjadi kesalahan dengan PlayFab: " + error.GenerateErrorReport());
        if (scoreTextDisplay != null)
        {
            scoreTextDisplay.text = "Error";
        }
    }
}