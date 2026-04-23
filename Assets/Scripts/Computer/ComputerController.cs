using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerController : MonoBehaviour
{
    public Camera playerViewCamera;
    public Transform cameraTexture;
    public Transform desktopTexture;
    private bool _isUsingComputer = false;
    

    void Start()
    {
        
    }

    void Update()
    {
        if (_isUsingComputer)
        {
            if (InputSystem.actions.FindAction("Quit").WasPressedThisFrame())
            {
                ExitComputer();
            }
        }
    }

    public void OpenComputer()
    {
        if (playerViewCamera == null) return;

        AdvancedPlayerController.Instance.DisableCamera();
        AdvancedPlayerController.Instance.DisableMovement();
        playerViewCamera.gameObject.SetActive(true);
        _isUsingComputer = true;
    }

    public void ExitComputer()
    {   
        if (playerViewCamera == null) return;

        AdvancedPlayerController.Instance.EnableCamera();
        AdvancedPlayerController.Instance.EnableMovement();
        playerViewCamera.gameObject.SetActive(false);
        _isUsingComputer = false;
    }

    public void Interact()
    {
        OpenComputer();
    }

    public void OnQuit()
    {
        
    }
}
