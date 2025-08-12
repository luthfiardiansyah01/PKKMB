using UnityEngine;
using UnityEngine.SceneManagement;

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
}
