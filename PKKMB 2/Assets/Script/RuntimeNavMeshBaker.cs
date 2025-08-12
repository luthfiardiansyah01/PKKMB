using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections; // Required for Coroutines

/// <summary>
/// This script listens for the Mapbox map to finish updating,
/// and then tells the NavMeshSurface to bake a new NavMesh.
/// This version includes a fallback to ensure baking happens even if the OnUpdated event is missed.
/// </summary>
[RequireComponent(typeof(AbstractMap))]
[RequireComponent(typeof(NavMeshSurface))]
public class RuntimeNavMeshBaker : MonoBehaviour
{
    private AbstractMap _map;
    private NavMeshSurface _navMeshSurface;
    private bool _hasBaked = false; // A flag to prevent baking more than once

    void Awake()
    {
        _map = GetComponent<AbstractMap>();
        _navMeshSurface = GetComponent<NavMeshSurface>();
        _map.OnUpdated += BakeNavMesh;
    }

    void Start()
    {
        // Fallback: If the map is already initialized and we haven't baked yet,
        // bake the navmesh after a short delay. This catches cases where OnUpdated fires before Awake/Start.
        if (_map.MapVisualizer.State == ModuleState.Finished && !_hasBaked)
        {
            StartCoroutine(DelayedBake());
        }
    }

    private void OnDestroy()
    {
        if (_map != null)
        {
            _map.OnUpdated -= BakeNavMesh;
        }
    }

    /// <summary>
    /// The primary method to bake the NavMesh, called by the OnUpdated event.
    /// </summary>
    void BakeNavMesh()
    {
        if (_hasBaked) return; // Don't bake again if we already have

        Debug.Log("Map.OnUpdated event fired. Baking NavMesh...");
        _navMeshSurface.BuildNavMesh();
        _hasBaked = true;
        Debug.Log("NavMesh baking complete.");
    }

    /// <summary>
    /// A delayed coroutine to act as a fallback baking method.
    /// </summary>
    IEnumerator DelayedBake()
    {
        // Wait for a very short time to ensure all map tiles are settled.
        yield return new WaitForSeconds(0.5f);

        if (_hasBaked) yield break; // Exit if the event-based method already ran

        Debug.Log("Fallback bake triggered. Baking NavMesh...");
        _navMeshSurface.BuildNavMesh();
        _hasBaked = true;
        Debug.Log("NavMesh baking complete.");
    }
}
