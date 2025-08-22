using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId;
    private GameObject infoPanel;

    private bool hasBeenTriggered = false;

    private TextMeshProUGUI namaGedung;
    private TextMeshProUGUI infoGedung;
    private Image imageGedung;

    void Start()
    {
        GameObject[] allImages = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject panel in allImages)
        {
            if (panel.name == "Info")
            {
                infoPanel = panel;

                namaGedung = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                infoGedung = infoPanel.transform.Find("Scroll View/Viewport/Content/InfoGedung").GetComponent<TextMeshProUGUI>();
                imageGedung = infoPanel.transform.Find("ImageGedung").GetComponent<Image>();

                // Panggil method untuk load gambar sesuai nama gedung
                // SetGedungImage(namaGedung.text);
                // imageGedung = infoPanel.transform.Find("ImageGedung").GetComponent<ImageDataFetcher>;// bagian ini saya mau ambil gambar dari folder Resources/BulidingInfoImage/(namaGedung) nama gambarnya sama dengan nama gedung di database play fab bagaimana cara saya mengaplikasikan pathnya?
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
            SetGedungImage(targetBuilding.id);

            Debug.Log($"Menampilkan info gedung: {targetBuilding.name}");

            hasBeenTriggered = true;
            if (infoPanel != null) infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Data untuk BuildingID {buildingId} tidak ditemukan.");
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