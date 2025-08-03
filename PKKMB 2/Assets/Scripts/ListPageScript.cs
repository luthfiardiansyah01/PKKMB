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
        public string name;
        public string location;
    }

    [System.Serializable]
    public class BuildingDataWrapper
    {
        public List<BuildingData> buildings;
    }

    public GameObject itemPrefab;
    public Transform contentParent;
    public TextMeshProUGUI buildingCounter;

    private List<BuildingData> buildingLocations = new List<BuildingData>();
    private List<string> unlockedBuildingIds = new List<string>();

    void Start()
    {
        GetDataBuilding();
        AddUnlockBuilding("10");
    }

    void GetDataBuilding()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnTitleDataSuccess, OnTitleDataError);
    }

    void OnTitleDataSuccess(GetTitleDataResult result)
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
                {
                    Debug.Log($"ID: {building.id}, Name: {building.name}, Location: {building.location}");
                }

                TrySpawnBuildingItems(); // jika sudah dapat building dan unlock
            }
            catch (System.Exception e)
            {
                Debug.LogError("Gagal parse JSON: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Tidak ada data");
        }
    }

    void OnTitleDataError(PlayFabError error)
    {
        Debug.LogError("Gagal ambil data: " + error.GenerateErrorReport());
    }

    public void AddUnlockBuilding(string newBuildingId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                string currentData = "";
                if (result.Data != null && result.Data.ContainsKey("unlockBuilding"))
                {
                    currentData = result.Data["unlockBuilding"].Value;
                }

                 unlockedBuildingIds = new List<string>(currentData.Split(','));

                if (!unlockedBuildingIds.Contains(newBuildingId))
                {
                    unlockedBuildingIds.Add(newBuildingId);
                }

                string updatedData = string.Join(",", unlockedBuildingIds);

                Debug.Log("List gedung " + updatedData);

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
                    error =>
                    {
                        Debug.LogError("Update failed: " + error.GenerateErrorReport());
                    });
            },
            error =>
            {
                Debug.LogError("Get data failed: " + error.GenerateErrorReport());
            });
    }
    public void GetUnlockBuilding()
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (result.Data != null && result.Data.ContainsKey("unlockBuilding"))
            {
                string data = result.Data["unlockBuilding"].Value;
                unlockedBuildingIds = new List<string>(data.Split(','));

                Debug.Log("Unlocked Buildings Loaded: " + string.Join(", ", unlockedBuildingIds));

                TrySpawnBuildingItems(); // Langsung tampilkan gedung yang unlocked
            }
            else
            {
                Debug.Log("No unlockBuilding data found, initializing empty list.");
                unlockedBuildingIds = new List<string>();
            }
        },
        error =>
        {
            Debug.LogError("Failed to get unlocked buildings: " + error.GenerateErrorReport());
        });
    }


    // bool hasSpawned = false;

    void TrySpawnBuildingItems()
    {
        // if (hasSpawned) return;
        if (buildingLocations.Count == 0 ) return;

        // hasSpawned = true;

        int visitedCount = 0;

        foreach (var data in buildingLocations)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);


            Image buildingImage = item.transform.Find("Image")?.GetComponent<Image>();
            if (buildingImage != null)
            {
                string imageName = data.name.Replace(" ", "").ToLowerInvariant();
                Sprite sprite = Resources.Load<Sprite>("BuildingImages/" + imageName);

                if (sprite != null)
                {
                    buildingImage.sprite = sprite;
                    Debug.Log("GedungCacuk");
                }
                else
                {
                    Debug.Log("Sprite not found for image: " + imageName);
                }
            }

            // Nama gedung
            var namaGedungText = item.transform.Find("NamaGedung").GetComponent<TextMeshProUGUI>();
            namaGedungText.text = data.name;

            // Status visited
            bool isVisited = unlockedBuildingIds.Contains(data.id);
            Debug.Log(isVisited);

                GameObject darkOverlay = item.transform.Find("DarkOverlay").gameObject;
                darkOverlay.SetActive(!isVisited);
                if (isVisited)
                    visitedCount++;

                buildingCounter.text = visitedCount + " / " + buildingLocations.Count + " Building";
            


        }

    }
}
