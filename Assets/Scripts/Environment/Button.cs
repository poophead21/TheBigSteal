using System;
using System.Collections;
using UnityEngine;

public class Button : Interactable
{
    private bool canInteract;
    private bool triggered;

    public bool interactOnStart;
    
    [SerializeField] private Material shiningMaterial;
    [SerializeField] private GameObject buttonTop;
    private Renderer _rendererButtonTop;
    [SerializeField] private GameObject door;
    private Door doorScript;

    private Animation _animation;

    private void Awake()
    {
        _rendererButtonTop = buttonTop.GetComponent<Renderer>();
        _animation  = GetComponent<Animation>();
        
        doorScript  = door.GetComponent<Door>();
    }

    private void Start()
    {
        if (interactOnStart)
        {
            ButtonIsInteractable();
        }
    }

    IEnumerator Interact()
    {
        _animation.Play();
        yield return new WaitForSeconds(0.5f);
        doorScript.OpenDoor();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            triggered = true;
            Debug.Log("trigger entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            triggered = false;
        }
        
    }
    private void OnEnable()
    {
        GameManager.Instance.OnCPActivated += ButtonIsInteractable;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCPActivated -= ButtonIsInteractable;
    }

    private void ButtonIsInteractable()
    {
        canInteract = true;
        
        var materialsCopy = _rendererButtonTop.materials;
        materialsCopy[0] = shiningMaterial;
        _rendererButtonTop.materials = materialsCopy;
        
    }

    public override bool CanInteract(GameObject interactor, InteractionType interactionType)
    {
        return (canInteract);
    }

    public override void Interact(GameObject interactor, InteractionType interactionType)
    {
        StartCoroutine(Interact());
    }
}
