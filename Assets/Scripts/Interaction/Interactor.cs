using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private PlayerUIController _playerUI => PlayerUIController.Instance;
    public Transform interactorSource;
    public float interactRange;
    private Interactable _currentInteractable = null;
    private Interactable CurrentInteractable
    {
        get => _currentInteractable;
        set
        {
            if (_currentInteractable != value)
            {
                if (_currentInteractable != null)
                {
                    _currentInteractable.OnInteractionNameChanged -= OnInteractionNameChanged;
                }
                
                _currentInteractable = value;
                if (_currentInteractable == null)
                {
                    _playerUI.DeleteActionKey("E");
                }
                else
                {
                    _currentInteractable.OnInteractionNameChanged += OnInteractionNameChanged;
                    _currentInteractable.OnRaycastHit();
                    _playerUI.AddActionKey("E", _currentInteractable.GetInteractionName());
                }
            }
        }
    }
    [SerializeField] private LayerMask hitLayers;
    private GameObject lastHitObj;
    private HashSet<string> _interactorDisableSet = new();

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
        if (!CanInteract()) return;

        Ray r = new(interactorSource.position, interactorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, hitLayers))
        {
            if (hitInfo.collider.gameObject != lastHitObj || CurrentInteractable == null)
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out InteractTrigger trigger))
                {
                    CurrentInteractable = trigger.InteractableTarget;
                }
                else
                {
                    CurrentInteractable = null;
                }
            }
        }
        else
        {
            CurrentInteractable = null;
        }
    }

    public void PerformInteract()
    {
        CurrentInteractable?.onObjectInteracted.Invoke();
    }

    public void DisableInteraction(string disableId) => _interactorDisableSet.Add(disableId);
    public void EnableInteract(string disableId) => _interactorDisableSet.Remove(disableId);
    public bool CanInteract() => _interactorDisableSet.Count <= 0;

    private void OnInteractionNameChanged(string newName)
    {
        _playerUI.AddActionKey("E", newName);
    }
}

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private string _interactionName = "Interact";
    public List<Collider> triggers = new();
    public abstract void Interact();
    public virtual string GetInteractionName() => _interactionName;
    public void OnRaycastHit() { SetInteractionName(GetInteractionName()); }
    public Action onObjectInteracted;
    public event Action<string> OnInteractionNameChanged;


    protected virtual void Awake()
    {
        onObjectInteracted += () =>
        {
            Interact();
            SetInteractionName(GetInteractionName());
        };
    }

    protected virtual void Start()
    {
        foreach (var t in triggers)
        {
            t.gameObject.GetOrAddComponent<InteractTrigger>().SetInteractable(this);
        }
    }
    protected virtual void Update() { }

    protected void SetInteractionName(string newInteractionName)
    {
        if (_interactionName != newInteractionName)
        {
            _interactionName = newInteractionName;
            OnInteractionNameChanged?.Invoke(_interactionName);
        }
    }
}
