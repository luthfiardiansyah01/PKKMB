using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class DialogManagerTMP : MonoBehaviour
{
    [Header("References")]
    public GameObject dialogContainer;
    public GameObject dialogLinePrefab;

    [Header("Dialog Settings")]
    [TextArea(2, 5)]
    public List<string> dialogLines;

    public float letterDelay = 0.05f;

    private int currentLineIndex = 0;
    private GameObject currentLineObj;
    private TextMeshProUGUI tmpText;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private string playerUsername = "Player"; // default sementara

    void Start()
    {
        GetUsernameFromPlayfab();
    }

    void GetUsernameFromPlayfab()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountSuccess, OnError);
    }

    void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        if (result.AccountInfo.TitleInfo.DisplayName != null)
            playerUsername = result.AccountInfo.TitleInfo.DisplayName;
        else if (result.AccountInfo.Username != null)
            playerUsername = result.AccountInfo.Username;

        // Setelah dapat username → isi dialog
        dialogLines = new List<string>()
        {
             "Hi, " + playerUsername + ". I'm Galih, your campus guide at Telkom University.",
        "My job is to help new students complete an important mission by exploring the campus and getting to know the key buildings",
        "But something strange happened... I suddenly lost my sense of direction! Everything feels unfamiliar, and I can't remember where anything is",
        "That's why I need your help. Will you join me on this journey, complete the challenges, and help me rediscover the campus?"
        };

        ShowLine();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError("Gagal ambil username: " + error.GenerateErrorReport());

        // Fallback pakai default playerUsername
        dialogLines = new List<string>()
        {
            "Hi, " + playerUsername + " I'm Galih, your campus guide at Telkom University.",
        "My job is to help new students complete an important mission by exploring the campus and getting to know the key buildings",
        "But something strange happened... I suddenly lost my sense of direction! Everything feels unfamiliar, and I can't remember where anything is",
        "That's why I need your help. Will you join me on this journey, complete the challenges, and help me rediscover the campus?"
        };

        ShowLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // klik kiri
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                tmpText.text = dialogLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                currentLineIndex++;
                if (currentLineIndex < dialogLines.Count)
                {
                    ShowLine();
                }
                else
                {
                    SceneManager.LoadScene("GamePlay");
                }
            }
        }
    }

    void ShowLine()
    {
        if (currentLineObj != null)
            Destroy(currentLineObj);

        currentLineObj = Instantiate(dialogLinePrefab, dialogContainer.transform);
        tmpText = currentLineObj.GetComponent<TextMeshProUGUI>();
        tmpText.text = "";

        typingCoroutine = StartCoroutine(TypeText(dialogLines[currentLineIndex]));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        for (int i = 0; i <= line.Length; i++)
        {
            tmpText.text = line.Substring(0, i);
            yield return new WaitForSeconds(letterDelay);
        }
        isTyping = false;
    }
}
