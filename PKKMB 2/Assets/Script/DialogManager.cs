using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // untuk ganti scene

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

    void Start()
    {
        ShowLine();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // klik kiri
        {
            if (isTyping)
            {
                // Langsung tampilkan seluruh teks
                StopCoroutine(typingCoroutine);
                tmpText.text = dialogLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // Ke baris berikutnya atau ganti scene
                currentLineIndex++;
                if (currentLineIndex < dialogLines.Count)
                {
                    ShowLine();
                }
                else
                {
                    // Ganti scene
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