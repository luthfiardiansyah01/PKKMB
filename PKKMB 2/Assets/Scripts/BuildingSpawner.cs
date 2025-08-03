using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildingSpawner : MonoBehaviour
{
    public BuildingLoc buildingLocScript;
    public GameObject itemPrefab;
    public Transform contentParent;
    public TextMeshProUGUI buildingCounter;


    // Simulasi data dari server
    [System.Serializable]
    public class BuildingData
    {
        public int id;
        public string name;
        public bool visited;
        public string imageName;
    }

    public List<BuildingData> buildingList = new List<BuildingData>();

    void Start()
    {
        SpawnBuildingItems();
    }

    void SpawnBuildingItems()
    {
        int visitedCount = 0;


        foreach (var data in buildingList)
        {
            GameObject item = Instantiate(itemPrefab, contentParent);
            Debug.Log("Item Root: " + item.name);


            // Gambar
            Sprite buildingImage = Resources.Load<Sprite>("BuildingImages/" + data.imageName);
            if (buildingImage != null)
                // Debug.Log(item);

                item.transform.Find("Image").GetComponent<Image>().sprite = buildingImage;

            Transform namaGedungTransform = item.transform.Find("NamaGedung");
            TextMeshProUGUI namaGedungText = namaGedungTransform.GetComponent<TextMeshProUGUI>();
            namaGedungText.text = data.name;

            // Visited status
            GameObject darkOverlay = item.transform.Find("DarkOverlay").gameObject;
            darkOverlay.SetActive(!data.visited); // kalau belum dikunjungi, tampilkan gelap

            if (data.visited)
                visitedCount++;
        }

        buildingCounter.text = visitedCount + " / " + buildingList.Count + " Building";
    }
}
