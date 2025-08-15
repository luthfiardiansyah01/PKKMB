using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlidingMenuControllerX : MonoBehaviour
{
    public RectTransform menuPanel; // Panel menu yang akan digeser
    public float slideDuration = 0.5f; // Durasi animasi slide
    public float hiddenXPosition; // Posisi X saat menu tersembunyi
    public float visibleXPosition; // Posisi X saat menu terlihat

    private bool isMenuVisible = false;

    // Fungsi untuk menggeser menu
    public void ToggleMenu()
    {
        if (isMenuVisible)
        {
            // Sembunyikan
            StartCoroutine(SlideMenu(hiddenXPosition));
        }
        else
        {
            // Tampilkan
            StartCoroutine(SlideMenu(visibleXPosition));
        }
    }

    // Coroutine animasi slide
    private IEnumerator SlideMenu(float targetX)
    {
        isMenuVisible = !isMenuVisible;

        float startX = menuPanel.anchoredPosition.x;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.Lerp(startX, targetX, elapsedTime / slideDuration);
            menuPanel.anchoredPosition = new Vector2(newX, menuPanel.anchoredPosition.y);
            yield return null;
        }

        // Pastikan posisi akhir benar
        menuPanel.anchoredPosition = new Vector2(targetX, menuPanel.anchoredPosition.y);
    }
}
