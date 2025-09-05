using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections.Generic;
using Mapbox.Unity.Utilities;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FogOfWarController : MonoBehaviour
{
    [Header("Referensi Peta")]
    [SerializeField]
    private AbstractMap _map;

    [Header("Area Terlihat")]
    [Tooltip("Koordinat latitude dan longitude dari pusat area yang terlihat.")]
    [SerializeField]
    private Vector2d _centerCoordinate = new Vector2d(-6.9730, 107.6312); // Contoh: Koordinat Telkom University

    [Tooltip("Radius area yang terlihat dalam satuan meter.")]
    [SerializeField]
    private float _clearRadiusInMeters = 1000f; // Contoh: Radius 1 km

    [Tooltip("Seberapa halus tepi lingkaran (semakin tinggi semakin halus).")]
    [SerializeField]
    private int _segments = 64; // Nama diubah agar lebih jelas

    [Header("Tampilan Kabut")]
    [Tooltip("Material yang digunakan untuk kabut (harus material transparan).")]
    [SerializeField]
    private Material _fogMaterial;

    [Tooltip("Ketinggian kabut di atas peta (sedikit di atas 0).")]
    [SerializeField]
    private float _fogHeight = 0.1f;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        if (_map == null)
        {
            Debug.LogError("Referensi AbstractMap belum di-assign!");
            return;
        }

        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _fogMaterial;

        _map.OnInitialized += GenerateFogMesh;
    }

    private void OnDestroy()
    {
        if (_map != null)
        {
            _map.OnInitialized -= GenerateFogMesh;
        }
    }

    /// <summary>
    /// LOGIKA FUNGSI INI SEPENUHNYA DIPERBARUI
    /// </summary>
    public void GenerateFogMesh()
    {
        // 1. Dapatkan properti dasar peta
        var mapCenterMercator = Conversions.LatLonToMeters(_map.CenterLatitudeLongitude);
        float mapSize = _map.UnityTileSize * Mathf.Pow(2, _map.InitialZoom) * 2; // Buat lebih besar untuk menutupi

        // 2. Buat vertices untuk LINGKARAN DALAM (lubang)
        Vector3 centerPosition = _map.GeoToWorldPosition(_centerCoordinate);
        float clearRadiusInWorldUnits = _clearRadiusInMeters / _map.WorldRelativeScale;
        
        List<Vector3> innerVertices = new List<Vector3>();
        for (int i = 0; i < _segments; i++)
        {
            float angle = i * 2 * Mathf.PI / _segments;
            float x = centerPosition.x + clearRadiusInWorldUnits * Mathf.Cos(angle);
            float z = centerPosition.z + clearRadiusInWorldUnits * Mathf.Sin(angle);
            innerVertices.Add(new Vector3(x, _fogHeight, z));
        }

        // 3. Buat vertices untuk LINGKARAN LUAR (batas kabut)
        List<Vector3> outerVertices = new List<Vector3>();
        for (int i = 0; i < _segments; i++)
        {
            float angle = i * 2 * Mathf.PI / _segments;
            float x = centerPosition.x + mapSize * Mathf.Cos(angle);
            float z = centerPosition.z + mapSize * Mathf.Sin(angle);
            outerVertices.Add(new Vector3(x, _fogHeight, z));
        }

        // 4. Gabungkan semua vertices
        // Urutannya: semua titik dalam dulu, baru semua titik luar
        List<Vector3> allVertices = new List<Vector3>();
        allVertices.AddRange(innerVertices);
        allVertices.AddRange(outerVertices);

        // 5. Buat triangles untuk menyambungkan cincin dalam dan luar
        List<int> triangles = new List<int>();
        int innerStartIndex = 0;
        int outerStartIndex = _segments;

        for (int i = 0; i < _segments; i++)
        {
            int currentInner = innerStartIndex + i;
            int nextInner = innerStartIndex + (i + 1) % _segments; // Kembali ke 0 jika di akhir
            
            int currentOuter = outerStartIndex + i;
            int nextOuter = outerStartIndex + (i + 1) % _segments;

            // Triangle 1 (membentuk satu bagian dari quad)
            triangles.Add(currentInner);
            triangles.Add(nextOuter);
            triangles.Add(currentOuter);
            
            // Triangle 2 (membentuk bagian kedua dari quad)
            triangles.Add(currentInner);
            triangles.Add(nextInner);
            triangles.Add(nextOuter);
        }

        // 6. Buat dan terapkan Mesh
        Mesh fogMesh = new Mesh();
        fogMesh.name = "FogOfWarMesh";
        fogMesh.vertices = allVertices.ToArray();
        fogMesh.triangles = triangles.ToArray();
        fogMesh.RecalculateNormals();

        _meshFilter.mesh = fogMesh;
    }
}