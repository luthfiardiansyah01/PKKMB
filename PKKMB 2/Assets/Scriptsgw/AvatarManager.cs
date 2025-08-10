using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Mapbox.Platform;

public class AvatarManager : MonoBehaviour
{
    // Start is called before the first frame update
    private string currentSessionId;
    public Transform content;
    // private string location = "Assets/Resources/Character";

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        // changeCharacter("ch002");
        getCurrentChar();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getCurrentChar()
    {
        CheckSession();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
    {
        if (result.Data != null && result.Data.ContainsKey("currentChar"))
        {
            string value = result.Data["currentChar"].Value; // misalnya "ch001"
            Debug.Log("Dapat ID karakter dari UserData: " + value);

            string path = $"Character/{value}/Base";
            GameObject characterPrefab = Resources.Load<GameObject>(path);

            if (characterPrefab != null)
            {
                Debug.Log("Prefab ditemukan: " + path);
                // Bersihkan 
                foreach (Transform child in content)
                {
                    Destroy(child.gameObject);
                }

                // Instantiate prefab ke parent content
                GameObject instance = Instantiate(characterPrefab, content);
                instance.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogWarning("Prefab tidak ditemukan di path: " + path);
            }
        }
        else
        {
            Debug.LogWarning("UserData tidak mengandung key 'currentChar'");
        }
    },
    error =>
    {
        Debug.LogError("Gagal ambil UserData: " + error.GenerateErrorReport());
    });
    }

    public void changeCharacter(string idChar)
    {
        CheckSession();
        var updateUserDataRequest = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                        {
                            { "currentChar", idChar}
                        }
        };

        PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
            updateResult =>
            {
                Debug.Log($"Character diperbarui ke: {idChar}");
                getCurrentChar();
            }, error => Debug.LogError("Gagal update character: " + error.GenerateErrorReport()));
    }

    void CheckSession()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("deviceSession"))
            {
                string sessionFromServer = result.Data["deviceSession"].Value;

                if (sessionFromServer != currentSessionId)
                {
                    Debug.LogWarning("Session tidak valid. User login dari device lain.");
                    SceneManager.LoadScene("Main Menu");
                }
            }
        },
        error => Debug.LogError("Gagal ambil session: " + error.GenerateErrorReport()));
    }
}
