using UnityEngine;

public enum InteractionType
{
    PRIMARY, SECONDARY
}

public abstract class Interactable : MonoBehaviour
{
    public abstract bool CanInteract(GameObject interactor, InteractionType interactionType);
    
    public abstract void Interact(GameObject interactor, InteractionType interactionType);
}
