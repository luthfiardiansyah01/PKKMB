using UnityEngine;
using UnityEngine.UI; // Untuk Button
using TMPro; // Untuk TMP_InputField
using System.Collections.Generic; // Penting untuk menggunakan List

public class Hidemanager : MonoBehaviour
{
    // Menggunakan List untuk Input Field agar lebih fleksibel
    public List<TMP_InputField> requiredInputFields;

    // Deklarasikan tombol aksi
    public Button actionButton;

    void Start()
    {
        // Panggil fungsi CheckAllInputFields saat aplikasi dimulai
        CheckAllInputFields();
    }

    void Update()
    {
        // Panggil fungsi CheckAllInputFields setiap frame
        CheckAllInputFields();
    }

    // Fungsi untuk memeriksa semua input field dalam list
    void CheckAllInputFields()
    {
        // Asumsikan semua field sudah terisi sampai terbukti sebaliknya
        bool allFieldsFilled = true;

        // Loop melalui setiap Input Field di dalam list
        foreach (TMP_InputField inputField in requiredInputFields)
        {
            // Jika ada satu saja input field yang null atau kosong,
            // maka set allFieldsFilled menjadi false dan keluar dari loop
            if (inputField == null || string.IsNullOrEmpty(inputField.text))
            {
                allFieldsFilled = false;
                break; // Keluar dari loop karena kita sudah tahu ada yang kosong
            }
        }

        // Mengatur properti interactable dari tombol aksi
        if (actionButton != null)
        {
            actionButton.interactable = allFieldsFilled;
        }
    }

    // Fungsi ini akan dipanggil saat tombol aksi ditekan (opsional)
    public void OnActionButtonClicked()
    {
        if (actionButton != null && actionButton.interactable) // Hanya jalankan jika tombol aktif
        {
            Debug.Log("Tombol Aksi Ditekan!");
            // Anda bisa mengakses teks dari setiap input field melalui loop
            foreach (TMP_InputField inputField in requiredInputFields)
            {
                Debug.Log(inputField.name + ": " + inputField.text);
            }

            // Di sini Anda bisa menambahkan logika lebih lanjut (login, registrasi, dll.)
        }
        else
        {
            Debug.LogWarning("Mohon isi semua field yang diperlukan.");
        }
    }
}