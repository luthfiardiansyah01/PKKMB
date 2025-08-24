namespace Mapbox.Unity.MeshGeneration.Factories
{
    using UnityEngine;
    using Mapbox.Directions;
    using System.Collections.Generic;
    using System.Linq;
    using Mapbox.Unity.Map;
    using Data;
    using Modifiers;
    using Mapbox.Utils;
    using Mapbox.Unity.Utilities;
    using System.Collections;

    public class DirectionsFactory : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _map;

        [SerializeField]
        MeshModifier[] MeshModifiers;
        [SerializeField]
        Material _material;

        [SerializeField]
        Transform[] _waypoints;
        private List<Vector3> _cachedWaypoints;

        // ✅ BARU: Tambahkan variabel ini untuk mengatur seberapa jauh waypoint harus bergerak
        // sebelum rute dihitung ulang. Atur nilainya di Inspector.
        [Header("Update Settings")]
        [SerializeField]
        [Tooltip("Jarak minimum (dalam meter Unity) waypoint harus bergerak sebelum rute dihitung ulang.")]
        private float _recalculationThreshold = 10f;

        private Directions _directions;
        private int _counter;
        GameObject _directionsGO;

        // ✅ BARU: Flag untuk mencegah query ganda dalam satu frame
        private bool _isQuerying = false; 

        protected virtual void Awake()
        {
            if (_map == null)
            {
                _map = FindObjectOfType<AbstractMap>();
            }
            _directions = MapboxAccess.Instance.Directions;
        }

        public void Start()
        {
            // Inisialisasi cache posisi awal
            _cachedWaypoints = new List<Vector3>(_waypoints.Length);
            foreach (var item in _waypoints)
            {
                _cachedWaypoints.Add(item.position);
            }

            foreach (var modifier in MeshModifiers)
            {
                modifier.Initialize();
            }

            // Panggil Query() sekali di awal untuk menampilkan rute pertama
            Query();
        }

        // Hapus OnDestroy jika tidak menggunakan event _map.OnInitialized atau _map.OnUpdated
        // protected virtual void OnDestroy() { ... }

        // ✅ BARU: Fungsi Update untuk mengecek pergerakan waypoint
        void Update()
        {
            // Jangan lakukan pengecekan jika sedang dalam proses query
            if (_isQuerying)
            {
                return;
            }

            bool needsRecalculation = false;
            for (int i = 0; i < _waypoints.Length; i++)
            {
                // Cek jika jarak antara posisi sekarang dan posisi tersimpan melebihi ambang batas
                if (Vector3.Distance(_waypoints[i].position, _cachedWaypoints[i]) > _recalculationThreshold)
                {
                    needsRecalculation = true;
                    break; // Cukup satu waypoint bergerak untuk memicu kalkulasi ulang
                }
            }

            if (needsRecalculation)
            {
                Query();
            }
        }

        void Query()
        {
            _isQuerying = true; // Tandai bahwa kita sedang memulai query

            var count = _waypoints.Length;
            var wp = new Vector2d[count];
            for (int i = 0; i < count; i++)
            {
                wp[i] = _waypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
                // Perbarui cache dengan posisi terbaru saat kita membuat query
                _cachedWaypoints[i] = _waypoints[i].position;
            }
            var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
            _directionResource.Steps = true;
            _directions.Query(_directionResource, HandleDirectionsResponse);
        }

        // Hapus Coroutine QueryTimer() karena sudah digantikan oleh Update()
        // public IEnumerator QueryTimer() { ... }

        void HandleDirectionsResponse(DirectionsResponse response)
        {
            if (response == null || null == response.Routes || response.Routes.Count < 1)
            {
                _isQuerying = false; // Query gagal, izinkan query berikutnya
                return;
            }

            var meshData = new MeshData();
            var dat = new List<Vector3>();
            foreach (var point in response.Routes[0].Geometry)
            {
                dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
            }

            var feat = new VectorFeatureUnity();
            feat.Points.Add(dat);

            foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
            {
                mod.Run(feat, meshData, _map.WorldRelativeScale);
            }

            CreateGameObject(meshData);
            _isQuerying = false; // Query selesai, izinkan query berikutnya
        }

        GameObject CreateGameObject(MeshData data)
        {
            if (_directionsGO != null)
            {
                // Gunakan DestroyImmediate jika ada potensi dipanggil dari editor
                // Tapi di runtime, Destroy() lebih baik.
                 Destroy(_directionsGO);
            }
            _directionsGO = new GameObject("direction waypoint " + " entity");
            var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
            mesh.subMeshCount = data.Triangles.Count;

            mesh.SetVertices(data.Vertices);
            _counter = data.Triangles.Count;
            for (int i = 0; i < _counter; i++)
            {
                var triangle = data.Triangles[i];
                mesh.SetTriangles(triangle, i);
            }

            _counter = data.UV.Count;
            for (int i = 0; i < _counter; i++)
            {
                var uv = data.UV[i];
                mesh.SetUVs(i, uv);
            }

            mesh.RecalculateNormals();
            _directionsGO.AddComponent<MeshRenderer>().material = _material;
            return _directionsGO;
        }

        public void ShowRoute(Vector2d start, Vector2d end)
        {
            var wp = new Vector2d[2];
            wp[0] = start;
            wp[1] = end;

            var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
            _directionResource.Steps = true;
            _directions.Query(_directionResource, HandleDirectionsResponse);
        }
    }
}