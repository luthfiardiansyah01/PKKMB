using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindAroundBuilding : MonoBehaviour
{
    private string currentSessionId;
    public string idFindAround;
    // Start is called before the first frame update
    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        CheckSession();
        if (QuestionMarkManager.Instance != null)
        {
            idFindAround = QuestionMarkManager.Instance.currentBuildingId;
            Debug.Log("📌 Quiz untuk gedung: " + idFindAround);
        }
        else
        {
            Debug.LogWarning("⚠ QuestionMarkManager tidak ditemukan, IdQuest kosong.");
        }
    }

    void Update()
    {

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
