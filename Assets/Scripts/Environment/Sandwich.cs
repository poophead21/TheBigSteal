using UnityEngine;

public class Sandwich : Interactable
{
    public override bool CanInteract(GameObject interactor, InteractionType interactionType)
    {
        return true;
    }

    public override void Interact(GameObject interactor, InteractionType interactionType)
    {
        Debug.Log("Sandwich interacted");
        ManageScenes.Instance.LoadNextLevel();
    }
    
}
