using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour, IInteractable
{
    public UnityEvent unityEvent;
    [SerializeField] private string actionText = "Interact";

    public void Interact()
    {
        unityEvent?.Invoke();
    }
}
