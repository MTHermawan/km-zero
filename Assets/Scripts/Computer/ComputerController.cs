using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ComputerController : Interactable
{
    [Serializable]
    private class ComputerApps
    {
        public string name;
        public VirtualScreen virtualScreen;
        public GraphicRaycaster raycaster;
        public Button desktopExecuteable;
    }

    private PlayerController PlayerController => PlayerController.Instance;
    private PlayerUIController playerUI => PlayerUIController.Instance;


    [Header("General")]
    public Camera playerViewCamera;
    private bool _isUsingComputer = false;
    public bool IsUsingComputer
    {
        get => _isUsingComputer;
        private set
        {
            if (_isUsingComputer != value)
            {
                string varId = nameof(IsUsingComputer) + GetInstanceID();
                
                _isUsingComputer = value;
                Debug.Log(_isUsingComputer);
                if (!_isUsingComputer)
                {
                    PlayerController.EnableCamera();
                    PlayerController.EnableMovement(varId);
                    playerViewCamera.gameObject.SetActive(false);
                    PlayerController.LockCursor();
                    playerUI.EnableCrosshair(varId);
                }
                else
                {
                    PlayerController.DisableCamera();
                    PlayerController.DisableMovement(varId);
                    playerViewCamera.gameObject.SetActive(true);
                    PlayerController.UnlockCursor();
                    playerUI.DisableCrosshair(varId);
                }
            }
        }
    }

    [SerializeField] private List<ComputerApps> apps;

    [Header("Camera")]
    [SerializeField] private SecurityCameraManager cameraManager;
    [SerializeField] private SecurityCameraUIController cameraUI;

    [Header("Desktop")]
    public VirtualScreen desktopScreen;
    public GraphicRaycaster desktopRaycaster;

    protected override void Start()
    {
        base.Start();
        InitializeComputer();
    }

    private void OpenComputer()
    {
        if (playerViewCamera == null || IsUsingComputer) return;

        IsUsingComputer = true;
        PlayerController.Input.onExit += ExitComputer;
    }

    private void ExitComputer()
    {
        if (playerViewCamera == null || !IsUsingComputer) return;

        IsUsingComputer = false;
        PlayerController.Input.onExit -= ExitComputer;
    }

    private void InitializeComputer()
    {
        desktopScreen.SetScreenCaster(desktopRaycaster);

        foreach (ComputerApps app in apps)
        {
            app.virtualScreen.SetScreenCaster(app.raycaster);
            app.desktopExecuteable.onClick.AddListener(() => { OpenApp(app.name); });
        }
        CloseAllApps();

        InitializeCameraScreen();
    }

    private void InitializeCameraScreen()
    {
        if (cameraManager == null || cameraUI == null) return;

        cameraManager.onCameraSwitched += () =>
        {
            UpdateCameraScreen();
        };

        cameraUI.onPreviousCameraClick += cameraManager.PrevCamera;
        cameraUI.onNextCameraClick += cameraManager.NextCamera;
        cameraUI.onExitCameraClick += CloseAllApps;

        cameraManager.ResetState();
    }

    private void UpdateCameraScreen()
    {
        apps.FirstOrDefault(x => x.name == "Camera")?.virtualScreen.SetScreenCamera(cameraManager.CurrentActiveCamera);
        cameraUI.ChangeCanvasCamera(cameraManager.CurrentActiveCamera);
    }

    public void OpenApp(string name)
    {
        foreach (ComputerApps app in apps)
        {
            if (app.name == name)
            {
                app.virtualScreen.gameObject.SetActive(true);
                app.virtualScreen.EnableHit();
            }
            else
            {
                app.virtualScreen.gameObject.SetActive(false);
                app.virtualScreen.DisableHit();
            }
        }
    }

    public void CloseAllApps()
    {
        foreach (ComputerApps app in apps)
        {
            app.virtualScreen.gameObject.SetActive(false);
            app.virtualScreen.DisableHit();
        }

        desktopScreen.gameObject.SetActive(true);
        desktopScreen.EnableHit();
    }

    public override void Interact()
    {
        OpenComputer();
    }
}
