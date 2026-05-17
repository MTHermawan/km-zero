using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private PlayerUIController playerUI => PlayerUIController.Instance;
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
                _currentInteractable = value;
                if (_currentInteractable == null)
                {
                    lastInteractionName = null;
                }
                else
                {
                    lastInteractionName = _currentInteractable.GetInteractionName();
                    Debug.Log($"Current Interactable: {lastInteractionName}");
                }
            }
        }
    }
    [SerializeField] private LayerMask hitLayers;
    private GameObject lastHitObj;
    private string _lastInteractionName;
    public string lastInteractionName
    {
        get => _lastInteractionName;
        private set
        {
            if (_lastInteractionName != value)
            {
                _lastInteractionName = value;
                if (string.IsNullOrEmpty(_lastInteractionName) && CanInteract())
                {
                    playerUI.DeleteActionKey("E");  
                }
                else if (!string.IsNullOrEmpty(_lastInteractionName))
                {
                    playerUI.AddActionKey("E", _lastInteractionName);
                }
            }
        }
    }
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
        if (!CanInteract())
        {
            if (CurrentInteractable != null) CurrentInteractable = null;
            lastHitObj = null;
            return;
        }

        Ray r = new(interactorSource.position, interactorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange, hitLayers))
        {
            if (hitInfo.collider.gameObject != lastHitObj || CurrentInteractable == null)
            {
                lastHitObj = hitInfo.collider.gameObject;
                if (hitInfo.collider.gameObject.TryGetComponent(out InteractTrigger trigger))
                {
                    CurrentInteractable = trigger.InteractableTarget;
                }
                else
                {
                    CurrentInteractable = null;
                    lastInteractionName = null;
                }

                if (CurrentInteractable != null)
                {
                    string latestName = CurrentInteractable.GetInteractionName();
                    if (latestName != lastInteractionName)
                    {
                        lastInteractionName = latestName;
                        playerUI.AddActionKey("E", latestName);
                    }
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
        if (CurrentInteractable == null) return;
        CurrentInteractable.onObjectInteracted?.Invoke();
        lastHitObj = null;
        CurrentInteractable = null;
        lastInteractionName = null;
    }

    public void DisableInteraction(string disableId) => _interactorDisableSet.Add(disableId);
    public void EnableInteract(string disableId) => _interactorDisableSet.Remove(disableId);
    public bool CanInteract() => _interactorDisableSet.Count <= 0;

    private static Interactor s_instance;
    public static Interactor Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<Interactor>();
            }
            return s_instance;
        }
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
