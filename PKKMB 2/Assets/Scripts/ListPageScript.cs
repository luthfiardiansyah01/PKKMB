using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class ListPageScript : MonoBehaviour
{
    [System.Serializable]
    public class BuildingData
    {
        public string id;
        public string formal;
        public string location;
        public bool isBuilding;
    }

    [System.Serializable]
    public class BuildingDataWrapper
    {
        public List<BuildingData> buildings;
    }

    public GameObject itemPrefab;
    public Transform contentParent;
    public TextMeshProUGUI buildingCounter;

    private List<BuildingData> buildingLocations = new();
    private List<string> unlockedBuildingIds = new();

    void Start()
    {
        GetUnlockBuilding();
    }

    #region --- PlayFab Data Flow ---

    public void AddUnlockBuilding(string newBuildingId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                string currentData = result.Data != null && result.Data.ContainsKey("unlockBuilding")
                    ? result.Data["unlockBuilding"].Value
                    : "";

                var buildingList = new List<string>(currentData.Split(','));

                if (!buildingList.Contains(newBuildingId))
                    buildingList.Add(newBuildingId);

                string updatedData = string.Join(",", buildingList);

                var updateRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string> { { "unlockBuilding", updatedData } }
                };

                PlayFabClientAPI.UpdateUserData(updateRequest,
                    updateResult =>
                    {
                        Debug.Log("Building data updated: " + updatedData);
                    },
                    error => Debug.LogError("Update failed: " + error.GenerateErrorReport()));
            },
            error => Debug.LogError("Get data failed: " + error.GenerateErrorReport()));
    }

    public void GetUnlockBuilding()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("unlockBuilding"))
                {
                    string data = result.Data["unlockBuilding"].Value;
                    unlockedBuildingIds = new List<string>(data.Split(','));
                    Debug.Log("Unlocked Buildings Loaded: " + string.Join(", ", unlockedBuildingIds));
                }
                else
                {
                    unlockedBuildingIds = new List<string>();
                    Debug.Log("No unlockBuilding data found.");
                }

                GetDataBuilding();
            },
            error => Debug.LogError("Failed to get unlocked buildings: " + error.GenerateErrorReport()));
    }

    void GetDataBuilding()
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
                            Debug.Log($"ID: {building.id}, formal: {building.formal}");

                        BuildUI();
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

    #endregion

    #region --- UI Builder ---

    void BuildUI()
    {
        int visitedCount = 0;
        int nonBuilding = 0;

        foreach (var data in buildingLocations)
        {
            if (data.isBuilding == true)
            {
                bool isVisited = unlockedBuildingIds.Contains(data.id);
                SpawnBuildingItem(data, isVisited);
                if (isVisited)
                    visitedCount++;
            }
            else
            {
                nonBuilding++;
                continue;
            }
        }

        buildingCounter.text = $"{visitedCount} / {buildingLocations.Count - nonBuilding} Building";
    }

    void SpawnBuildingItem(BuildingData data, bool isVisited)
    {
        GameObject item = Instantiate(itemPrefab, contentParent);

        // Nama gedung
        var namaGedungText = item.transform.Find("NamaGedung")?.GetComponent<TextMeshProUGUI>();
        if (namaGedungText != null) namaGedungText.text = data.formal;

        // Gambar
        var buildingImage = item.transform.Find("Image")?.GetComponent<Image>();
        if (buildingImage != null)
        {
            string formal = data.formal.Replace(" ", "").ToLowerInvariant();
            Sprite sprite = Resources.Load<Sprite>("BuildingImages/" + formal);
            if (sprite != null)
                buildingImage.sprite = sprite;
            else
                Debug.LogWarning("Sprite not found: " + formal);
        }

        // Overlay
        var darkOverlay = item.transform.Find("DarkOverlay")?.gameObject;
        if (darkOverlay != null)
            darkOverlay.SetActive(!isVisited);
    }

    #endregion
}
