using UnityEngine;
using TMPro;
using UnityEngine.EventSystems; // Required for hover detection

public class TextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMeshPro;

    // Set your colors in the Inspector
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    void Start()
    {
        // Automatically get the TextMeshPro component attached to this GameObject
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.color = normalColor;
    }

    // Detects when the mouse enters the text boundary
    public void OnPointerEnter(PointerEventData eventData)
    {
        textMeshPro.color = hoverColor;
    }

    // Detects when the mouse leaves the text boundary
    public void OnPointerExit(PointerEventData eventData)
    {
        textMeshPro.color = normalColor;
    }
}
