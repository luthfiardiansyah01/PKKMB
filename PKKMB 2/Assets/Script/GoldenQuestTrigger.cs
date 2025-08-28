using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldenQuestTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId;  // di-set otomatis dari SpawnOnMap
    private bool playerInZone = false;

    // Referensi ke panel Info
    private GameObject infoPanel;
    private TextMeshProUGUI judulQuest;
    private TextMeshProUGUI deskripsiQuest;
    private Image imageQuest;

    void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allObjects)
        {
            if (panel.name == "Info") // nama panel kamu
            {
                infoPanel = panel;
                judulQuest = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                deskripsiQuest = infoPanel.transform.Find("Scroll View/Viewport/Content/InfoGedung").GetComponent<TextMeshProUGUI>();
                imageQuest = infoPanel.transform.Find("ImageGedung").GetComponent<Image>();
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
        if (!playerInZone) return; // hanya bisa diklik jika player ada di zona

        if (GameManager.Instance != null && GameManager.Instance.buildingCache.ContainsKey(buildingId))
        {
            BuildingData target = GameManager.Instance.buildingCache[buildingId];

            // Set isi panel
            judulQuest.text = target.name;
            deskripsiQuest.text = target.description;
            SetQuestImage(target.id);

            infoPanel.SetActive(true);
            Debug.Log($"GoldenQuest {buildingId} ditekan -> tampilkan panel info");
        }
        else
        {
            Debug.LogWarning("Data untuk GoldenQuest " + buildingId + " tidak ditemukan di cache GameManager!");
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
            Debug.LogWarning("Gambar untuk GoldenQuest " + questId + " tidak ditemukan!");
        }
    }
}
