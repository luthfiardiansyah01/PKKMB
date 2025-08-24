using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;

public class ChangeScenecpt : MonoBehaviour
{
    // Fungsi untuk ganti scene berdasarkan nama
    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Fungsi untuk ganti scene berdasarkan index (build index)
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Contoh kalau mau keluar dari game
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }

    public void LogoutPlayfab()
    {
        // Hapus session PlayFab
        PlayFabClientAPI.ForgetAllCredentials();

        // Bisa juga bersihkan PlayerPrefs kalau kamu simpan ID/email
        PlayerPrefs.DeleteKey("PlayFabId");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.DeleteKey("Password");
        PlayerPrefs.Save();

        Debug.Log("Logout berhasil, kembali ke login scene.");

        // Pindah ke Login Scene
        SceneManager.LoadScene("Main Menu");
    }
}
