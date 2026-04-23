using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecurityCameraManager : MonoBehaviour
{
    [SerializeField] private List<SecurityCamera> securityCameras = new();
    private int _currentCameraIndex ;
    public int currentCameraIndex
    {
        get => _currentCameraIndex;
        set
        {
            _currentCameraIndex = Mathf.Clamp(value, 0, securityCameras.Count - 1);
            RefreshCameraState();
        }
    }

    private void RefreshCameraState()
    {
        if (currentCameraIndex <= 0) return;

        for (int i = 0; i < securityCameras.Count; i++)
        {
            securityCameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    private void NextCamera() => currentCameraIndex = (currentCameraIndex + 1) % securityCameras.Count;
    private void PrevCamera() => currentCameraIndex = (currentCameraIndex - 1) % securityCameras.Count;
}
