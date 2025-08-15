using UnityEngine;

public class BuildingTrigger : MonoBehaviour
{
    public string buildingId;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered || !other.CompareTag("Player"))
        {
            return; // Keluar dari fungsi jika bukan Player atau sudah di-trigger
        }

        Debug.Log("✔️ Player memasuki gedung dengan ID: " + buildingId);

        hasBeenTriggered = true;
        
    }
}