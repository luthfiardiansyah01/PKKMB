using UnityEngine;

public class OpenLinkButton : MonoBehaviour
{
    // Link tujuan
    [SerializeField] private string url = "https://forms.gle/6EHsXGZ8DahEszHo9";

    // Fungsi ini dipanggil saat tombol ditekan
    public void OpenLink()
    {
        Application.OpenURL(url);
    }
}
