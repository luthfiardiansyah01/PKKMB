using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class SendPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private string currentSessionId;
    private string leaderboardName = "Leaderboard";
    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        CheckSession();
    }

    // Update is called once per frame
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

    public void SubmitScore(int score)
    {
        CheckSession();
        var statRequest = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(statRequest,
            result =>
            {
                Debug.Log("Skor berhasil dikirim ke PlayFab!");

                PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onGetUserDataSuccess, OnError);

                void onGetUserDataSuccess(GetUserDataResult getResult)
                {
                    int currentCoin = 0;
                    if (getResult.Data != null && getResult.Data.ContainsKey("coin"))
                    {
                        int.TryParse(getResult.Data["coin"].Value, out currentCoin);
                    }

                    int newTotal = currentCoin + score;

                    var updateUserDataRequest = new UpdateUserDataRequest
                    {
                        Data = new Dictionary<string, string>
                        {
                            { "coin", newTotal.ToString() }
                        }
                    };

                    PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
                        updateResult => Debug.Log($"Coin berhasil diperbarui ke: {newTotal}"),
                        error => Debug.LogError("Gagal update coin: " + error.GenerateErrorReport()));
                }
            },
            error => Debug.LogError("Gagal kirim skor: " + error.GenerateErrorReport())
        );
    }
    
    void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }
}
