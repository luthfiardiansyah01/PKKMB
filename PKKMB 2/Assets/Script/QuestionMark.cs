using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionMark : MonoBehaviour
{
    private string buildingId;
    BuildingTrigger trigger;

    // Panel Info
    private GameObject infoPanel;
    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI infoGedung;
    private Image imageGedung;


    private void Start()
    {
        // Ambil BuildingTrigger dari parent (karena QuestionMark adalah child dari building)
        trigger = GetComponentInParent<BuildingTrigger>();
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
                infoGedung = infoPanel.transform.Find("Scroll View/Viewport/Content/InfoGedung").GetComponent<TextMeshProUGUI>();
                imageGedung = infoPanel.transform.Find("ImageGedung").GetComponent<Image>();

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
            // Atur isi panel Info sesuai gedung
            namaGedung.text = targetBuilding.name; // bisa diganti ambil data dari database/manager
            infoGedung.text = targetBuilding.description;
            SetGedungImage(targetBuilding.id);


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


}
