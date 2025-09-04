using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for Button functionality
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using Mapbox.Unity.Map; // Required for Mapbox
using Mapbox.Utils; // Required for Mapbox
using Mapbox.Unity.MeshGeneration.Factories; // Required for Mapbox

// --- The Quest data classes are defined here for clarity ---
[Serializable]
public class Questaw
{
    // Make sure these property names match the JSON from PlayFab
    public string QuestId;
    public string Destination;
    public string buildingId;
}

[Serializable]
public class QuestGroup
{
    public int group;
    public List<Questaw> quests;
}

[Serializable]
public class QuestGroupWrapper
{
    public List<QuestGroup> groups;
}


public class PlayerTask : MonoBehaviour
{
    [Header("Quest UI Title")]
    public TextMeshProUGUI title1;
    public TextMeshProUGUI title2;
    public TextMeshProUGUI title3;

    [Header("Quest UI Description")]
    public TextMeshProUGUI description1;
    public TextMeshProUGUI description2;
    public TextMeshProUGUI description3;

    [Header("Quest Start Buttons")]
    public GameObject start1;
    public GameObject start2;
    public GameObject start3;

    // --- References for routing and scene objects ---
    private AbstractMap map;
    private Transform player;
    private DirectionsFactory directionsFactory;
    private List<BuildingTrigger> sceneBuildings;

    private Dictionary<int, QuestGroup> questCache = new();
    private string questKey = "QuestDatabase";


    private void Awake()
    {
        // Find essential routing components automatically
        map = FindObjectOfType<AbstractMap>();
        GameObject playerObj = GameObject.Find("PlayerTarget");
        if (playerObj != null) player = playerObj.transform;
        directionsFactory = FindObjectOfType<DirectionsFactory>();

        if (map == null || player == null || directionsFactory == null)
            Debug.LogError("Routing component (Map, Player, or DirectionsFactory) not found!");
    }

    private void Start()
    {
        // Scan the scene for all buildings with a BuildingTrigger script
        sceneBuildings = new List<BuildingTrigger>(FindObjectsOfType<BuildingTrigger>());
        Debug.Log($"Found {sceneBuildings.Count} buildings in the scene.");
        
        LoadQuestDatabase();
    }

    // Fetches the entire quest database from PlayFab Title Data
    private void LoadQuestDatabase()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey(questKey))
                {
                    string rawJson = result.Data[questKey];
                    try
                    {
                        var wrapper = JsonUtility.FromJson<QuestGroupWrapper>("{\"groups\":" + rawJson + "}");
                        foreach (var group in wrapper.groups)
                        {
                            if (!questCache.ContainsKey(group.group))
                            {
                                questCache.Add(group.group, group);
                            }
                        }
                        CheckGroupNumber(); // After loading, check the player's group
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to parse Quest JSON: " + e.Message);
                    }
                }
            },
            error => Debug.LogError("Failed to get quest data: " + error.GenerateErrorReport()));
    }

    // Fetches the current player's group number from their User Data
    private void CheckGroupNumber()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("GroupNumber"))
                {
                    string group = result.Data["GroupNumber"].Value;
                    if (int.TryParse(group, out int groupNumber) && questCache.ContainsKey(groupNumber))
                    {
                        ApplyQuestToUI(questCache[groupNumber].quests);
                    }
                }
            },
            error => Debug.LogError("Failed to get UserData: " + error.GenerateErrorReport()));
    }

    // Applies the quest data to the UI and sets up the buttons
    private void ApplyQuestToUI(List<Questaw> quests)
    {
        if (quests == null) return;

        // Setup Quest 1
        if (quests.Count > 0)
        {
            title1.text = quests[0].Destination;
            description1.text = "Find the " + quests[0].Destination + " to complete your quest";
            start1.SetActive(true);

            Button button1 = start1.GetComponent<Button>();
            if (button1 != null)
            {
                button1.onClick.RemoveAllListeners();
                string quest1BuildingId = quests[0].buildingId;
                button1.onClick.AddListener(() => ShowRouteToBuilding(quest1BuildingId));
            }
        }

        // Setup Quest 2
        if (quests.Count > 1)
        {
            title2.text = quests[1].Destination;
            description2.text = "Find the " + quests[1].Destination + " to complete your quest";
            start2.SetActive(true);

            Button button2 = start2.GetComponent<Button>();
            if (button2 != null)
            {
                button2.onClick.RemoveAllListeners();
                string quest2BuildingId = quests[1].buildingId;
                button2.onClick.AddListener(() => ShowRouteToBuilding(quest2BuildingId));
            }
        }

        // Setup Quest 3
        if (quests.Count > 2)
        {
            title3.text = quests[2].Destination;
            description3.text = "Find the " + quests[2].Destination + " to complete your quest";
            start3.SetActive(true);
            
            Button button3 = start3.GetComponent<Button>();
            if (button3 != null)
            {
                button3.onClick.RemoveAllListeners();
                string quest3BuildingId = quests[2].buildingId;
                button3.onClick.AddListener(() => ShowRouteToBuilding(quest3BuildingId));
            }
        }
    }
    
    // Finds the building by ID and displays the route
    public void ShowRouteToBuilding(string targetBuildingId)
    {
        Debug.Log("Attempting to show route to building ID: " + targetBuildingId);

        if (map == null || player == null || directionsFactory == null)
        {
            Debug.LogError("A reference for routing is missing! Cannot create route.");
            return;
        }

        BuildingTrigger targetBuilding = null;
        
        // Find the building in the scene that has the matching ID
        foreach (var building in sceneBuildings)
        {
            if (building.buildingId == targetBuildingId)
            {
                targetBuilding = building;
                break;
            }
        }

        if (targetBuilding != null)
        {
            // Use the found building's position for the route
            Vector3 targetPosition = targetBuilding.transform.position;
            Vector2d playerCoord = map.WorldToGeoPosition(player.position);
            Vector2d buildingCoord = map.WorldToGeoPosition(targetPosition);

            directionsFactory.ShowRoute(playerCoord, buildingCoord);
        }
        else
        {
            Debug.LogError("Could not find a BuildingTrigger in the scene with ID: " + targetBuildingId);
        }
    }
}