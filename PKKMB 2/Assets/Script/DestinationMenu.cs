using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Button
using TMPro; // Use TextMeshPro for UI text
using System.Collections.Generic; // Required for using Lists

/// <summary>
/// Manages a UI menu that allows the player to select a GPS destination from a list.
/// This script dynamically creates buttons for each destination, assuming they use TextMeshPro.
/// </summary>
public class DestinationMenu : MonoBehaviour
{
    [Header("Destinations")]
    // A list of all possible destination transforms.
    public List<Transform> destinationPoints;

    [Header("UI Settings")]
    // The UI Panel that will contain the destination buttons.
    public GameObject menuPanel;
    // The UI Button prefab to use for each destination.
    public Button buttonPrefab;

    // A reference to the GpsSystem in the scene.
    private GpsSystem gpsSystem;

    void Start()
    {
        // Find the GpsSystem in the scene.
        gpsSystem = FindObjectOfType<GpsSystem>();

        if (gpsSystem == null)
        {
            Debug.LogError("DestinationMenu could not find a GpsSystem in the scene!");
            return;
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        CreateDestinationButtons();
    }

    /// <summary>
    /// Populates the menu panel with buttons, one for each destination.
    /// </summary>
    void CreateDestinationButtons()
    {
        if (buttonPrefab == null || menuPanel == null)
        {
            Debug.LogError("Button Prefab or Menu Panel is not assigned in the Inspector!");
            return;
        }

        foreach (Transform destination in destinationPoints)
        {
            Button newButton = Instantiate(buttonPrefab, menuPanel.transform);

            // Get the TextMeshPro component from the button's children.
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // --- DEBUGGING LINE ADDED ---
                // This will print a message to the console for each button it creates.
                Debug.Log($"Creating button for '{destination.name}' and setting text.");

                buttonText.text = destination.name;
            }
            else
            {
                Debug.LogWarning("Button prefab does not have a TextMeshProUGUI component in its children.", newButton);
            }

            // Add a listener to the button's onClick event.
            newButton.onClick.AddListener(() => SetGpsDestination(destination));
        }
    }

    /// <summary>
    /// Sets the destination on the GpsSystem and closes the menu.
    /// </summary>
    public void SetGpsDestination(Transform destination)
    {
        if (gpsSystem != null)
        {
            gpsSystem.SetDestination(destination);
            // This is the message you were already seeing, which is good!
            Debug.Log("New destination set from menu: " + destination.name);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the visibility of the destination menu.
    /// </summary>
    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }
    }
}
