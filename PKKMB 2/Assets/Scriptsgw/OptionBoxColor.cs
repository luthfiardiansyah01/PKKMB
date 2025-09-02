using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class OptionBoxColor : MonoBehaviour
{
    public RawImage optionBox;            // ini drag UI background (persegi panjang abu-abu)
    public Color normalColor = Color.gray;
    public Color selectedColor = Color.red;

    private Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleChanged);
        UpdateColor(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        UpdateColor(isOn);
    }

    void UpdateColor(bool isOn)
    {
        if (optionBox != null)
            optionBox.color = isOn ? selectedColor : normalColor;
    }
}