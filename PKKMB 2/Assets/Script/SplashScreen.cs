using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float delayTime = 3f; // Durasi splash screen
    public string nextSceneName = "MainScene";

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(nextSceneName);
    }
}
