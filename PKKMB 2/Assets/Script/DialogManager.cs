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
    public float destroyDelay = 1f; // Waktu tambahan sebelum baris dihapus

    private void Start()
    {
        StartCoroutine(PlayDialog());
    }

    IEnumerator PlayDialog()
    {
        foreach (string line in dialogLines)
        {
            // Membuat baris dialog baru
            GameObject newLine = Instantiate(dialogLinePrefab, dialogContainer.transform);
            TextMeshProUGUI tmpText = newLine.GetComponent<TextMeshProUGUI>();

            // Menampilkan teks
            tmpText.text = ""; 
            yield return StartCoroutine(TypeText(tmpText, line));

            // Menunggu jeda setelah teks selesai ditampilkan
            yield return new WaitForSeconds(delayBetweenLines); 

            // Menunggu sebentar sebelum menghapus baris dialog
            yield return new WaitForSeconds(destroyDelay); 
            Destroy(newLine);
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