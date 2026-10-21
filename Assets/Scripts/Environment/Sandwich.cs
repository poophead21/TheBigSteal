using UnityEngine;

public class Sandwich : Interactable
{
    public override bool CanInteract(GameObject interactor, InteractionType interactionType)
    {
        return true;
    }

    public override void Interact(GameObject interactor, InteractionType interactionType)
    {
        if (Application.isEditor)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; 
#endif
        }
        else
        {
            Application.Quit();
        }
    }
}
