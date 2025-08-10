using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneMove : MonoBehaviour
{
    public float delay = 3f; // waktu delay dalam detik sebelum pindah scene

    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GamePlay");
    }
}
