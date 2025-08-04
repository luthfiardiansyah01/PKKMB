using UnityEngine;

/// <summary>
/// This script allows the player to click on a 3D object in the scene
/// to set it as the destination for the GpsSystem.
/// </summary>
public class ClickToSetDestination : MonoBehaviour
{
    // A reference to the GpsSystem script on your player.
    public GpsSystem gpsSystem;

    // The tag that identifies clickable destination objects.
    public string destinationTag = "Destination";

    // The main camera used for raycasting.
    private Camera mainCamera;

    void Start()
    {
        // Get the main camera from the scene.
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for a left mouse button click.
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse position.
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast.
            // We check if the ray hits a collider within 1000 units.
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                // Check if the hit object has the correct tag.
                if (hit.collider.CompareTag(destinationTag))
                {
                    // If the GpsSystem reference is set, update the destination.
                    if (gpsSystem != null)
                    {
                        Debug.Log("New destination set to: " + hit.transform.name);
                        gpsSystem.SetDestination(hit.transform);
                    }
                    else
                    {
                        Debug.LogWarning("GpsSystem reference is not set in the Inspector.");
                    }
                }
            }
        }
    }
}
