using System;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraManager : MonoBehaviour
{
    [SerializeField] private List<SecurityCamera> securityCameras = new();
    private int _currentCameraIndex;
    public int CurrentCameraIndex
    {
        get => _currentCameraIndex;
        private set
        {
            _currentCameraIndex = Mathf.Clamp(value, 0, securityCameras.Count - 1);
            onCameraSwitched?.Invoke();
        }
    }
    public Action onCameraSwitched;

    public Camera CurrentActiveCamera => securityCameras[CurrentCameraIndex].CameraView != null ? securityCameras[CurrentCameraIndex].CameraView : null;

    void Start()
    {
        onCameraSwitched += ReloadCameraRender;
    }

    private void ReloadCameraRender()
    {
        if (CurrentCameraIndex < 0) return;

        for (int i = 0; i < securityCameras.Count; i++)
        {
            if (CurrentCameraIndex == i) securityCameras[i].EnableCamera();
            else securityCameras[i].DisableCamera();
        }
        // CameraUI.ChangeCanvasCamera(CurrentActiveCamera);
    }

    public void ResetState()
    {
        CurrentCameraIndex = 0;
    }

    public void NextCamera()
    {
        Debug.Log("Next Camera");
        CurrentCameraIndex = (CurrentCameraIndex + 1) % securityCameras.Count;
    }
    public void PrevCamera()
    {
        Debug.Log("Prev Camera");
        CurrentCameraIndex = (securityCameras.Count + CurrentCameraIndex - 1) % securityCameras.Count;
    }
}
