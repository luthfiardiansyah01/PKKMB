using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    public GameObject objToTransform;   // karakter
    public GameObject objToActivate;    // panel baju (tidak digunakan untuk set active, tapi untuk referensi RectTransform)
    public RectTransform menuPanel;     // Panel yang akan di-slide 

    public Vector3 targetScale = new Vector3(0.6f, 0.6f, 0.6f);
    public Vector3 targetPosition = new Vector3(0f, 200f, 0f);

    public float hiddenYPosition; // Posisi Y saat menu tersembunyi
    public float visibleYPosition; // Posisi Y saat menu terlihat

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private bool isPanelActive = false;
    private float duration = 0.5f; // durasi animasi

    void Start()
    {
        if (objToTransform != null)
        {
            originalScale = objToTransform.transform.localScale;
            originalPosition = objToTransform.GetComponent<RectTransform>().localPosition;
        }

        // Set posisi awal panel menu ke posisi tersembunyi
        if (menuPanel != null)
        {
            menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, hiddenYPosition);
        }
    }

    public void ToggleOutfitPanel()
    {
        if (objToTransform == null || menuPanel == null) return;

        isPanelActive = !isPanelActive;

        Vector3 startScale = objToTransform.transform.localScale;
        Vector3 startPos = objToTransform.GetComponent<RectTransform>().localPosition;

        Vector3 endScale = isPanelActive ? targetScale : originalScale;
        Vector3 endPos = isPanelActive ? targetPosition : originalPosition;

        float targetY = isPanelActive ? visibleYPosition : hiddenYPosition;

        // Stop any ongoing animation before starting a new one
        StopAllCoroutines();
        StartCoroutine(AnimateBoth(objToTransform.GetComponent<RectTransform>(), menuPanel, startScale, endScale, startPos, endPos, targetY));
    }

    IEnumerator AnimateBoth(RectTransform targetChar, RectTransform targetPanel, Vector3 fromScale, Vector3 toScale, Vector3 fromPos, Vector3 toPos, float targetY)
    {
        float elapsed = 0f;
        Vector3 startPanelPos = targetPanel.anchoredPosition;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            
            // Animasi karakter
            targetChar.localScale = Vector3.Lerp(fromScale, toScale, t);
            targetChar.localPosition = Vector3.Lerp(fromPos, toPos, t);

            // Animasi slide panel
            float newY = Mathf.Lerp(startPanelPos.y, targetY, t);
            targetPanel.anchoredPosition = new Vector2(targetPanel.anchoredPosition.x, newY);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are applied
        targetChar.localScale = toScale;
        targetChar.localPosition = toPos;
        targetPanel.anchoredPosition = new Vector2(targetPanel.anchoredPosition.x, targetY);
    }
}