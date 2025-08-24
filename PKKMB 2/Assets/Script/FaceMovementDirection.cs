using UnityEngine;

public class FaceMovementDirection : MonoBehaviour
{
    // Kecepatan putaran karakter. Semakin besar nilainya, semakin cepat ia berputar.
    [Tooltip("Kecepatan karakter berputar menghadap arah tujuan.")]
    private float rotationSpeed = 10f;

    // Ambang batas pergerakan agar karakter tidak berputar saat diam atau bergerak sangat lambat.
    [Tooltip("Seberapa jauh karakter harus bergerak sebelum mulai berputar.")]
    private float minMovementThreshold = 0.01f;

    // Variabel untuk menyimpan posisi dari frame sebelumnya.
    private Vector3 _previousPosition;

    void Start()
    {
        // Inisialisasi posisi awal.
        _previousPosition = transform.position;
    }

    void Update()
    {
        // Dapatkan posisi saat ini.
        Vector3 currentPosition = transform.position;

        // Hitung vektor pergerakan dari frame lalu ke frame sekarang.
        Vector3 movementDirection = currentPosition - _previousPosition;

        // PENTING: Ratakan vektor di sumbu Y. Ini mencegah karakter miring ke atas atau ke bawah.
        // Karakter hanya akan berputar di sumbu Y (kiri dan kanan).
        movementDirection.y = 0;

        // Hanya berputar jika karakter benar-benar bergerak melebihi ambang batas.
        // Ini mencegah getaran (jitter) saat karakter diam.
        if (movementDirection.magnitude > minMovementThreshold)
        {
            // Buat rotasi yang menghadap ke arah pergerakan.
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Putar karakter dari rotasi saat ini ke rotasi tujuan secara halus (smooth).
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Simpan posisi saat ini untuk digunakan di frame berikutnya.
        _previousPosition = currentPosition;
    }
}