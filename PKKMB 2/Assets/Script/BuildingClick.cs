using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.MeshGeneration.Factories;

public class BuildingClick : MonoBehaviour
{
    private AbstractMap map;
    private Transform player;
    private DirectionsFactory directionsFactory;

    private void Awake()
    {
        // Cari map
        map = FindObjectOfType<AbstractMap>();
        if (map == null)
            Debug.LogError("[BuildingClick] Tidak menemukan AbstractMap di scene!");

        // Cari player
        GameObject playerObj = GameObject.Find("PlayerTarget");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[BuildingClick] Tidak menemukan PlayerTarget di scene!");

        // Cari DirectionsFactory
        directionsFactory = FindObjectOfType<DirectionsFactory>();
        if (directionsFactory == null)
            Debug.LogError("[BuildingClick] Tidak menemukan DirectionsFactory di scene!");
    }

    private void OnMouseDown()
    {
        if (map == null || player == null || directionsFactory == null)
        {
            Debug.LogError("[BuildingClick] Referensi masih null, tidak bisa membuat rute!");
            return;
        }

        Vector2d playerCoord = map.WorldToGeoPosition(player.position);
        Vector2d buildingCoord = map.WorldToGeoPosition(transform.position);

        directionsFactory.ShowRoute(playerCoord, buildingCoord);
    }
}