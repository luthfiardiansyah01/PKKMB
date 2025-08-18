using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LinkOpener : MonoBehaviour
{
    public TextMeshProUGUI tmpText;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, Input.mousePosition, null);
        if (linkIndex != -1) // Kalau ada link diklik
        {
        TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
        string url = linkInfo.GetLinkID();
        Application.OpenURL(url);
        }
        Debug.Log("kepencet");
    }
}
