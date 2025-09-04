using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GoldenQuestTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId; // diisi otomatis dari SpawnOnMap
    private bool playerInZone = false;

    // Panel Info
    private GameObject infoPanel;
    private TextMeshProUGUI judulQuest;
    private TextMeshProUGUI deskripsiQuest;
    private Image imageQuest;

    // Tambahan: kalau nanti ada mini quiz
    private TextMeshProUGUI namaGedung2;
    private TextMeshProUGUI listAround, listAround2, listAround3;
    private Toggle toggle1, toggle2, toggle3;
    private Button submitButton;
    private TextMeshProUGUI textSubmitButton;

    private string findTheBuildingTemplate =
        "Look around {BUILDING_NAME}, check the box below that you think is correct. Submit your answer to earn bonus points!";

    void Start()
    {
        // Cari panel Info sama seperti QuestionMark
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allObjects)
        {
            if (panel.name == "Info")
            {
                infoPanel = panel;
                judulQuest = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                deskripsiQuest = infoPanel.transform.Find("Scroll View/Viewport/Content/InfoGedung").GetComponent<TextMeshProUGUI>();
                imageQuest = infoPanel.transform.Find("ImageGedung").GetComponent<Image>();

                // Jika quest juga butuh toggle quiz
                namaGedung2 = infoPanel.transform.Find("FindTheBuildingSection/Deskripsi Quiz")?.GetComponent<TextMeshProUGUI>();
                listAround = infoPanel.transform.Find("FindTheBuildingSection/Toggle/Label")?.GetComponent<TextMeshProUGUI>();
                listAround2 = infoPanel.transform.Find("FindTheBuildingSection/Toggle2/Label")?.GetComponent<TextMeshProUGUI>();
                listAround3 = infoPanel.transform.Find("FindTheBuildingSection/Toggle3/Label")?.GetComponent<TextMeshProUGUI>();

                toggle1 = infoPanel.transform.Find("FindTheBuildingSection/Toggle")?.GetComponent<Toggle>();
                toggle2 = infoPanel.transform.Find("FindTheBuildingSection/Toggle2")?.GetComponent<Toggle>();
                toggle3 = infoPanel.transform.Find("FindTheBuildingSection/Toggle3")?.GetComponent<Toggle>();
                submitButton = infoPanel.transform.Find("FindTheBuildingSection/SubmitButton")?.GetComponent<Button>();
                textSubmitButton = infoPanel.transform.Find("FindTheBuildingSection/SubmitButton/TextSubmit")?.GetComponent<TextMeshProUGUI>();

                

                break;
            }
        }

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log("Player masuk ke zona GoldenQuest: " + buildingId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log("Player keluar dari zona GoldenQuest: " + buildingId);
        }
    }

    private void OnMouseDown()
    {
        if (!playerInZone) return; // hanya bisa diklik kalau player ada di zona

        if (GameManager.Instance != null && GameManager.Instance.buildingCache.ContainsKey(buildingId))
        {
            BuildingData target = GameManager.Instance.buildingCache[buildingId];

            // Set isi panel
            judulQuest.text = target.name;
            deskripsiQuest.text = target.description;
            namaGedung2.text = findTheBuildingTemplate.Replace("{BUILDING_NAME}", target.name);

            SetQuestImage(target.id);

            // (Opsional) isi toggle dari FindAroundBuilding.Instance jika ada data
            FindAroundSet findAroundSet = FindAroundBuilding.Instance.GetFindAroundByBuilding(buildingId);
            if (findAroundSet != null && findAroundSet.quests.Count > 0)
            {
                var quest = findAroundSet.quests[0];
                if (quest.options.Count > 0) listAround.text = quest.options[0];
                if (quest.options.Count > 1) listAround2.text = quest.options[1];
                if (quest.options.Count > 2) listAround3.text = quest.options[2];
            }

            if (submitButton != null)
                {
                    submitButton.onClick.RemoveAllListeners();
                    submitButton.onClick.AddListener(SubmitAnswer);
                }

            infoPanel.SetActive(true);
            Debug.Log($"GoldenQuest {buildingId} ditekan -> tampilkan panel info");
        }
        else
        {
            Debug.LogWarning("⚠ Data GoldenQuest " + buildingId + " tidak ditemukan di GameManager!");
        }
    }

    private void SetQuestImage(string questId)
    {
        Sprite sprite = Resources.Load<Sprite>("BuildingInfoImage/" + questId);
        if (sprite != null)
        {
            imageQuest.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("⚠ Gambar untuk GoldenQuest " + questId + " tidak ditemukan!");
        }
    }

    // --- Tambahan: kalau quest ini juga butuh evaluasi jawaban ---
    public void SubmitAnswer()
    {
        FindAroundSet findAroundSet = FindAroundBuilding.Instance.GetFindAroundByBuilding(buildingId);
        Debug.Log("Ini Id golden quest ke sekian - " + buildingId);
        if (findAroundSet == null || findAroundSet.quests.Count == 0) return;

        var quest = findAroundSet.quests[0];
        List<string> selectedAnswers = new List<string>();

        if (toggle1 != null && toggle1.isOn) selectedAnswers.Add(listAround.text);
        if (toggle2 != null && toggle2.isOn) selectedAnswers.Add(listAround2.text);
        if (toggle3 != null && toggle3.isOn) selectedAnswers.Add(listAround3.text);

        List<string> correctAnswers = quest.answer;
        int correctSelected = selectedAnswers.FindAll(a => correctAnswers.Contains(a)).Count;
        int incorrectSelected = selectedAnswers.FindAll(a => !correctAnswers.Contains(a)).Count;
        int totalCorrectAnswersAvailable = correctAnswers.Count;

        int finalScore = 0;
       if (incorrectSelected > 0)
        {
            finalScore = 0;
        }
        // Jika tidak ada kesalahan sama sekali, baru kita cek untuk skor 20 atau 10.
        else
        {
            // Aturan #2: Kondisi Sempurna
            if (correctSelected == totalCorrectAnswersAvailable && correctSelected > 0)
            {
                finalScore = 20;
            }
            // Aturan #3: Kondisi Hanya Satu Jawaban Benar
            else if (correctSelected == 1)
            {
                finalScore = 10;
            }
            // Aturan #4: Kondisi lain (misal: memilih 2 dari 3 benar) skor tetap 0.
        }

        Debug.Log($"📊 Skor dihitung: {finalScore} poin (Benar: {correctSelected}, Salah: {incorrectSelected})");

        // --- Langkah 3: Kirim hasil dan tandai kuis selesai ---
        if (finalScore > 0)
        {
            
        }
        // TODO: Kalau mau, bisa tambahkan SubmitScore ke PlayFab seperti di QuestionMark
    }
}
