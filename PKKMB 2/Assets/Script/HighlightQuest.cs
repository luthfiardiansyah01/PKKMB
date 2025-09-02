using UnityEngine;

public class HighlightQuest : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    public float pulseSpeed;

    private Material questMat;
    private Color originalEmission;
    private bool isPlayerNear = false;

    void Start()
    {
        questMat = GetComponent<MeshRenderer>().material;

        // Ambil emission asli (atau hitam kalau belum ada)
        if (questMat.IsKeywordEnabled("_EMISSION"))
            originalEmission = questMat.GetColor("_EmissionColor");
        else
            originalEmission = Color.black;
    }

    void Update()
    {
        if (isPlayerNear)
        {
            // bikin efek nyala-pudar
            float emissionStrength = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            Color finalColor = highlightColor * Mathf.LinearToGammaSpace(emissionStrength * 2f);

            questMat.SetColor("_EmissionColor", finalColor);
            questMat.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player masuk area quest");
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player keluar area quest");
            isPlayerNear = false;

            // balikin ke emission normal
            questMat.SetColor("_EmissionColor", originalEmission);
        }
    }
}