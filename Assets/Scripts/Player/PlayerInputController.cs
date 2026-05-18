using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public CinemachineInputAxisController cinemachineInput;
    public Vector2 MoveInputVector { get; private set; }
    public Vector2 LookInputVector { get; private set; }
    public Vector2 ZoomInputVector { get; private set; }
    public Action onLook;
    public Action onJump;
    public Action onExit;
    public Action onZoom;
    public Action onRotate;
    public Action onInteract;
    public Action onLateInteract;
    public Action onPause;
    
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

    private PlayerUIController playerUI => PlayerUIController.Instance;
    public void OnInteract()
    {
        onInteract?.Invoke();
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

    public void EnablePlayerCinemachineInput()
    {
        cinemachineInput.enabled = true;
    }

    public void DisablePlayerCinemachineInput()
    {
        cinemachineInput.enabled = false;
    }

    public void OnPause()
    {
        onPause?.Invoke();
        playerUI.TogglePauseMenu(!GameManager.Instance.IsPausing);
    }
}
