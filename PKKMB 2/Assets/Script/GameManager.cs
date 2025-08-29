using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Dictionary<string, BuildingData> buildingCache = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadBuildingData();
    }

    public void RefreshScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void LoadBuildingData()
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
                        foreach (var building in wrapper.buildings)
                        {
                            if (!buildingCache.ContainsKey(building.id))
                            {
                                buildingCache.Add(building.id, building);
                            }
                        }
                        Debug.Log("total buildingCache: " + buildingCache.Count);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Gagal parse JSON: " + e.Message);
                    }
                }
                else
                {
                    Debug.Log("Tidak ada data gedung di PlayFab");
                }
            },
            error => Debug.LogError("Gagal ambil data: " + error.GenerateErrorReport()));
    }
}