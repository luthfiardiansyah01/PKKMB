using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Factories;

public class ListManager : MonoBehaviour
{
    // --- UI & PREFAB REFERENCES (ASSIGN IN INSPECTOR) ---
    [Header("UI & Prefab References")]
    public GameObject listItemPrefab;
    public Transform contentParent;
    public GameObject seachPanel;

    // --- PRIVATE REFERENCES FOR ROUTING ---
    private AbstractMap map;
    private Transform player;
    private DirectionsFactory directionsFactory;

    // --- DATA & SCENE OBJECTS ---
    // CHANGE 1: The list is now of your existing 'BuildingTrigger' type.
    private List<BuildingTrigger> sceneBuildings; 
    private List<ItemData> allItems; // The "master list" of all items loaded from JSON

    #region Unity Methods

    private void Awake()
    {
        // Find essential scene objects automatically
        map = FindObjectOfType<AbstractMap>();
        if (map == null)
            Debug.LogError("[ListManager] Cannot find AbstractMap in the scene!");

        GameObject playerObj = GameObject.Find("PlayerTarget");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[ListManager] Cannot find a GameObject named 'PlayerTarget' in the scene!");

        directionsFactory = FindObjectOfType<DirectionsFactory>();
        if (directionsFactory == null)
            Debug.LogError("[ListManager] Cannot find DirectionsFactory in the scene!");
    }

    void Start()
    {
        // We will now call our new refresh function here as well.
        RefreshBuildingList();

        // Load the data from JSON and create the initial list
        LoadItemsAndPopulateList();
    }

// --- NEW PUBLIC FUNCTION ---
    public void RefreshBuildingList()
    {
        // This line scans the scene for all building triggers.
        sceneBuildings = new List<BuildingTrigger>(FindObjectsOfType<BuildingTrigger>());
        Debug.Log($"Refreshed list. Found {sceneBuildings.Count} buildings in the scene.");
    }
    #endregion

    #region Core List Logic

    void LoadItemsAndPopulateList()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("items");

        if (jsonFile != null)
        {
            ItemCollection collection = JsonUtility.FromJson<ItemCollection>(jsonFile.text);
            
            this.allItems = collection.itemList; 
            
            PopulateList(this.allItems);
        }
        else
        {
            Debug.LogError("Cannot find items.json file in the Assets/Resources folder!");
        }
    }

    void PopulateList(List<ItemData> itemsToDisplay)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in itemsToDisplay)
        {
            GameObject newListItem = Instantiate(listItemPrefab, contentParent);
            ListItemController controller = newListItem.GetComponent<ListItemController>();
            
            controller.Setup(item, OnItemSelected);
        }
    }

    #endregion

    #region UI Interaction Logic

    public void FilterList(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            PopulateList(allItems);
            return;
        }

        List<ItemData> filteredItems = new List<ItemData>();

        foreach (var item in allItems)
        {
            if (item.name.ToLower().Contains(searchText.ToLower()) || 
                item.formal.ToLower().Contains(searchText.ToLower()))
            {
                filteredItems.Add(item);
            }
        }
        
        PopulateList(filteredItems);
    }

    // In ListManager.cs

// In ListManager.cs

    void OnItemSelected(ItemData item)
    {
        Debug.Log("Clicked on: " + item.name + " (ID: " + item.id + ")");

        if (map == null || player == null || directionsFactory == null)
        {
            Debug.LogError("A reference for routing is missing! Cannot create route.");
            return;
        }

        BuildingTrigger targetTrigger = null;
        
        // Find the building trigger component in the scene
        foreach (var building in sceneBuildings)
        { 
            if (building.buildingId == item.id.ToString())
            {
                targetTrigger = building;
                break;
            }
        }

        if (targetTrigger != null)
        {
            // --- THIS IS THE ONLY LINE THAT CHANGES ---
            // Get the position of the PARENT object of the trigger.
            Vector3 targetPosition = targetTrigger.transform.parent.position;

            // Use that parent position for the route
            Vector2d playerCoord = map.WorldToGeoPosition(player.position);
            Vector2d buildingCoord = map.WorldToGeoPosition(targetPosition);

            directionsFactory.ShowRoute(playerCoord, buildingCoord);
            
            if(seachPanel != null)
            {
                seachPanel.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Could not find a BuildingTrigger in the scene with ID: " + item.id);
        }
    }
    #endregion
}