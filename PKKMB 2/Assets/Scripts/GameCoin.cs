using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using TMPro;

public class GameCoin : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    void Start()
    {
        GetCoinBalance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCoinBalance()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => {
                if (result.VirtualCurrency.ContainsKey("CO"))
                {
                    int coinBalance = result.VirtualCurrency["CO"];
                    Debug.Log("Coin balance: " + coinBalance);

                    if (coinText != null)
                    {
                        coinText.text = coinBalance.ToString();
                    }
                }
                else
                {
                    Debug.LogWarning("Mata uang 'CO' tidak ditemukan di inventaris.");
                    if (coinText != null)
                    {
                        coinText.text = "0";
                    }
                }
            },
            error => {
                Debug.LogError("Gagal mengambil koin: " + error.GenerateErrorReport());
            });
    }
}
