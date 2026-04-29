using TMPro;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject blackscreen;
    [SerializeField] private GameObject crosshairPanel;
    private int _crosshairDisableCount = 0;
    public int CrosshairDisableCount
    {
        get => _crosshairDisableCount;
        private set
        {
            _crosshairDisableCount = Mathf.Max(0, value);
            RefreshUIState();
        }
    }

    [Header("Inspect Camera")]
    [SerializeField] private Canvas inspectCanvas;
    [SerializeField] private GameObject inspectBackground;

    [Header("Dialogue")]
    [SerializeField] private GameObject textBoxContainer;
    [SerializeField] private TMP_Text textBoxContent;
    

    void Awake()
    {
        RefreshUIState();
    }

    public void RefreshUIState()
    {
        crosshairPanel?.SetActive(_crosshairDisableCount <= 0);
    }

    public void EnableCrosshair() => CrosshairDisableCount--;
    public void DisableCrosshair() => CrosshairDisableCount++;

    public void SetInspectCamera(Camera newCam) => inspectCanvas.worldCamera = newCam;
    public void EnableInspectUI()
    {
        inspectBackground.SetActive(true);
    }
    public void DisableInspectUI()
    {
        inspectBackground.SetActive(false);
    }

    public void EnableTextbox() => textBoxContainer.SetActive(true);
    public void DisableTextbox() => textBoxContainer.SetActive(false);
    public void SetTextboxContent(string text)
    {
        textBoxContent.SetText(text);
    }

    private static PlayerUIController s_instance;
    public static PlayerUIController Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<PlayerUIController>();
            }
            return s_instance;
        }
    } 
}
