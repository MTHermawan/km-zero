using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraView;
    // [SerializeField] private bool activeCamera = true;
    
    void Awake()
    {
        _cameraView ??= GetComponentInChildren<Camera>().transform;
    }

    void EnableCamera() => _cameraView.gameObject.SetActive(true);
    void DisableCamera() => _cameraView.gameObject.SetActive(false);

    // public bool IsActiveCamera() => activeCamera;
}
