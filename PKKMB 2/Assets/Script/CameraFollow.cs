using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target (Player)")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5f, -5f); // posisi awal relatif
    public float followSpeed = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 0.2f;
    private float yaw = 0f;
    private Vector2 lastTouchPos;
    private bool isDragging = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 3f;
    public float maxZoom = 15f;
    private float currentZoom;   // jarak sekarang (panjang offset)

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target (Player) belum di-assign!");
            return;
        }

        currentZoom = offset.magnitude; // panjang offset awal
    }

    void Update()
    {
        // --- Rotasi dengan 1 jari ---
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastTouchPos;
                yaw += delta.x * rotationSpeed * Time.deltaTime;
                lastTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }

        // --- Zoom dengan pinch (2 jari) ---
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0Prev - touch1Prev).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            currentZoom -= difference * zoomSpeed * Time.deltaTime;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // rotasi sekeliling Y
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);

        // vektor offset asli (arah), lalu skalakan dengan currentZoom
        Vector3 direction = rotation * offset.normalized;
        Vector3 desiredPosition = target.position + direction * currentZoom;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Kamera menghadap player
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}