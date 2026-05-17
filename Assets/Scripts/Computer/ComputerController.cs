using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ComputerController : Interactable
{
    [Serializable]
    private class ComputerApps
    {
        public string name;
        public VirtualScreen virtualScreen;
        public GraphicRaycaster screenUI;
        public Button desktopExecuteable;
        public Camera screenCamera;
    }

    private PlayerController PlayerController => PlayerController.Instance;
    private PlayerUIController PlayerUI => PlayerUIController.Instance;
    private Interactor Interactor => Interactor.Instance;
    private GameManager GameManager => GameManager.Instance;


    [Header("General")]
    public CinemachineCamera playerComputerView;
    private bool _isUsingComputer = false;
    public bool IsUsingComputer
    {
        get => _isUsingComputer;
        private set
        {
            if (_isUsingComputer != value)
            {
                _isUsingComputer = value;
            }
        }
    }

    [SerializeField] private List<ComputerApps> apps;

    [Header("Camera")]
    [SerializeField] private SecurityCameraManager cameraManager;
    [SerializeField] private SecurityCameraUIController cameraUIController;

    [Header("Desktop")]
    public VirtualScreen desktopScreen;
    public Camera desktopCamera;
    public GraphicRaycaster desktopUI;

    protected override void Start()
    {
        base.Start();
        InitializeComputer();
    }

    private void OpenComputer()
    {
        if (playerComputerView == null || IsUsingComputer) return;


        IsUsingComputer = true;
        playerComputerView.gameObject.SetActive(true);

        PlayerController.Input.DisablePlayerCinemachineInput();
        PlayerController.DisableMovement(nameof(IsUsingComputer) + GetInstanceID());
        PlayerController.UnlockCursor();
        Interactor.DisableInteraction(nameof(IsUsingComputer) + GetInstanceID());
        PlayerUI.DisableCrosshair(nameof(IsUsingComputer) + GetInstanceID());
        OpenDesktop();

        PlayerController.Input.onInteract += ExitComputer;
        PlayerUI.AddActionKey("E", "Turn Off");
    }

    private void ExitComputer()
    {
        if (playerComputerView == null || !IsUsingComputer) return;

        IEnumerator ExitComputerCoroutine()
        {
            yield return null;
            IsUsingComputer = false;
            playerComputerView.gameObject.SetActive(false);
            PlayerUI.EnableCrosshair(nameof(IsUsingComputer) + GetInstanceID());
            Interactor.EnableInteract(nameof(IsUsingComputer) + GetInstanceID());
            PlayerController.LockCursor();
            TurnOffPower();

            yield return null;
            yield return new WaitUntil(() => !PlayerController.cinemachineBrain.IsBlending);
            PlayerController.Input.EnablePlayerCinemachineInput();
            PlayerController.EnableMovement(nameof(IsUsingComputer) + GetInstanceID());
            yield return null;

            PlayerController.Input.onExit -= ExitComputer;
        }

        GameManager.UniqueCoroutine(nameof(ExitComputer) + GetInstanceID(), ExitComputerCoroutine());
    }

    private void InitializeComputer()
    {
        desktopScreen?.SetScreenCamera(desktopCamera);
        desktopScreen?.SetScreenCaster(desktopUI);

        foreach (ComputerApps app in apps)
        {
            if (app.virtualScreen == null || app.screenUI == null || app.desktopExecuteable == null) continue;

            app.virtualScreen?.SetScreenCamera(app.screenCamera);
            app.virtualScreen?.SetScreenCaster(app.screenUI);
            app.desktopExecuteable.onClick.AddListener(() => { OpenApp(app.name); });
        }
        InitializeCameraScreen();
        TurnOffPower();
    }

    private void InitializeCameraScreen()
    {
        if (cameraManager == null || cameraUIController == null) return;

        cameraManager.onCameraSwitched += () =>
        {
            UpdateCameraScreen();
        };

        cameraUIController.onPreviousCameraClick += cameraManager.PrevCamera;
        cameraUIController.onNextCameraClick += cameraManager.NextCamera;
        cameraUIController.onExitCameraClick += OpenDesktop;

        cameraManager.ResetState();
    }

    private void UpdateCameraScreen()
    {
        ComputerApps cameraApp = apps.FirstOrDefault(x => x.name == "Camera");
        if (cameraApp != null)
        {
            cameraApp.screenCamera = cameraManager.CurrentActiveCamera;

            cameraApp?.virtualScreen?.SetScreenCamera(cameraApp.screenCamera);
            cameraUIController?.ChangeCanvasCamera(cameraManager.CurrentActiveCamera);
        }
    }

    public void OpenApp(string name)
    {
        foreach (ComputerApps app in apps)
        {
            if (app.name == name)
            {
                app.virtualScreen?.gameObject.SetActive(true);
                app.virtualScreen.EnableHit();
            }
            else
            {
                app.virtualScreen.gameObject.SetActive(false);
                app.virtualScreen.DisableHit();
            }
        }
    }

    public void OpenDesktop()
    {
        foreach (ComputerApps app in apps)
        {
            app.virtualScreen?.gameObject.SetActive(false);
            app.virtualScreen?.DisableHit();
        }

        desktopScreen?.gameObject.SetActive(true);
        desktopScreen?.EnableHit();
    }

    private void TurnOffPower()
    {
        OpenDesktop();

        desktopScreen?.gameObject.SetActive(false);
        desktopScreen?.DisableHit();
    }

    public override void Interact()
    {
        OpenComputer();
    }
}
