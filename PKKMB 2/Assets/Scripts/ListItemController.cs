// ListItemController.cs
using UnityEngine;
using UnityEngine.UI; // Required for Button
using TMPro;
using System; // Required for Action

public class ListItemController : MonoBehaviour
{
    public Image itemImage;
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

        string imagePath = "BuildingImages/" + data.formal.Replace(" ", "").ToLowerInvariant();
        
        // Load the sprite from the path
        Sprite itemSprite = Resources.Load<Sprite>(imagePath);

        // Check if the sprite was found before assigning it
        if (itemSprite != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.gameObject.SetActive(true); // Ensure the image object is visible
        }
        else
        {
            // If no image is found for this item, hide the image component
            itemImage.gameObject.SetActive(false);
            Debug.LogWarning($"Image not found for item: {data.name} at path: {imagePath}");
        }
    }
}