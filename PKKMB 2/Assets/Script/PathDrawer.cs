using UnityEngine;
using UnityEngine.AI;

public class PathDrawer : MonoBehaviour
{
    public Transform player;
    public Transform destination;

    private LineRenderer lineRenderer;
    private NavMeshPath navPath;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        navPath = new NavMeshPath();

        // Set visual style here
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // Create and assign a material
        Material lineMat = new Material(Shader.Find("Sprites/Default"));
        lineMat.color = Color.green;
        lineRenderer.material = lineMat;

        // Optional: set line color directly
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        
    }

    void Update()
{
    if (player == null || destination == null) return;

    NavMesh.CalculatePath(player.position, destination.position, NavMesh.AllAreas, navPath);

    if (navPath.status == NavMeshPathStatus.PathComplete)
    {
        lineRenderer.positionCount = navPath.corners.Length;
        lineRenderer.SetPositions(navPath.corners);
        Debug.Log("Drawing line with " + navPath.corners.Length + " points");
    }
    else
    {
        Debug.LogWarning("Path incomplete or invalid!");
        lineRenderer.positionCount = 0;
    }
}

}
