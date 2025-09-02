using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class Daily5 : MonoBehaviour
{
    private string currentSessionId;
    private string leaderboardName = "Leaderboard";
    private string leaderboardAllTIme = "Leaderboard_AllTime";


    public GameObject NotClaimed_1, Claim_1, Claimed_1;
    public GameObject NotClaimed_2, Claim_2, Claimed_2;
    public GameObject NotClaimed_3, Claim_3, Claimed_3;
    public GameObject NotClaimed_4, Claim_4, Claimed_4;
    public GameObject NotClaimed_5, Claim_5, Claimed_5;
    public GameObject NotClaimed_6, Claim_6, Claimed_6;
    public GameObject NotClaimed_7, Claim_7, Claimed_7;

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        cekDailytoButton();
    }

    void Update()
    {

    }

    public void AddUnlockDaily(string newdailyId, Action callback)
    {
        CheckSession();
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                string currentData = "";
                if (result.Data != null && result.Data.ContainsKey("unlockDaily"))
                {
                    currentData = result.Data["unlockDaily"].Value;
                }

                List<string> dailyList = new List<string>(currentData.Split(','));

                if (!dailyList.Contains(newdailyId))
                {
                    dailyList.Add(newdailyId);
                }

                string updatedData = string.Join(",", dailyList);

                var updateRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                    { "unlockDaily", updatedData }
                    }
                };

                PlayFabClientAPI.UpdateUserData(updateRequest,
                    updateResult =>
                    {
                        Debug.Log("daily data updated: " + updatedData);
                        callback?.Invoke(); // ← yang benar adalah 'callback'
                    },
                    error =>
                    {
                        Debug.LogError("Update failed: " + error.GenerateErrorReport());
                    });
            },
            error =>
            {
                Debug.LogError("Get data failed: " + error.GenerateErrorReport());
            });
    }


    public void getDayNow()
    {
        DateTime now = DateTime.UtcNow;
        CheckSession();
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result =>
            {
                if (result.AccountInfo != null && result.AccountInfo.Created != null)
                {
                    Debug.Log("Account created date : " + result.AccountInfo.Created);
                    DateTime createdDate = result.AccountInfo.Created;
                    Debug.Log("Current date and time: " + ((now - createdDate).Days));
                }
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            }
    );
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
                    SceneManager.LoadScene("LoginScene");
                }
            }
        },
        error => Debug.LogError("Gagal ambil session: " + error.GenerateErrorReport()));
    }

    string idDaily = null;

    public void setDaily(string id)
    {
        idDaily = id;
    }
    public void SubmitScores(int score)
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
                },
                new StatisticUpdate
                {
                    StatisticName = leaderboardAllTIme,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.AddUserVirtualCurrency(new PlayFab.ClientModels.AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "CO",
            Amount = score
        },
        result =>
        {
            PlayFabClientAPI.UpdatePlayerStatistics(statRequest,
            result =>
                {
                    AddUnlockDaily(idDaily, () =>
                    {
                        cekDailytoButton(); // ← ini dipanggil setelah unlock daily berhasil
                    });
                    Debug.Log("Skor berhasil dikirim ke PlayFab!");
                },
                error => Debug.LogError("Gagal kirim skor: " + error.GenerateErrorReport())
            );
            Debug.Log($"Berhasil menambahkan {score} koin. Total koin sekarang: {result.Balance}");
        },
        error => Debug.LogError("Gagal menambahkan koin: " + error.GenerateErrorReport()));
    }


    public void cekDailytoButton()
    {
        CheckSession();

        // Ambil data user (unlockDaily)
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                string currentData = "";
                if (result.Data != null && result.Data.ContainsKey("unlockDaily"))
                {
                    currentData = result.Data["unlockDaily"].Value;
                }

                List<string> dailyList = new List<string>(currentData.Split(','));
                DateTime now = DateTime.UtcNow;

                // Setelah unlockDaily diambil, baru ambil account info (tanggal dibuat)
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
                    accountResult =>
                    {
                        if (accountResult.AccountInfo != null && accountResult.AccountInfo.Created != null)
                        {
                            DateTime createdDate = accountResult.AccountInfo.Created;
                            int currentDay = (now - createdDate).Days + 1;

                            Debug.Log("Account created date: " + createdDate);
                            Debug.Log("Current day since created: " + currentDay);

                            for (int i = 1; i <= 7; i++)
                            {
                                string dayStr = i.ToString();
                                bool sudahKlaim = dailyList.Contains(dayStr);
                                bool hariIni = (currentDay == i);

                                if (sudahKlaim)
                                {
                                    // Sudah diklaim → tampilkan Claimed
                                    SetDailyState(i, false, false, true);
                                }
                                else if (hariIni)
                                {
                                    // Hari ini dan belum diklaim → tampilkan tombol Claim
                                    SetDailyState(i, true, false, false);
                                }
                                else
                                {
                                    // Belum diklaim dan bukan hari ini → tampilkan NotClaimed
                                    SetDailyState(i, true, true, false);
                                }
                            }
                        }
                    },
                    error =>
                    {
                        Debug.LogError("GetAccountInfo failed: " + error.GenerateErrorReport());
                    });
            },
            error =>
            {
                Debug.LogError("GetUserData failed: " + error.GenerateErrorReport());
            });
    }

    // Fungsi bantu untuk mengatur tampilan UI berdasarkan status
    private void SetDailyState(int day, bool claim, bool notClaimed, bool claimed)
    {
        switch (day)
        {
            case 1:
                Claim_1.SetActive(claim);
                NotClaimed_1.SetActive(notClaimed);
                Claimed_1.SetActive(claimed);
                break;
            case 2:
                Claim_2.SetActive(claim);
                NotClaimed_2.SetActive(notClaimed);
                Claimed_2.SetActive(claimed);
                break;
            case 3:
                Claim_3.SetActive(claim);
                NotClaimed_3.SetActive(notClaimed);
                Claimed_3.SetActive(claimed);
                break;
            case 4:
                Claim_4.SetActive(claim);
                NotClaimed_4.SetActive(notClaimed);
                Claimed_4.SetActive(claimed);
                break;
            case 5:
                Claim_5.SetActive(claim);
                NotClaimed_5.SetActive(notClaimed);
                Claimed_5.SetActive(claimed);
                break;
            case 6:
                Claim_6.SetActive(claim);
                NotClaimed_6.SetActive(notClaimed);
                Claimed_6.SetActive(claimed);
                break;
            case 7:
                Claim_7.SetActive(claim);
                NotClaimed_7.SetActive(notClaimed);
                Claimed_7.SetActive(claimed);
                break;
        }
    }


    void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }
}
