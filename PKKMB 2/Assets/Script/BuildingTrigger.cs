using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Mapbox.Json;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting;

public class BuildingTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId;
    private GameObject questionMark;
    public Button ButtonStartQuiz;
    public TextMeshProUGUI TextTMP;

    private string currentSessionId;

    void Start()
    {
        currentSessionId = SystemInfo.deviceUniqueIdentifier;
        // Cari child bernama "QuestionMark" dari building ini
        Transform qmTransform = transform.Find("QuestionMark");
        if (qmTransform != null)
        {
            questionMark = qmTransform.gameObject;
            questionMark.SetActive(false); // Pastikan awalnya mati
        }
        else
        {
            Debug.LogWarning($"Building tidak punya child QuestionMark!");
        }
        
        Transform buttonTransform = transform.Find("Info/QuizSection/ButtonStartQuiz");
        if (buttonTransform != null)
        {
            ButtonStartQuiz = buttonTransform.GetComponent<Button>();

            // 🔎 Cari Text TMP di dalam ButtonStartQuiz
            Transform textTransform = buttonTransform.Find("Text (TMP)");
            if (textTransform != null)
            {
                TextTMP = textTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("❌ Text (TMP) tidak ditemukan di dalam ButtonStartQuiz");
            }
        }
        else
        {
            Debug.LogWarning("❌ ButtonStartQuiz tidak ditemukan di Info/QuizSection");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Player")) return;

        // Aktifkan question mark
        if (questionMark != null)
        {
            questionMark.SetActive(true);
            Debug.Log($"Question mark untuk {buildingId} ditampilkan");
            
            if (QuestionMarkManager.Instance != null)
            {
                QuestionMarkManager.Instance.currentBuildingId = buildingId;
                // CheckQuizStatus(); //ini bikin error
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Aktifkan question mark
        if (questionMark != null)
        {
            questionMark.SetActive(false);
            Debug.Log($"Question mark untuk {buildingId} ditutup");
        }
    }

    public void CheckQuizStatus()
    {
        CheckSession();

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            bool isCompleted = false;
            if (result.Data != null && result.Data.ContainsKey("completedQuizzes"))
            {
                string currentData = result.Data["completedQuizzes"].Value;
                List<string> completedList = new List<string>(currentData.Split(','));

                // Cek apakah ID gedung ini ada di dalam daftar yang sudah selesai
                if (completedList.Contains(buildingId))
                {
                    isCompleted = true;
                }
            }

            // Update tampilan tombol berdasarkan status
            UpdateQuizButtonUI(isCompleted);

        },
        error =>
        {
            Debug.LogError("Gagal memeriksa status kuis: " + error.GenerateErrorReport());
        
            // ButtonStartQuiz.interactable = false;
            // ButtonStartQuiz.gameObject.SetActive(false);
            // TextTMP.text = "Error";
        });
    }

    private void UpdateQuizButtonUI(bool isCompleted)
    {
        if (isCompleted)
        {
            // Jika sudah selesai
            // TextTMP.text = "SELESAI";
            ButtonStartQuiz.interactable = false; // Tombol tidak bisa diklik lagi
        }
        else
        {
            // Jika belum dikerjakan
            // TextTMP.text = "MULAI KUIS";
            ButtonStartQuiz.interactable = true; // Tombol bisa diklik
        }
    }

    void CheckSession()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("deviceSession"))
            {
                string sessionFromServer = result.Data["deviceSession"].Value;

                if (sessionFromServer != currentSessionId)
                {
                    Debug.LogWarning("Session tidak valid. User login dari device lain.");
                    SceneManager.LoadScene("Main Menu");
                }
            }
        },
        error => Debug.LogError("Gagal ambil session: " + error.GenerateErrorReport()));
    }


}
