using UnityEngine;
using UnityEngine.Events;

public class InteractTrigger : MonoBehaviour, IInteractable
{
    public UnityEvent unityEvent;

    public void Interact()
    {
        unityEvent?.Invoke();
    }
}
