using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private bool isZoomed = false;
    private Vector3 originalPosition;
    public float zoomYPosition = 40f;
    public float zoomSpeed = 5f;
    public float normalYPosition = 80f;

    void Start()
    {
        // Simpan posisi awal camera
        originalPosition = transform.position;
    }

    public void ToggleZoom()
    {
        isZoomed = !isZoomed;
        
        if (isZoomed)
        {
            // Zoom in - turunkan posisi Y ke 80
            Vector3 newPosition = transform.position;
            newPosition.y = zoomYPosition;
            transform.position = newPosition;
        }
        else
        {
            // Normal - kembalikan posisi Y ke 114
            Vector3 newPosition = transform.position;
            newPosition.y = normalYPosition;
            transform.position = newPosition;
        }
    }
}
