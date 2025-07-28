using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManagerTMP : MonoBehaviour
{
    [Header("References")]
    public GameObject dialogContainer; 
    public GameObject dialogLinePrefab; 

    [Header("Dialog Settings")]
    [TextArea(2, 5)]
    public List<string> dialogLines; 

    public float delayBetweenLines = 3f; 
    public float letterDelay = 0.05f;    

    private void Start()
    {
        StartCoroutine(PlayDialog());
    }

    IEnumerator PlayDialog()
    {
        foreach (string line in dialogLines)
        {
            GameObject newLine = Instantiate(dialogLinePrefab, dialogContainer.transform);
            TextMeshProUGUI tmpText = newLine.GetComponent<TextMeshProUGUI>();

            tmpText.text = ""; 
            yield return StartCoroutine(TypeText(tmpText, line));

            yield return new WaitForSeconds(delayBetweenLines); 
        }
    }

    IEnumerator TypeText(TextMeshProUGUI tmpText, string fullText)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            tmpText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(letterDelay);
        }
    }
}
