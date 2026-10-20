using System;
using System.Collections;
using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    public bool canInteract = false;
    private bool triggered = false;
    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && triggered && canInteract)
        {
            StartCoroutine(Interact());
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
}
