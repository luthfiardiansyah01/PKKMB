using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // Kecepatan rotasi
    public float rotationSpeed = 1f;

    void Update()
    {
        // Mendeteksi input sentuhan (untuk perangkat seluler)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            HandleTouchInput();
        }
        // Mendeteksi input mouse (untuk pengujian di Unity Editor)
        else if (Input.GetMouseButton(0))
        {
            HandleMouseInput();
        }
    }

    void HandleTouchInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                float deltaX = Input.GetTouch(0).deltaPosition.x;
                transform.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    void HandleMouseInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                float deltaX = Input.GetAxis("Mouse X");
                transform.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
