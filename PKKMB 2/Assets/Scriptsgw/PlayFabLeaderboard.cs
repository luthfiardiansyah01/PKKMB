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

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class PlayFabLeaderboard : MonoBehaviour
{
    public GameObject lbItemPrefab; // Drag prefab LBListItem ke sini
    public Transform lbContentParent; // Tempat prefab ditaruh

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

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login berhasil!");
        GetLeaderboard();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login gagal: " + error.GenerateErrorReport());
    }

    void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PlayerCoins", // Ganti sesuai yang kamu pakai di PlayFab
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnLeaderboardError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        // Hapus isi lama
        foreach (Transform child in lbContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in result.Leaderboard)
        {
            GameObject item = Instantiate(lbItemPrefab, lbContentParent);

            // Isi komponen langsung dari sini
            Transform avatar = item.transform.Find("AvatarImage");
            Transform nameText = item.transform.Find("NameText");
            Transform scoreText = item.transform.Find("ScoreText");

            if (avatar != null)
                avatar.GetComponent<Image>().color = Random.ColorHSV(); // Contoh placeholder avatar

            if (nameText != null)
                nameText.GetComponent<TextMeshProUGUI>().text = entry.DisplayName ?? "Guest";

            if (scoreText != null)
                scoreText.GetComponent<TextMeshProUGUI>().text = entry.StatValue.ToString();
        }
    }

    void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Gagal ambil leaderboard: " + error.GenerateErrorReport());
    }
}


