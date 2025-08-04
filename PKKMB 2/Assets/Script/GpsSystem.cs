using UnityEngine;
using UnityEngine.AI; // Required for NavMesh Agent
using TMPro; // Use TextMeshPro for UI text

/// <summary>
/// A GPS-style navigation system that draws a line to a target destination.
/// This version ONLY draws the line and does NOT move the player object.
/// It also updates a TextMeshPro UI element to display the current destination's name.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(NavMeshAgent))]
public class GpsSystem : MonoBehaviour
{
    [Header("UI Settings")]
    // The TextMeshPro UI element that will display the name of the current destination.
    public TextMeshProUGUI destinationDisplay;
    // The default text to show when no destination is selected.
    public string defaultDisplayText = "Select Destination";

    // The target destination for the GPS.
    private Transform targetDestination;

    // The NavMeshAgent component attached to this GameObject.
    private NavMeshAgent navMeshAgent;

    // The LineRenderer component to draw the path.
    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the required components.
        navMeshAgent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        // --- Configure the Line Renderer ---
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.useWorldSpace = true;
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        }

        // --- Disable Agent Movement ---
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        // --- Clear destination on start ---
        ClearDestination();
    }

    void Update()
    {
        // We must manually update the agent's position to match the player's transform.
        navMeshAgent.nextPosition = transform.position;

        // If a target is set, calculate and draw the path.
        if (targetDestination != null)
        {
            navMeshAgent.SetDestination(targetDestination.position);
            if (navMeshAgent.hasPath)
            {
                DrawPath();
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void DrawPath()
    {
        lineRenderer.positionCount = navMeshAgent.path.corners.Length;
        lineRenderer.SetPositions(navMeshAgent.path.corners);
    }

    /// <summary>
    /// Sets a new destination for the GPS and updates the UI display.
    /// </summary>
    public void SetDestination(Transform newDestination)
    {
        targetDestination = newDestination;

        // Update the UI Text if it's assigned.
        if (destinationDisplay != null && newDestination != null)
        {
            destinationDisplay.text = newDestination.name;
        }
    }

    /// <summary>
    /// Clears the current destination, removes the line, and resets the UI display.
    /// </summary>
    public void ClearDestination()
    {
        targetDestination = null;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }

        // Reset the UI Text to the default message.
        if (destinationDisplay != null)
        {
            destinationDisplay.text = defaultDisplayText;
        }
    }
}
