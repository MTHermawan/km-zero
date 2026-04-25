using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInputVector { get; private set; }
    public Vector2 LookInputVector { get; private set; }
    public Action onLook;
    public Action onJump;
    public Action onExit;
    
    private void OnMove(InputValue inputValue)
    {
        MoveInputVector = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        LookInputVector = value.Get<Vector2>();
        onLook?.Invoke();
    }

    public void OnJump(InputValue value)
    {
        onJump?.Invoke();
    }

    public void OnInteract()
    {
        if (TryGetComponent(out Interactor interactable))
        {
            interactable.PerformInteract();
        }
    }

    public void OnExit()
    {
        onExit?.Invoke();
    }
}
