using UnityEngine;
using System.Collections;

public class ButtonController3D : MonoBehaviour
{
    [Header("Objek 3D yang dikontrol")]
    public Transform[] objsToTransform;   // Bisa pilih beberapa objek 3D

    [Header("UI Panel")]
    public RectTransform menuPanel;     

    [Header("Target Transform")]
    public Vector3 targetScale = new Vector3(0.6f, 0.6f, 0.6f);
    public Vector3 targetPosition = new Vector3(0f, 2f, 0f); 

    [Header("Posisi Panel")]
    public float hiddenYPosition; 
    public float visibleYPosition; 

    private Vector3[] originalScales;
    private Vector3[] originalPositions;
    private bool isPanelActive = false;
    private float duration = 0.5f; // durasi animasi

    void Start()
    {
        // Simpan posisi & scale asli tiap objek
        if (objsToTransform != null && objsToTransform.Length > 0)
        {
            originalScales = new Vector3[objsToTransform.Length];
            originalPositions = new Vector3[objsToTransform.Length];

            for (int i = 0; i < objsToTransform.Length; i++)
            {
                if (objsToTransform[i] != null)
                {
                    originalScales[i] = objsToTransform[i].localScale;
                    originalPositions[i] = objsToTransform[i].localPosition;
                }
            }
        }

        // Set posisi awal panel menu ke posisi tersembunyi
        if (menuPanel != null)
        {
            menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, hiddenYPosition);
        }
    }

    public void ToggleOutfitPanel()
    {
        if (objsToTransform == null || objsToTransform.Length == 0 || menuPanel == null) return;

        isPanelActive = !isPanelActive;

        StopAllCoroutines();
        StartCoroutine(AnimateBoth());
    }

    IEnumerator AnimateBoth()
    {
        float elapsed = 0f;
        Vector3 startPanelPos = menuPanel.anchoredPosition;
        float targetY = isPanelActive ? visibleYPosition : hiddenYPosition;

        // Siapkan start & end tiap objek
        Vector3[] startScales = new Vector3[objsToTransform.Length];
        Vector3[] startPositions = new Vector3[objsToTransform.Length];
        Vector3[] endScales = new Vector3[objsToTransform.Length];
        Vector3[] endPositions = new Vector3[objsToTransform.Length];

        for (int i = 0; i < objsToTransform.Length; i++)
        {
            startScales[i] = objsToTransform[i].localScale;
            startPositions[i] = objsToTransform[i].localPosition;
            endScales[i] = isPanelActive ? targetScale : originalScales[i];
            endPositions[i] = isPanelActive ? targetPosition : originalPositions[i];
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Animasi semua objek 3D
            for (int i = 0; i < objsToTransform.Length; i++)
            {
                objsToTransform[i].localScale = Vector3.Lerp(startScales[i], endScales[i], t);
                objsToTransform[i].localPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
            }

            // Animasi slide panel UI
            float newY = Mathf.Lerp(startPanelPos.y, targetY, t);
            menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, newY);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pastikan nilai akhir diterapkan
        for (int i = 0; i < objsToTransform.Length; i++)
        {
            objsToTransform[i].localScale = endScales[i];
            objsToTransform[i].localPosition = endPositions[i];
        }
        menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, targetY);
    }
}
