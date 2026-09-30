using System;
using UnityEngine;

public class TestCheckpointProgression : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private PlayerMovement _playerMovement;
    private bool isInteracting;
    private void Start()
    {
        _playerMovement = _player.GetComponent<PlayerMovement>();
    }
    
    private void OnEnable()
    {
        GameManager.Instance.OnCPIndexReset += SetCPIndex;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.OnCPIndexReset -= SetCPIndex;
    }

    private void SetCPIndex(int cpIndex)
    {
        Debug.Log("CP Index: " + cpIndex);
    }
    
}
