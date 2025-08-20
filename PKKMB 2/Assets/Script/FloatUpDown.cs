using UnityEngine;

public class FloatUpDown : MonoBehaviour
{
    public float minY = 0f;   // Batas bawah relatif dari posisi awal
    public float maxY = 1f;   // Batas atas relatif dari posisi awal
    public float speed = 1f;  // Kecepatan naik-turun

    public float rotationSpeed = 50f;

    private Vector3 startPos;

    void Start()
    {
        // Simpan posisi awal
        startPos = transform.position;
    }

    void Update()
    {
        // Hitung posisi Y bolak-balik antara minY dan maxY
        float newY = Mathf.PingPong(Time.time * speed, maxY - minY) + minY;
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}