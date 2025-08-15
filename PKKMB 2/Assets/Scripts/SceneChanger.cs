using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Variabel untuk menyimpan nama scene yang akan dimuat
    public string nextSceneName;

    // Variabel untuk menentukan berapa lama waktu tunggu sebelum ganti scene
    public float delayTime = 5f;

    // Variabel untuk menyimpan timer
    private float timer;

    void Start()
    {
        // Inisialisasi timer saat scene dimulai
        timer = delayTime;
    }

    void Update()
    {
        // Mengurangi timer setiap frame berdasarkan waktu yang berlalu
        timer -= Time.deltaTime;

        // Memeriksa apakah timer sudah habis
        if (timer <= 0)
        {
            // Memanggil fungsi untuk memuat scene berikutnya
            LoadNextScene();
        }
    }

    // Fungsi untuk memuat scene baru
    void LoadNextScene()
    {
        // Memuat scene dengan nama yang telah ditentukan
        SceneManager.LoadScene(nextSceneName);
    }
}