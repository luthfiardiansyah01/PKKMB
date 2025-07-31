// using System.Collections.Generic;
// using UnityEngine;
// using PlayFab;
// using PlayFab.ClientModels;

// public class PlayFabLeaderboard : MonoBehaviour
// {
//     public LeaderBoardManager leaderboardManager;
//     private string statisticName = "Leaderboard";

//     public GameObject lbListPrefab; // Prefab list leaderboard
//     public Transform content;    

//     void Start()
//     {
//         Login();
//         // leaderboardManager.UpdateLeaderboard()
//     }

//     void Login()
//     {
//         var request = new LoginWithCustomIDRequest
//         {
//             CustomId = SystemInfo.deviceUniqueIdentifier,
//             CreateAccount = true
//         };

//         PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailed);
//     }

//     void OnLoginSuccess(LoginResult result)
//     {
//         Debug.Log("Login berhasil!");
//         SubmitScore(10);
//         GetLeaderboard();
//     }

//     void OnLoginFailed(PlayFabError error)
//     {
//         Debug.LogError("Gagal login: " + error.GenerateErrorReport());
//     }

//     public void GetLeaderboard()
//     {
//         var request = new GetLeaderboardRequest
//         {
//             StatisticName = statisticName,
//             StartPosition = 0,
//             MaxResultsCount = 10
//         };

//         PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
//     }

//     public void SubmitScore(int score)
//     {
//         var statRequest = new UpdatePlayerStatisticsRequest
//         {
//             Statistics = new List<StatisticUpdate>
//     {
//         new StatisticUpdate
//         {
//             StatisticName = statisticName,
//             Value = score
//         }
//     }
//         };

//         PlayFabClientAPI.UpdatePlayerStatistics(statRequest,
//             result =>
//             {
//                 Debug.Log("Skor berhasil dikirim ke PlayFab!");

//                 PlayFabClientAPI.GetUserData(new GetUserDataRequest(), onGetUserDataSuccess, OnError);

//                 void onGetUserDataSuccess(GetUserDataResult getResult)
//                 {
//                     int currentCoin = 0;
//                     if (getResult.Data != null && getResult.Data.ContainsKey("coin"))
//                     {
//                         int.TryParse(getResult.Data["coin"].Value, out currentCoin);
//                     }

//                     int newTotal = currentCoin + score;

//                     var updateUserDataRequest = new UpdateUserDataRequest
//                     {
//                         Data = new Dictionary<string, string>
//                         {
//                     { "coin", newTotal.ToString() }
//                         }
//                     };

//                     PlayFabClientAPI.UpdateUserData(updateUserDataRequest,
//                         updateResult => Debug.Log($"Coin berhasil diperbarui ke: {newTotal}"),
//                         error => Debug.LogError("Gagal update coin: " + error.GenerateErrorReport()));
//                 }
//             },
//             error => Debug.LogError("Gagal kirim skor: " + error.GenerateErrorReport())
//         );
//     }
//     void OnGetLeaderboardSuccess(GetLeaderboardResult result)
//     {
//         Debug.Log("=== TOP 10 LEADERBOARD ===");
//         Debug.Log($"Total pemain: {result.Leaderboard.Count}");

//         if (result.Leaderboard.Count == 0)
//         {
//             Debug.Log("Leaderboard masih kosong.");
//             return;
//         }

//         foreach (var entry in result.Leaderboard)
//         {
//             string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;
//             Debug.Log($"{entry.Position + 1} - {entry.StatValue} - {playerName}");
//         }
//     }

//     void OnError(PlayFabError error)
//     {
//         Debug.LogError("Gagal ambil leaderboard: " + error.GenerateErrorReport());
//     }
    
// }

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLeaderboard : MonoBehaviour
{
    // public LeaderBoardManager leaderboardManager;
    private string statisticName = "Leaderboard";

    [Header("Leaderboard UI")]
    public GameObject lbListPrefab; // Prefab untuk rank 4–10
    public Transform content;       // Scroll View Content untuk list 4–10

    [Header("Podium UI")]
    public GameObject podium1;
    public GameObject podium2;
    public GameObject podium3;

    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailed);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login berhasil!");
        SubmitScore(10);
        GetLeaderboard();
    }

    void OnLoginFailed(PlayFabError error)
    {
        Debug.LogError("Gagal login: " + error.GenerateErrorReport());
    }

    public void SubmitScore(int score)
    {
        var statRequest = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
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

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);


    }

    void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("=== TOP 10 LEADERBOARD ===");
        Debug.Log($"Total pemain: {result.Leaderboard.Count}");

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            var entry = result.Leaderboard[i+1];
            string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;
            var score = entry.StatValue;

            if (i == 0) FillPodium(podium1, entry);
            else if (i == 1) FillPodium(podium2, entry);
            else if (i == 2) FillPodium(podium3, entry);
            else CreateListItem(entry);

            Debug.Log($"{entry.Position + 1} - {entry.StatValue} - {playerName}");
        }
    }

    void FillPodium(GameObject podium, PlayerLeaderboardEntry entry)
    {
        if (podium == null) return;

        string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;

        Transform usernameObj = podium.transform.Find("UsernameText");
        if (usernameObj != null)
        {
            Text usernameText = usernameObj.GetComponent<Text>();
            if (usernameText != null)
                usernameText.text = playerName;
        }

        Transform scoreObj = podium.transform.Find("ScoreText");
        if (scoreObj != null)
        {
            Text scoreText = scoreObj.GetComponent<Text>();
            if (scoreText != null)
                scoreText.text = entry.StatValue.ToString();
        }

        // Optional Avatar (nonaktifkan untuk sekarang)
        /*
        RawImage avatarImage = podium.GetComponent<RawImage>();
        if (avatarImage != null)
        {
            // Load avatar dari URL jika ada sistem avatar di masa depan
        }
        */
    }

    void CreateListItem(PlayerLeaderboardEntry entry)
    {
        GameObject item = Instantiate(lbListPrefab, content);
        item.transform.localScale = Vector3.one;

        string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;

        Transform rankObj = item.transform.Find("RankText");
        if (rankObj != null)
        {
            Text rankText = rankObj.GetComponent<Text>();
            if (rankText != null)
                rankText.text = (entry.Position + 1).ToString();
        }

        Transform usernameObj = item.transform.Find("UsernameText");
        if (usernameObj != null)
        {
            Text usernameText = usernameObj.GetComponent<Text>();
            if (usernameText != null)
                usernameText.text = playerName;
        }

        Transform scoreObj = item.transform.Find("ScoreText");
        if (scoreObj != null)
        {
            Text scoreText = scoreObj.GetComponent<Text>();
            if (scoreText != null)
                scoreText.text = entry.StatValue.ToString();
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }
}

