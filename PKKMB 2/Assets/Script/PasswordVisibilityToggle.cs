using UnityEngine;
using UnityEngine.UI; // Untuk Button
using TMPro; // Untuk TMP_InputField dan TMP_Text

public class PasswordVisibilityToggle : MonoBehaviour
{
    public TMP_InputField passwordInputField;
    public TMP_Text toggleButtonText; // Opsi: Teks pada tombol Show/Hide

    private bool isPasswordVisible = false; // Status saat ini: apakah password terlihat?

    void Start()
    {
        // Pastikan input field dimulai dalam mode password (karakter bintang)
        // Dan teks tombol sesuai dengan status awal
        SetPasswordVisibility(false);
    }

    // Fungsi ini dipanggil ketika tombol Show/Hide ditekan
    public void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible; // Membalik status

        SetPasswordVisibility(isPasswordVisible);
    }

    private void SetPasswordVisibility(bool isVisible)
    {
        if (passwordInputField == null)
        {
            Debug.LogError("Password Input Field belum diatur di Inspector!");
            return;
        }

        if (isVisible)
        {
            // Jika terlihat (Show), ubah Content Type ke Standard
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            if (toggleButtonText != null)
            {
                toggleButtonText.text = "Hide"; // Ubah teks tombol menjadi "Hide"
            }
        }
        else
        {
            // Jika tersembunyi (Hide), ubah Content Type ke Password
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
            if (toggleButtonText != null)
            {
                toggleButtonText.text = "Show"; // Ubah teks tombol menjadi "Show"
            }
        }

        // Penting: Reset caret (kursor teks) setelah mengubah Content Type
        // Ini memastikan teks diperbarui dengan benar
        passwordInputField.ForceLabelUpdate();
    }
}