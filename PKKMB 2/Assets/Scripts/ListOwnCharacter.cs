using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; // Tambahkan ini untuk menggunakan Action

public class ListOwnCharacter : MonoBehaviour
{
    private string currentSessionId;
    public GameObject prefabList;
    private List<string> daftarOwnCharId = new List<string>();
    public Transform contentParent;

    private ChangeAvatar changeAvatarScript;

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        changeAvatarScript = FindObjectOfType<ChangeAvatar>();
        GetUserInventoryItems();
    }

    // Update is not needed

    public void UpdateList()
    {
        if (changeAvatarScript == null)
        {
            Debug.LogError("Skrip ChangeAvatar tidak ditemukan");
            return;
        }

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (string itemId in daftarOwnCharId)
        {
            GameObject panelInstance = Instantiate(prefabList, contentParent);

            Button panelButton = panelInstance.GetComponent<Button>();

            if (panelButton != null)
            {
                panelButton.onClick.AddListener(() => changeAvatarScript.changeCharacter(itemId));
            }
            else
            {
                Debug.LogError("Komponen Button tidak ditemukan pada prefab Panel!");
            }

            Transform chTransform = panelInstance.transform.Find("CH");
            if (chTransform != null)
            {
                Image chImage = chTransform.GetComponent<Image>();
                if (chImage != null)
                {
                    string path = $"Character/{itemId}";
                    Sprite characterSprite = Resources.Load<Sprite>(path);
                    if (characterSprite != null)
                    {
                        chImage.sprite = characterSprite;
                    }
                }
            }
        }
    }

    // Tambahkan parameter Action onComplete
    public void GetUserInventoryItems(Action onComplete = null)
    {
        CheckSession();
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            Debug.Log("Inventaris berhasil diambil.");
            daftarOwnCharId.Clear();
            foreach (var item in result.Inventory)
            {
                daftarOwnCharId.Add(item.ItemId);
            }
            UpdateList();
            // Panggil callback setelah list diperbarui
            onComplete?.Invoke();
        },
        error =>
        {
            Debug.LogError("Gagal mengambil inventaris: " + error.GenerateErrorReport());
            onComplete?.Invoke(); // Panggil juga saat error
        });
    }

    // CheckSession() tetap sama
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