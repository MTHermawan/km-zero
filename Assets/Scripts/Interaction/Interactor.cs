using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform interactorSource;
    public float interactRange;
    private IInteractable currentInteractable;
    private GameObject lastHitObj;

    void Awake()
    {
        interactorSource = interactorSource != null ? interactorSource : Camera.main.transform;
    }

    void Update()
    {
        CheckInteractable();
    }

    private void CheckInteractable()
    {
        Ray r = new(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
        {
            if (hitInfo.collider.gameObject != lastHitObj || currentInteractable == null)
            {
                lastHitObj = hitInfo.collider.gameObject;
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    currentInteractable = interactObj;
                }
                else
                {
                    currentInteractable = null;
                }
            }
        }
        else
        {
            currentInteractable = null;
        }
    }

    public void PerformInteract()
    {
        currentInteractable?.Interact();
    }
}
