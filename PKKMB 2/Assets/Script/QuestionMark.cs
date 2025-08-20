using UnityEngine;
using TMPro;

public class QuestionMark : MonoBehaviour
{
    private string buildingId;

    // Panel Info
    private GameObject infoPanel;
    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI infoGedung;

    private void Start()
    {
        // Ambil BuildingTrigger dari parent (karena QuestionMark adalah child dari building)
        BuildingTrigger trigger = GetComponentInParent<BuildingTrigger>();
        if (trigger != null)
        {
            buildingId = trigger.buildingId;
        }
        else
        {
            Debug.LogError("QuestionMarkButton tidak menemukan BuildingTrigger di parent!");
        }

        // Cari panel Info
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allObjects)
        {
            if (panel.name == "Info")
            {
                infoPanel = panel;
                namaGedung = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                infoGedung = infoPanel.transform.Find("InfoGedung").GetComponent<TextMeshProUGUI>();
                break;
            }
        }

        // Pastikan panel Info awalnya tidak aktif
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(buildingId) && infoPanel != null)
        {
            // Atur isi panel Info sesuai gedung
            namaGedung.text = buildingId; // bisa diganti ambil data dari database/manager
            infoGedung.text = $"Ini adalah informasi tentang gedung {buildingId}.";

            // Tampilkan panel
            infoPanel.SetActive(true);

            Debug.Log($"Question mark diklik -> tampilkan info {buildingId}");
        }
    }
}
