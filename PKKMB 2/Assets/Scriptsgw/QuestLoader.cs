using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Mapbox.Json;


[System.Serializable]
public class QuestSet2
{
    public int id;
    public string name;
    public List<Quests2> quests;
}

[System.Serializable]
public class Quests2
{
    public string questId;
    public string question;
    public List<string> options;
    public string answer;
}



public class QuestLoader : MonoBehaviour
{
    public Dictionary<string, QuestSet2> ListQuest = new Dictionary<string, QuestSet2>();
    public QuestSet2 questSet;
    public string IdQuest;

    void Start()
    {
        GetQuestSet();
    }

    //void GetQuestList()
    //{
    //    PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
    //    {
    //        if (result.Data != null && result.Data.ContainsKey("ListQuest"))
    //        {
    //            string json = result.Data["ListQuest"];

    //            ListQuest = JsonConvert.DeserializeObject<Dictionary<string, QuestSet>>(json);

    //            foreach (var set in ListQuest)
    //            {
    //                Debug.Log($"ID Set: {set.Key} - Nama: {set.Value.name}");
    //                foreach (var q in set.Value.quests)
    //                {
    //                    Debug.Log($"  {q.questId} | {q.question} | Opsi: {string.Join(", ", q.options)} | Jawaban: {q.answer}");
    //                }
    //            }
    //        }
    //    },
    //    error => Debug.LogError(error.GenerateErrorReport()));
    //}

    void GetQuestSet()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("ListQuiz"))
            {
                string json = result.Data["ListQuiz"];

                var allQuests = JsonConvert.DeserializeObject<Dictionary<string, QuestSet2>>(json);

                if (allQuests.ContainsKey(IdQuest))
                {
                    questSet = allQuests[IdQuest];

                    Debug.Log($"ID Set: {questSet.id} - Nama: {questSet.name}");
                    foreach (var q in questSet.quests)
                    {
                        Debug.Log($"  {q.questId} | {q.question} | Pilihan: {string.Join(", ", q.options)} | Jawaban: {q.answer}");
                    }
                }
                else
                {
                    Debug.LogWarning("Quiz ID 1 tidak ditemukan di Title Data.");
                }
            }
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}