using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class BuildingData
{
    public string id;
    public string name;
    public string description;
    public string url;
}

[System.Serializable]
public class BuildingDataWrapper
{
    public List<BuildingData> buildings;
}

public class BuildingTrigger : MonoBehaviour
{
    public string buildingId;
    private GameObject infoPanel;

    private bool hasBeenTriggered = false;
    private bool dataLoaded = false; // 🔹 Flag untuk menandakan data sudah siap

    private List<BuildingData> buildingLocations = new();
    private List<string> unlockedBuildingIds = new();

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

                // Ambil komponen text dari child
                namaGedung = infoPanel.transform.Find("JudulGedung").GetComponent<TextMeshProUGUI>();
                infoGedung = infoPanel.transform.Find("InfoGedung").GetComponent<TextMeshProUGUI>();

                if (namaGedung.GetComponent<LinkOpener>() == null)
                {
                    namaGedung.gameObject.AddComponent<LinkOpener>();
                }
                break;
            }
        }
        GetInfoBuilding();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 🔹 Cek apakah data sudah dimuat dan belum pernah trigger sebelumnya
        if (!dataLoaded || hasBeenTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        // Cari data gedung yang sesuai ID
        BuildingData targetBuilding = buildingLocations.Find(b => b.id == buildingId);
        if (targetBuilding != null)
        {
            // Set text panel
            namaGedung.text = $"<link={targetBuilding.url}><u>{targetBuilding.name}</u></link>";
            infoGedung.text = targetBuilding.description;

            Debug.Log($"✔️ Menampilkan info gedung: {targetBuilding.name}");
            hasBeenTriggered = true;
            // Aktifkan panel
            if (infoPanel != null)
            {
                infoPanel.SetActive(true);
                Debug.Log("Panel Info aktif");
            }
            else
            {
                Debug.LogWarning("Panel Info tidak ditemukan di scene!");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Data gedung dengan ID {buildingId} tidak ditemukan.");
        }


    }

    void GetInfoBuilding()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("BuildingLocation"))
                {
                    string rawJson = result.Data["BuildingLocation"];
                    Debug.Log("Raw JSON: " + rawJson);

                    try
                    {
                        var wrapper = JsonUtility.FromJson<BuildingDataWrapper>(rawJson);
                        buildingLocations = wrapper.buildings;

                        Debug.Log("Total building: " + buildingLocations.Count);
                        foreach (var building in buildingLocations)
                            Debug.Log($"ID: {building.id}, name: {building.name}, description : {building.description}");

                        // 🔹 Tandai data sudah siap
                        dataLoaded = true;
                        Debug.Log("Data gedung berhasil dimuat, trigger sudah aktif!");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Gagal parse JSON: " + e.Message);
                    }
                }
                else
                {
                    Debug.Log("Tidak ada data gedung.");
                }
            },
            error => Debug.LogError("Gagal ambil data: " + error.GenerateErrorReport()));
    }
}
