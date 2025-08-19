using UnityEngine;
using TMPro;

public class BuildingTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId;
    private GameObject infoPanel;

    private bool hasBeenTriggered = false;

    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI infoGedung;

    void Start()
    {
        GameObject[] allImages = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allImages)
        {
            if (panel.name == "Info")
            {
                infoPanel = panel;

                namaGedung = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                infoGedung = infoPanel.transform.Find("InfoGedung").GetComponent<TextMeshProUGUI>();
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player")) return;

        if (GameManager.Instance != null && GameManager.Instance.buildingCache.ContainsKey(buildingId))
        {
            BuildingData targetBuilding = GameManager.Instance.buildingCache[buildingId];

            namaGedung.text = targetBuilding.name;
            infoGedung.text = targetBuilding.description;

            Debug.Log($"Menampilkan info gedung: {targetBuilding.name}");

            hasBeenTriggered = true;
            if (infoPanel != null) infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Data untuk BuildingID {buildingId} tidak ditemukan.");
        }
    }
}