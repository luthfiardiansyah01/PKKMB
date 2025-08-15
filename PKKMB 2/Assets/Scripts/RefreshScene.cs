using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using UnityEngine.SceneManagement;
public class RefreshScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshScenes()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
