using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class MenuHistoryForBuilding : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform contentParent;


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

        foreach (var data in buildingList)
        {
            if (data.visited)
            {
                GameObject item = Instantiate(itemPrefab, contentParent);
                // Gambar
                Sprite buildingImage = Resources.Load<Sprite>("BuildingImages/" + data.imageName);
                if (buildingImage != null)
                    // Debug.Log(item);

                    item.transform.Find("Image").GetComponent<Image>().sprite = buildingImage;

                Transform namaGedungTransform = item.transform.Find("NamaGedung");
                TextMeshProUGUI namaGedungText = namaGedungTransform.GetComponent<TextMeshProUGUI>();
                namaGedungText.text = data.name;

            }


        }
    }
}
