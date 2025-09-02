using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mapbox.Examples
{
    [System.Serializable]
    public class Placement
    {
        public string BuildingId;
        public bool nonBuilding;
        public GameObject BuildingPrefab;
        [Geocode]
        public string LocationString;
    }

    public class SpawnOnMap : MonoBehaviour
    {
        public static SpawnOnMap Instance;
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        Placement[] _placements;

        List<GameObject> _spawnedObjects;
        Vector2d[] _locations;


        void Start()
        {
            _spawnedObjects = new List<GameObject>();
            _locations = new Vector2d[_placements.Length];

            // Jalankan proses spawn dengan delay
            StartCoroutine(SpawnWithDelay());
        }

       private IEnumerator SpawnWithDelay()
        {
            yield return new WaitForSeconds(4f);

            for (int i = 0; i < _placements.Length; i++)
            {
                var placement = _placements[i];

                // Konversi string lokasi ke koordinat
                _locations[i] = Conversions.StringToLatLon(placement.LocationString);

                GameObject instance = Instantiate(placement.BuildingPrefab);

                if (placement.nonBuilding)
                {
                    // GoldenQuest
                    GoldenQuestTrigger trigger = instance.GetComponent<GoldenQuestTrigger>();
                    if (trigger != null)
                    {
                        trigger.buildingId = placement.BuildingId;
                    }
                    else
                    {
                        Debug.LogWarning("Prefab GoldenQuest " + placement.BuildingPrefab.name + " tidak punya GoldenQuestTrigger!");
                    }
                }
                else
                {
                    // Building biasa
                    BuildingTrigger triggerScript = instance.GetComponent<BuildingTrigger>();
                    if (triggerScript != null)
                    {
                        triggerScript.buildingId = placement.BuildingId;
                    }
                    else
                    {
                        Debug.LogWarning("Prefab Building " + placement.BuildingPrefab.name + " tidak punya BuildingTrigger!");
                    }
                }

                _spawnedObjects.Add(instance);

                // Atur posisi awal
                instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
            }
        }

        private void Update()
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
            }
        }
        
    }
    
}
