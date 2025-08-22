using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    // Array untuk menyimpan semua objek karakter yang bisa dipilih.
    // Atur objek-objek ini di Inspector Unity.
    public GameObject[] characters;

    // Metode untuk memilih karakter tertentu.
    // Panggil metode ini dari tombol UI atau script lain saat karakter dipilih.
    public void SelectCharacter(GameObject selectedCharacter)
    {
        // Pastikan array karakter tidak kosong.
        if (characters.Length == 0)
        {
            Debug.LogWarning("Array karakter kosong. Silakan tambahkan objek karakter di Inspector.");
            return;
        }

        // Loop melalui setiap karakter dalam array.
        foreach (GameObject character in characters)
        {
            // Periksa apakah karakter saat ini adalah karakter yang dipilih.
            if (character == selectedCharacter)
            {
                // Jika ya, aktifkan karakter ini.
                character.SetActive(true);
                Debug.Log("Karakter " + character.name + " telah diaktifkan.");
            }
            else
            {
                // Jika tidak, nonaktifkan karakter ini.
                character.SetActive(false);
                Debug.Log("Karakter " + character.name + " telah dinonaktifkan.");
            }
        }
    }
}