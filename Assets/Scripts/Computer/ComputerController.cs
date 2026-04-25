using UnityEngine;
using UnityEngine.UI;

public class ComputerController : MonoBehaviour
{
    private PlayerController PlayerController => PlayerController.Instance;
    private PlayerUIController playerUI => PlayerUIController.Instance;

    [Header("Player Config")]
    public Camera playerViewCamera;
    private bool _isUsingComputer = false;

    [Header("Camera")]
    [SerializeField] private VirtualScreen cameraVirtualScreen;
    [SerializeField] private SecurityCameraManager cameraManager;
    [SerializeField] private SecurityCameraUIController cameraUI;

    [Header("Desktop")]
    public VirtualScreen desktopScreen;

    void Start()
    {
        InitializeCameraScreen();
    }

    public void OpenComputer()
    {
        if (playerViewCamera == null) return;

        PlayerController.DisableCamera();
        PlayerController.DisableMovement();

        _isUsingComputer = true;
    
        playerViewCamera.gameObject.SetActive(true);
        cameraVirtualScreen.EnableHit();

        PlayerController.Input.onExit += ExitComputer;
    }

    public void ExitComputer()
    {
        if (playerViewCamera == null) return;

        PlayerController.EnableCamera();
        PlayerController.EnableMovement();
        playerViewCamera.gameObject.SetActive(false);
        _isUsingComputer = false;

        cameraVirtualScreen.DisableHit();
        PlayerController.Input.onExit -= ExitComputer;
    }

    private void InitializeCameraScreen()
    {
        if (cameraManager == null || cameraUI == null) return;

        cameraManager.onCameraSwitched += () =>
        {
            UpdateCameraScreen();
        };

        cameraVirtualScreen.screenCaster = cameraUI.gameObject.GetComponent<GraphicRaycaster>();
        cameraUI.onPreviousCameraClick += cameraManager.PrevCamera;
        cameraUI.onNextCameraClick += cameraManager.NextCamera;

        cameraVirtualScreen.DisableHit();
        cameraManager.ResetState();
    }

    private void UpdateCameraScreen()
    {
        cameraVirtualScreen.screenCamera = cameraManager.CurrentActiveCamera;
        cameraUI.ChangeCanvasCamera(cameraManager.CurrentActiveCamera);
    }

    public void Interact()
    {
        OpenComputer();
    }
}
