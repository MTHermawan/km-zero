using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInputVector { get; private set; }
    public Vector2 LookInputVector { get; private set; }
    public Vector2 ZoomInputVector { get; private set; }
    public Action onLook;
    public Action onJump;
    public Action onExit;
    public Action onZoom;
    public Action onRotate;
    
    private void OnMove(InputValue inputValue)
    {
        MoveInputVector = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        LookInputVector = value.Get<Vector2>();
        onLook?.Invoke();
    }

    public void OnJump()
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

    public void OnZoom(InputValue value)
    {
        ZoomInputVector = value.Get<Vector2>();
        onZoom?.Invoke();
    }

    public void OnRotate()
    {
        onRotate?.Invoke();
    }
}
