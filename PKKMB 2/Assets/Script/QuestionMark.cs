using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuestionMark : MonoBehaviour
{
    private string buildingId;
    BuildingTrigger trigger;

    // Panel Info
    private Button mulaiQuizButton;
    private GameObject infoPanel;
    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI namaGedung2;
    private TextMeshProUGUI infoGedung;
    private Image imageGedung;

    // Toggle label FindAround
    private TextMeshProUGUI listAround;
    private TextMeshProUGUI listAround2;
    private TextMeshProUGUI listAround3;

    public Button ButtonStartQuiz;
    public Image DoneQuiz;
    public TextMeshProUGUI StartText;
    private string currentSessionId;

    private string findTheBuildingTemplate =
    "Look around {BUILDING_NAME}, check the box below that you think is correct. Submit your answer to earn bonus points!";

    private void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;

        // Ambil BuildingTrigger dari parent
        trigger = GetComponentInParent<BuildingTrigger>();
        if (trigger != null)
        {
            buildingId = trigger.buildingId;
            CheckQuizStatus();
        }
        else
        {
            Debug.LogError("❌ QuestionMark tidak menemukan BuildingTrigger di parent!");
        }

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

                namaGedung2 = infoPanel.transform.Find("FindTheBuildingSection/Deskripsi Quiz").GetComponent<TextMeshProUGUI>();
                listAround = infoPanel.transform.Find("FindTheBuildingSection/Toggle/Label").GetComponent<TextMeshProUGUI>();
                listAround2 = infoPanel.transform.Find("FindTheBuildingSection/Toggle2/Label").GetComponent<TextMeshProUGUI>();
                listAround3 = infoPanel.transform.Find("FindTheBuildingSection/Toggle3/Label").GetComponent<TextMeshProUGUI>();
                break;
            }
        }

        // Pastikan panel Info awalnya tidak aktif
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.buildingCache.ContainsKey(buildingId))
        {
            BuildingData targetBuilding = GameManager.Instance.buildingCache[buildingId];

            // Isi panel Info sesuai gedung
            namaGedung.text = targetBuilding.name;
            infoGedung.text = targetBuilding.description;
            namaGedung2.text = findTheBuildingTemplate.Replace("{BUILDING_NAME}", targetBuilding.name);
            SetGedungImage(targetBuilding.id);

            // Cek status kuis
            ButtonStartQuiz.onClick.RemoveAllListeners();
            ButtonStartQuiz.onClick.AddListener(CheckQuizStatus);

            // 🔎 Ambil data FindAround dari cache
            FindAroundSet findAroundSet = FindAroundBuilding.Instance.GetFindAroundByBuilding(buildingId);
            if (findAroundSet != null && findAroundSet.quests != null && findAroundSet.quests.Count > 0)
            {
                var quest = findAroundSet.quests[0]; // Ambil quest pertama

                if (quest.options.Count > 0) listAround.text = quest.options[0];
                if (quest.options.Count > 1) listAround2.text = quest.options[1];
                if (quest.options.Count > 2) listAround3.text = quest.options[2];

                Debug.Log($"📝 Options untuk {buildingId}: {string.Join(", ", quest.options)}");
                Debug.Log($"✅ Answer untuk {buildingId}: {string.Join(", ", quest.answer)}");
            }
            else
            {
                Debug.LogWarning($"⚠ Tidak ada data FindAround untuk buildingId={buildingId}");
            }

            // Tampilkan panel
            infoPanel.SetActive(true);

            Debug.Log($"📌 Question mark diklik -> tampilkan info {buildingId}");
        }
        else
        {
            Debug.LogError($"❌ BuildingId {buildingId} tidak ditemukan di GameManager.buildingCache");
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
            Debug.LogWarning("⚠ Gambar tidak ditemukan untuk ID: " + buildingId);
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

                // Cek apakah ID gedung ini sudah selesai
                if (completedList.Contains(buildingId))
                {
                    isCompleted = true;
                }
            }

            UpdateQuizButtonUI(isCompleted);

        },
        error =>
        {
            Debug.LogError("❌ Gagal memeriksa status kuis: " + error.GenerateErrorReport());
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
                    Debug.LogWarning("⚠ Session tidak valid. User login dari device lain.");
                    SceneManager.LoadScene("Main Menu");
                }
            }
        },
        error => Debug.LogError("❌ Gagal ambil session: " + error.GenerateErrorReport()));
    }

    void UpdateQuizButtonUI(bool isCompleted)
    {
        if (isCompleted)
        {
            StartText.text = "Done";
            ButtonStartQuiz.interactable = false;
        }
        else
        {
            StartText.text = "Start";
            ButtonStartQuiz.interactable = true;
        }
    }
}
