// ListItemController.cs
using UnityEngine;
using UnityEngine.UI; // Required for Button
using TMPro;
using System; // Required for Action

public class ListItemController : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text formalNameText;
    private Button button;

    void Awake()
    {
        // Get the button component on this game object
        button = GetComponent<Button>();
    }

    // We've updated the Setup method!
    // It now accepts an "Action" which is basically a function to call.
    public void Setup(ItemData data, Action<ItemData> onItemSelected)
    {
        nameText.text = data.name;
        formalNameText.text = data.formal;
        
        // Remove any previous listeners to prevent duplicates, then add the new one.
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onItemSelected(data));
    }
}