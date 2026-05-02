using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public Interactable InteractableTarget { get; private set; }

    public void SetInteractable(Interactable target)
    {
        InteractableTarget = target;
    }

    // public UnityEvent unityEvent;

    // public override void Interact()
    // {
    //     unityEvent?.Invoke();
    // }
}
