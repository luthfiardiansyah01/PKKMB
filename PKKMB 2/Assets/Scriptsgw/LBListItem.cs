using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class LBListItem : MonoBehaviour
{
    private string leaderboardName = "Leaderboard";

    [Header("Leaderboard UI")]
    public GameObject lbListPrefab;
    public Transform content;

    [Header("Podium UI")]
    public GameObject podium1;
    public GameObject podium2;
    public GameObject podium3;

    private string currentSessionId;
    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        CheckSession();
        // SubmitScore(10);
        GetLeaderboard();
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

    }

    void OnLoginFailed(PlayFabError error)
    {
        Debug.LogError("Gagal login: " + error.GenerateErrorReport());
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

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }

    void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("=== TOP 10 LEADERBOARD ===");
        Debug.Log($"Total pemain: {result.Leaderboard.Count}");

        // podium1.SetActive(false);
        // podium2.SetActive(false);
        // podium3.SetActive(false);
        DisplayLeaderboard(result.Leaderboard);
    }

    void DisplayLeaderboard(List<PlayerLeaderboardEntry> leaderboardEntries)
    {
        // foreach (Transform child in content)
        // {
        //     Destroy(child.gameObject);
        // }

        // podium1.SetActive(false);
        // podium2.SetActive(false);
        // podium3.SetActive(false);

        for (int i = 0; i < leaderboardEntries.Count && i < 10; i++)
        {
            var entry = leaderboardEntries[i];
            string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;
            // string playerName = string.IsNullOrEmpty(entry.Username) ? entry.PlayFabId : entry.Username;

            if (i == 0)
            {
                // podium1.SetActive(true);
                FillPodium(podium1, playerName, i + 1, entry.StatValue, entry.PlayFabId); // 🟠 DIUBAH
            }
            else if (i == 1)
            {
                podium2.SetActive(true);
                FillPodium(podium2, playerName, i + 1, entry.StatValue, entry.PlayFabId); // 🟠 DIUBAH
                Debug.Log("prinnnnnnn2" + entry.StatValue);
            }
            else if (i == 2)
            {
                podium3.SetActive(true);
                FillPodium(podium3, playerName, i + 1, entry.StatValue, entry.PlayFabId); // 🟠 DIUBAH
                Debug.Log("prinnnnnnn3" + entry.StatValue);

            }
            else
            {
                CreateListItem(entry);
            }

            Debug.Log($"{entry.Position + 1} - {entry.StatValue} - {playerName}");
        }

        if (leaderboardEntries.Count < 2)
        {
            podium2.SetActive(true);
            FillPodium(podium2, "", 2, 0, "");
        }
        if (leaderboardEntries.Count < 3)
        {
            podium3.SetActive(true);
            FillPodium(podium3, "", 3, 0, "");
        }
    }

    // 🟠 DIUBAH: Tambah parameter score
    void FillPodium(GameObject podium, string playerName, int rank, int score, string playFabId)
    {
        Transform usernameTransform = podium.transform.Find("UsernameText");
        Transform poinTransform = podium.transform.Find("ScoreText");
        Transform imageTransform = podium.transform.Find("Image/AvatarImage"); // 🟢 TAMBAHAN

        if (usernameTransform != null)
        {
            Debug.Log($"[Podium {rank}] Dapat komponen UsernameText");
            TextMeshProUGUI usernameText = usernameTransform.GetComponent<TextMeshProUGUI>();
            usernameText.text = playerName;
        }
        else
        {
            Debug.LogWarning($"[Podium {rank}] UsernameText tidak ditemukan.");
        }

        if (poinTransform != null) // 🟢 TAMBAHAN
        {
            TextMeshProUGUI scoreText = poinTransform.GetComponent<TextMeshProUGUI>(); // 🟢 TAMBAHAN
            scoreText.text = score.ToString(); // 🟢 TAMBAHAN
        }
        else
        {
            Debug.LogWarning($"[Podium {rank}] ScoreText tidak ditemukan.");
        }

        if (imageTransform != null)
        {
            Image avatarImage = imageTransform.GetComponent<Image>();
            LoadPlayerAvatar(playFabId, avatarImage);
        }
        else
        {
            Debug.LogWarning("Aduh gimana sih dli gambarnya ga dapet");
        }
    }

    void CreateListItem(PlayerLeaderboardEntry entry)
    {
        GameObject item = Instantiate(lbListPrefab, content);
        item.transform.localScale = Vector3.one;

        string playerName = string.IsNullOrEmpty(entry.DisplayName) ? entry.PlayFabId : entry.DisplayName;
        // string playerName = string.IsNullOrEmpty(entry.Username) ? entry.PlayFabId : entry.Username;

        Transform rankObj = item.transform.Find("RankText");
        if (rankObj != null)
        {
            var rankText = rankObj.GetComponent<TextMeshProUGUI>();
            if (rankText != null)
                rankText.text = (entry.Position + 1).ToString();
        }

        Transform avatarObj = item.transform.Find("AvatarImage");
        if (avatarObj != null)
        {
            var avatarImage = avatarObj.GetComponent<Image>();
            LoadPlayerAvatar(entry.PlayFabId, avatarImage);
        }

        Transform usernameObj = item.transform.Find("UsernameText");
        if (usernameObj != null)
        {
            var usernameText = usernameObj.GetComponent<TextMeshProUGUI>();
            if (usernameText != null)
                usernameText.text = playerName;
        }

        Transform scoreObj = item.transform.Find("ScoreText");
        if (scoreObj != null)
        {
            var scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
            if (scoreText != null)
                scoreText.text = entry.StatValue.ToString();
        }
    }

    void LoadPlayerAvatar(string playFabId, Image avatarImage)
    {
        if (playFabId == "")
        {
            Sprite avatarSprite = Resources.Load<Sprite>($"profile/2D/OkePutih");
            avatarImage.sprite = avatarSprite;
            return;
        }
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = playFabId
        },
        result =>
        {
            if (result.Data != null && result.Data.ContainsKey("currentChar"))
            {
                Debug.Log("Cihuyyyyy");
                string avatarId = result.Data["currentChar"].Value;
                Debug.Log($"[Avatar] PlayFabId: {playFabId}, avatarId: {avatarId}");
                // Debug.Log("Cihuyyyyy");

                Sprite avatarSprite = Resources.Load<Sprite>($"profile/2D/{avatarId}");
                if (avatarSprite != null)
                {
                    avatarImage.sprite = avatarSprite;
                    Debug.Log($"[Avatar] Berhasil load sprite {avatarId}");
                }
                else
                {
                    Debug.LogWarning($"[Avatar] Gagal load sprite: UI Design/Character/2D/{avatarId}");
                }
            }
            else
            {
                Debug.LogWarning($"[Avatar] Tidak ada avatarId untuk {playFabId}");
            }
        },
        error => Debug.LogError("Gagal ambil avatar: " + error.GenerateErrorReport()));
    }


    void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }

    public void RefreshScene()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        GetLeaderboard();
    }

    public void changeToAlltime()
    {
        leaderboardName = "Leaderboard_AllTime";
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        GetLeaderboard();
    }

    public void changeDaily()
    {
        leaderboardName = "Leaderboard";
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        GetLeaderboard();
    }
}
