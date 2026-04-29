using System;
using UnityEngine;
using UnityEngine.UI;

public class SecurityCameraUIController : MonoBehaviour
{
    [SerializeField] private Canvas _securityCameraCanvas;
    [SerializeField] private Button _previousCameraButton;
    [SerializeField] private Button _nextCameraButton;
    [SerializeField] private Button _exitCameraButton;

    public Action onPreviousCameraClick;
    public Action onNextCameraClick;
    public Action onExitCameraClick;

    void Awake()
    {
        _securityCameraCanvas ??= GetComponent<Canvas>();
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _previousCameraButton?.onClick.AddListener(() => { onPreviousCameraClick.Invoke(); });
        _nextCameraButton?.onClick.AddListener(() => { onNextCameraClick.Invoke(); });
        _exitCameraButton?.onClick.AddListener(() => { onExitCameraClick.Invoke(); });

    //     if (SecurityCameraManager == null) return;

    //     _previousCameraButton?.onClick.AddListener(() => { SecurityCameraManager.PrevCamera(); });
    //     _nextCameraButton?.onClick.AddListener(() => { SecurityCameraManager.NextCamera(); });
        
    }

    public void ChangeCanvasCamera(Camera newCam)
    {
        if (_securityCameraCanvas == null) return;
        
        _securityCameraCanvas.worldCamera = newCam;
    }
}
