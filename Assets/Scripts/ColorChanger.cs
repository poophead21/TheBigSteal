using UnityEngine;
using TMPro; // Needed to control TextMeshPro components

public class ColorChanger : MonoBehaviour
{
    // Drag your TextMeshPro object here in the Inspector
    public TMP_Text textToChange;

    // This method will run when the button is clicked
    public void ChangeToRandomColor()
    {
        // Generates a completely random color
        textToChange.color = new Color(Random.value, Random.value, Random.value);
    }

    // Alternative: Use a specific color instead of a random one
    public void ChangeToRed()
    {
        textToChange.color = Color.red;
    }
}
