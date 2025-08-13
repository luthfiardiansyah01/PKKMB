using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Mapbox.Json;

// [System.Serializable]
// public class QuestSet
// {
//     public int id;
//     public string name;
//     public List<Quests> quests;
// }

// [System.Serializable]
// public class Quests
// {
//     public string questId;
//     public string question;
//     public List<string> options;
//     public string answer;
// }


public class QuizManager : MonoBehaviour
{
    [Header("Database & UI References")]
    public QuestionDatabase questionDatabase;
    public Text questionText;
    public Toggle[] optionToggles;

    [Header("Progress UI")]
    public Image progressFill;
    public GameObject finishConfirmationPanel;

    public Button prevButton;
    public Button nextButton;

    private QuestionData[] randomizedQuestions;
    private int currentQuestionIndex = 0;


    //Baru
    // public Dictionary<string, QuestSet> ListQuest = new Dictionary<string, QuestSet>();
    // public QuestSet questSet;
    // public string IdQuest;

    void Start()
    {
        if (questionDatabase != null && questionDatabase.questions.Length > 0)
        {
            randomizedQuestions = questionDatabase.questions.OrderBy(q => Random.value).ToArray();
            LoadQuestion();
        }
        else
        {
            Debug.LogError("❌ Question Database kosong atau belum diassign!");
        }
    }

    void LoadQuestion()
    {
        if (randomizedQuestions == null || randomizedQuestions.Length == 0)
            return;

        var q = randomizedQuestions[currentQuestionIndex];
        var shuffledOptions = q.options.OrderBy(o => Random.value).ToArray();

        questionText.text = q.questionText;

        for (int i = 0; i < optionToggles.Length; i++)
        {
            if (i < shuffledOptions.Length)
            {
                Text label = optionToggles[i].GetComponentInChildren<Text>();
                if (label != null)
                    label.text = shuffledOptions[i];

                optionToggles[i].gameObject.SetActive(true);
                optionToggles[i].isOn = false;
            }
            else
            {
                optionToggles[i].gameObject.SetActive(false);
            }
        }

        UpdateProgress();
    }

    void UpdateProgress()
    {
        float progressValue = (float)(currentQuestionIndex + 1) / randomizedQuestions.Length;
        progressFill.fillAmount = progressValue;
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex < randomizedQuestions.Length - 1)
        {
            currentQuestionIndex++;
            LoadQuestion();
        }
        else
        {
            ShowFinishConfirmation();
        }
    }

    public void PrevQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            LoadQuestion();
        }
    }

    // void GetQuestSet()
    // {
    //     PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
    //     {
    //         if (result.Data != null && result.Data.ContainsKey("ListQuiz"))
    //         {
    //             string json = result.Data["ListQuiz"];

    //             var allQuests = JsonConvert.DeserializeObject<Dictionary<string, QuestSet>>(json);

    //             if (allQuests.ContainsKey(IdQuest))
    //             {
    //                 questSet = allQuests[IdQuest];

    //                 Debug.Log($"ID Set: {questSet.id} - Nama: {questSet.name}");
    //                 foreach (var q in questSet.quests)
    //                 {
    //                     Debug.Log($"  {q.questId} | {q.question} | Pilihan: {string.Join(", ", q.options)} | Jawaban: {q.answer}");
    //                 }
    //             }
    //             else
    //             {
    //                 Debug.LogWarning("Quiz ID 1 tidak ditemukan di Title Data.");
    //             }
    //         }
    //     },
    //     error => Debug.LogError(error.GenerateErrorReport()));
    // }

    public void ShowFinishConfirmation()
    {
        finishConfirmationPanel?.SetActive(true);
    }

    public void HideFinishConfirmation()
    {
        finishConfirmationPanel?.SetActive(false);
    }

    public void FinishQuiz()
    {
        Debug.Log("✅ Quiz Finished!");
    }
}
