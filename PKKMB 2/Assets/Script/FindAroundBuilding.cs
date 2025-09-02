using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Mapbox.Json;


[System.Serializable]
public class FindAroundQuest
{
    public List<string> options;
    public List<string> answer;
}

[System.Serializable]
public class FindAroundSet
{
    public string id;
    public List<FindAroundQuest> quests;
}
public class FindAroundBuilding : MonoBehaviour
{

    public static FindAroundBuilding Instance;
    public Dictionary<string, FindAroundSet> findAroundCache = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadFindAroundData();
    }

    private void LoadFindAroundData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("FindAround"))
                {
                    string rawJson = result.Data["FindAround"];

                    try
                    {
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, FindAroundSet>>(rawJson);
                        findAroundCache = dict;

                        Debug.Log($"✅ FindAround data berhasil dimuat. Total set: {findAroundCache.Count}");

                        foreach (var kvp in findAroundCache)
                        {
                            Debug.Log($"📌 BuildingID: {kvp.Key}, Jumlah Quest: {kvp.Value.quests.Count}");

                            // Loop semua quest
                            for (int i = 0; i < kvp.Value.quests.Count; i++)
                            {
                                var quest = kvp.Value.quests[i];
                                Debug.Log($"   ➡ Quest {i + 1}: Jumlah opsi = {quest.options.Count}");

                                // Loop semua option
                                for (int j = 0; j < quest.options.Count; j++)
                                {
                                    Debug.Log($"      🔹 Option {j + 1}: {quest.options[j]}");
                                }

                                // Kalau ada jawaban
                                if (quest.answer != null && quest.answer.Count > 0)
                                {
                                    Debug.Log($"      ✅ Answer: {string.Join(", ", quest.answer)}");
                                }
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("❌ Gagal parse JSON FindAround: " + e.Message);
                    }

                }
                else
                {
                    Debug.Log("⚠ Tidak ada data FindAround di PlayFab");
                }
            },
            error => Debug.LogError("Gagal ambil data FindAround: " + error.GenerateErrorReport()));
    }

    public FindAroundSet GetFindAroundByBuilding(string id)
    {
        if (findAroundCache.ContainsKey(id))
            return findAroundCache[id];
        return null;
    }
}
