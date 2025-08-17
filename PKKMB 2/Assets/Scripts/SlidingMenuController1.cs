using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlidingMenuController1 : MonoBehaviour
{
    public RectTransform menuPanel; // Referensi ke RectTransform dari panel menu
    public float slideDuration = 0.5f; // Durasi animasi slide
    public float hiddenYPosition; // Posisi Y saat menu tersembunyi
    public float visibleYPosition; // Posisi Y saat menu terlihat

    private bool isMenuVisible = false;

    // Fungsi untuk menggeser menu
    public void ToggleMenu()
    {
        if (isMenuVisible)
        {
            // Jika menu terlihat, sembunyikan
            StartCoroutine(SlideMenu(hiddenYPosition));
        }
        else
        {
            // Jika menu tersembunyi, tampilkan
            StartCoroutine(SlideMenu(visibleYPosition));
        }
    }

    // Coroutine untuk menganimasikan slide menu
    private IEnumerator SlideMenu(float targetY)
    {
        isMenuVisible = !isMenuVisible; // Balik status menu

        float startY = menuPanel.anchoredPosition.y;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startY, targetY, elapsedTime / slideDuration);
            menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, newY);
            yield return null;
        }

        // Pastikan posisi akhir benar
        menuPanel.anchoredPosition = new Vector2(menuPanel.anchoredPosition.x, targetY);
    }
}