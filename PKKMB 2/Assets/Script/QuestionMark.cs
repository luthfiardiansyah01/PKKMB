using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Mapbox.Json;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting;


public class QuestionMark : MonoBehaviour
{
    private string buildingId;
    BuildingTrigger trigger;

    // Panel Info
    private Button mulaiQuizButton;
    private GameObject infoPanel;
    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI infoGedung;
    private Image imageGedung;

    public Button ButtonStartQuiz;
    public Image DoneQuiz;
    public TextMeshProUGUI StartText;
    private string currentSessionId;

    private void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        // Ambil BuildingTrigger dari parent (karena QuestionMark adalah child dari building)
        trigger = GetComponentInParent<BuildingTrigger>();
        if (trigger != null)
        {
            buildingId = trigger.buildingId;
            CheckQuizStatus();
        }
        else
        {
            Debug.LogError("QuestionMarkButton tidak menemukan BuildingTrigger di parent!");
        }
        // if (mulaiQuizButton != null)
        // {
        //     mulaiQuizButton.onClick.AddListener(MulaiQuiz);
        // }

        // Cari panel Info
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allObjects)
        {
            if (panel.name == "Info")
            {
                infoPanel = panel;
                namaGedung = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                infoGedung = infoPanel.transform.Find("Scroll View/Viewport/Content/InfoGedung").GetComponent<TextMeshProUGUI>();
                imageGedung = infoPanel.transform.Find("ImageGedung").GetComponent<Image>();
                ButtonStartQuiz = infoPanel.transform.Find("QuizSection/ButtonStartQuiz").GetComponent<Button>();
                StartText = infoPanel.transform.Find("QuizSection/ButtonStartQuiz/StartText").GetComponent<TextMeshProUGUI>();

                break;
            }
        }

        // Pastikan panel Info awalnya tidak aktif
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void MulaiQuiz()
    {
        // simpan id gedung yg sedang dibuka
        QuestionMarkManager.Instance.currentBuildingId = buildingId;

        // pindah ke scene quiz
        // SceneManager.LoadScene("QuizScene");
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.buildingCache.ContainsKey(buildingId))
        {
            BuildingData targetBuilding = GameManager.Instance.buildingCache[buildingId];
            // Atur isi panel Info sesuai gedung
            namaGedung.text = targetBuilding.name; // bisa diganti ambil data dari database/manager
            infoGedung.text = targetBuilding.description;
            SetGedungImage(targetBuilding.id);
            // ButtonStartQuiz.onClick.RemoveAllListeners();
            ButtonStartQuiz.onClick.AddListener(CheckQuizStatus);


            // Tampilkan panel
            infoPanel.SetActive(true);

            Debug.Log($"Question mark diklik -> tampilkan info {buildingId}");
        }
    }

    private void SetGedungImage(string buildingId)
    {
        Sprite spriteGedung = Resources.Load<Sprite>("BuildingInfoImage/" + buildingId);

        if (spriteGedung != null)
        {
            imageGedung.sprite = spriteGedung;
        }
        else
        {
            Debug.LogWarning("Gambar tidak ditemukan untuk ID: " + buildingId);
        }
    }

    public void CheckQuizStatus()
    {
        CheckSession();

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            bool isCompleted = false;
            if (result.Data != null && result.Data.ContainsKey("completedQuizzes"))
            {
                string currentData = result.Data["completedQuizzes"].Value;
                List<string> completedList = new List<string>(currentData.Split(','));

                // Cek apakah ID gedung ini ada di dalam daftar yang sudah selesai
                if (completedList.Contains(buildingId))
                {
                    isCompleted = true;
                }
            }

            // Update tampilan tombol berdasarkan status
            UpdateQuizButtonUI(isCompleted);

        },
        error =>
        {
            Debug.LogError("Gagal memeriksa status kuis: " + error.GenerateErrorReport());

            // ButtonStartQuiz.interactable = false;
            // ButtonStartQuiz.gameObject.SetActive(false);
            // TextTMP.text = "Error";
        });
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

    void UpdateQuizButtonUI(bool isCompleted)
    {
        if (isCompleted)
        {
            // Jika sudah selesai
            StartText.text = "Done";
            ButtonStartQuiz.interactable = false; // Tombol tidak bisa diklik lagi
        }
        else
        {
            // Jika belum dikerjakan
            StartText.text = "Start";
            ButtonStartQuiz.interactable = true; // Tombol bisa diklik
        }
    }



}
