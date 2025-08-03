using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class BuildingLoc : MonoBehaviour
{
    List<BuildingData> buildingLocations = new List<BuildingData>();

    void Start()
    {
        Login();
    }

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

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailed);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login berhasil!");
        GetDataBuilding();
        AddUnlockBuilding("7");
    }

    void OnLoginFailed(PlayFabError error)
    {
        Debug.LogError("Gagal login: " + error.GenerateErrorReport());
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
                var buildingCount = buildingLocations.Count;
                // hitung building
                Debug.Log("total building: " + buildingCount);

                foreach (var building in buildingLocations)
                {
                    Debug.Log($"ID: {building.id}, Name: {building.name}, Location: {building.location}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Gagal parse JOSN: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Tidak ada data");
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

                List<string> buildingList = new List<string>(currentData.Split(','));

                if (!buildingList.Contains(newBuildingId))
                {
                    buildingList.Add(newBuildingId);
                }

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
    
}