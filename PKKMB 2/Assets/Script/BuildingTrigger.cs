using UnityEngine;

public class BuildingTrigger : MonoBehaviour
{
    [SerializeField] public string buildingId;
    private GameObject questionMark;

    void Start()
    {
        // Cari child bernama "QuestionMark" dari building ini
        Transform qmTransform = transform.Find("QuestionMark");
        if (qmTransform != null)
        {
            questionMark = qmTransform.gameObject;
            questionMark.SetActive(false); // Pastikan awalnya mati
        }
        else
        {
            Debug.LogWarning($"Building tidak punya child QuestionMark!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Aktifkan question mark
        if (questionMark != null)
        {
            questionMark.SetActive(true);
            Debug.Log($"Question mark untuk {buildingId} ditampilkan");
        }
    }

     private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Aktifkan question mark
        if (questionMark != null)
        {
            questionMark.SetActive(false);
            Debug.Log($"Question mark untuk {buildingId} ditutup");
        }
    }
}
