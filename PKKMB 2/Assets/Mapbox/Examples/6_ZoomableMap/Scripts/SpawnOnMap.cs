using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

namespace Mapbox.Examples
{
    // KELAS BARU UNTUK MEMASANGKAN DATA (bisa ditaruh di sini atau di file sendiri)
    [System.Serializable]
    public class Placement
    {
        public string BuildingId;
        public GameObject BuildingPrefab;
        [Geocode]
        public string LocationString;

    }

    public class SpawnOnMap : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        Placement[] _placements;

        [SerializeField]
        float _spawnScale = 100f;

        List<GameObject> _spawnedObjects;
        Vector2d[] _locations;

        void Start()
        {
            _spawnedObjects = new List<GameObject>();
            _locations = new Vector2d[_placements.Length];

            for (int i = 0; i < _placements.Length; i++)
            {
                var placement = _placements[i];

                // Konversi string lokasi menjadi koordinat Vector2d
                _locations[i] = Conversions.StringToLatLon(placement.LocationString);

                // Instantiate prefab SPESIFIK untuk placement ini
                var instance = Instantiate(placement.BuildingPrefab); // -> Menggunakan prefab dari data placement
                _spawnedObjects.Add(instance); // Tambahkan ke daftar untuk di-update

                BuildingTrigger triggerScript = instance.GetComponent<BuildingTrigger>();
                if (triggerScript != null)
                {
                    triggerScript.buildingId = placement.BuildingId;
                }
                else
                {
                    Debug.LogWarning("Prefab " + placement.BuildingPrefab.name + " tidak memiliki komponen BuildingTrigger!");
                }

                // Atur posisi dan skala seperti sebelumnya
                instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
                instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
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
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
    }
}