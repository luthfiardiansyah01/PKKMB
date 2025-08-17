using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class HistoryPageScript : MonoBehaviour
{
    [System.Serializable]
    public class BuildingData
    {
        public string id;
        public string formal;
        public string location;
    }

    [System.Serializable]
    public class BuildingDataWrapper
    {
        public List<BuildingData> buildings;
    }

    public GameObject itemPrefab;
    public Transform contentParent;

    private List<BuildingData> buildingLocations = new();
    private List<string> unlockedBuildingIds = new();

    void Start()
    {
        AddUnlockBuilding("10");
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
                    Data = new Dictionary<string, string>
                    {
                        { "unlockBuilding", updatedData }
                    }
                };

                PlayFabClientAPI.UpdateUserData(updateRequest,
                    updateResult =>
                    {
                        Debug.Log("Building data updated: " + updatedData);
                        GetUnlockBuilding();
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

                GetDataBuilding(); // Lanjut ke data building
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

                    try
                    {
                        var wrapper = JsonUtility.FromJson<BuildingDataWrapper>(rawJson);
                        buildingLocations = wrapper.buildings;
                        Debug.Log("Total building loaded: " + buildingLocations.Count);

                        BuildHistoryUI();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Gagal parse JSON: " + e.Message);
                    }
                }
                else
                {
                    Debug.Log("Tidak ada data BuildingLocation.");
                }
            },
            error => Debug.LogError("Gagal ambil data gedung: " + error.GenerateErrorReport()));
    }

    #endregion

    #region --- UI Builder ---

    void BuildHistoryUI()
    {
        foreach (var data in buildingLocations)
        {
            if (!unlockedBuildingIds.Contains(data.id))
                continue;

            GameObject item = Instantiate(itemPrefab, contentParent);

            // Nama Gedung
            var namaGedungText = item.transform.Find("NamaGedung")?.GetComponent<TextMeshProUGUI>();
            if (namaGedungText != null)
                namaGedungText.text = data.formal;

            // Gambar
            var buildingImage = item.transform.Find("Image")?.GetComponent<Image>();
            if (buildingImage != null)
            {
                string imageName = data.formal.Replace(" ", "").ToLowerInvariant();
                Sprite sprite = Resources.Load<Sprite>("BuildingImages/" + imageName);
                if (sprite != null)
                    buildingImage.sprite = sprite;
                else
                    Debug.LogWarning("Sprite not found for image: " + imageName);
            }
        }
    }

    #endregion
}
