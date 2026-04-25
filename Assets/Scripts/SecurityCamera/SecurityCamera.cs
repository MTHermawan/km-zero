using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private Camera _cameraView;
    public Camera CameraView
    {
        get => _cameraView;
        set => _cameraView = value;
    }
    // [SerializeField] private bool activeCamera = true;

    void Awake()
    {
        _cameraView ??= GetComponentInChildren<Camera>();
    }

    public void EnableCamera() => _cameraView.gameObject.SetActive(true);
    public void DisableCamera() => _cameraView.gameObject.SetActive(false);

    // public bool IsActiveCamera() => activeCamera;
}
