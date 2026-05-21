using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    private GameManager GameManager => GameManager.Instance;
    private PlayerInput UIPlayerInput;

    [SerializeField] private GameObject blackpanel;
    [Header("Overlay")]
    [SerializeField] private GameObject crosshairPanel;
    private HashSet<string> _crosshairDisableSet = new();

    [Header("Inspect Camera")]
    [SerializeField] private Canvas inspectCanvas;
    [SerializeField] private GameObject inspectBackground;

    [Header("Dialogue")]
    [SerializeField] private GameObject textBoxContainer;
    [SerializeField] private TMP_Text textBoxContent;

    [Header("Action Key")]
    [SerializeField] private LayoutGroup actionKeyGroup;
    [SerializeField] private GameObject actionKeyItemPrefab;
    private Dictionary<string, string> keyList = new();

    [Header("Time Display")]
    [SerializeField] private TMP_Text timeDisplay;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Shift Result")]
    [SerializeField] private GameObject shiftResultPanel;
    [SerializeField] private TMP_Text wrongCarText;
    


    void Awake()
    {
        RefreshUIState();
        UIPlayerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        ClearActionKeys();
        InitializeTimeDisplay();
        TimeManager.Instance.StartTime();
        TimeManager.Instance.onTimeEnd += ShowShiftResult;
    }

    public void RefreshUIState()
    {
        crosshairPanel?.SetActive(_crosshairDisableSet.Count <= 0);

        // RefreshActionKeys();
    }

    public void DisableCrosshair(string disableId)
    {
        _crosshairDisableSet.Add(disableId);
        RefreshUIState();
    }
    public void EnableCrosshair(string disableId)
    {
        _crosshairDisableSet.Remove(disableId);
        RefreshUIState();
    }

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

    public void ClearActionKeys()
    {
        ActionKeyUI[] _allKeyUI = GetAllKeyUI();
        foreach (ActionKeyUI k in _allKeyUI)
        {
            Destroy(k.gameObject);
        }
    }

    public void AddActionKey(string actionKey, string actionLabel)
    {
        // Debug.Log($"AddActionKey: {actionKey} - {actionLabel}");

        keyList[actionKey] = actionLabel;
        ActionKeyUI existedKey = GetKeyUI(actionKey);
        if (existedKey != null)
        {
            existedKey.UpdateLabel(actionLabel);
            return;
        }

        GameObject go = Instantiate(actionKeyItemPrefab, actionKeyGroup.transform);
        ActionKeyUI newKey = go.GetComponent<ActionKeyUI>();
        newKey.Setup(actionKey, actionLabel);
        RefreshLayoutGroup(actionKeyGroup);
    }

    public void DeleteActionKey(string actionKey)
    {
        // Debug.Log($"DeleteActionKey: {actionKey}");
        if (!keyList.ContainsKey(actionKey)) return;

        ActionKeyUI keyUI = GetKeyUI(actionKey);
        if (keyUI != null) DestroyImmediate(keyUI.gameObject); // ← ganti Destroy
        keyList.Remove(actionKey);
    }

    public void DeleteActionKey(string actionKey, string actionLabel)
    {
        // Debug.Log($"DeleteActionKey: {actionKey} - {actionLabel}");

        if (!keyList.ContainsKey(actionKey)) return;
        if (keyList[actionKey] != actionLabel) return;

        Destroy(GetKeyUI(actionKey)?.gameObject);
        keyList.Remove(actionKey);
    }

    public ActionKeyUI[] GetAllKeyUI()
    {
        if (actionKeyGroup == null) return null;

        return actionKeyGroup.GetComponentsInChildren<ActionKeyUI>(true);
    }

    public ActionKeyUI GetKeyUI(string key)
    {
        if (actionKeyGroup != null)
        {
            foreach (ActionKeyUI k in GetAllKeyUI())
            {
                if (k.Key == key) return k;
            }
        }
        return null;
    }

    public void RefreshActionKeys()
    {
        ClearActionKeys();
        foreach (var k in keyList)
        {
            AddActionKey(k.Key, k.Value);
        }
    }

    public void RefreshLayoutGroup(LayoutGroup layoutGroup)
    {
        // layoutGroup.enabled = false;
        // Canvas.ForceUpdateCanvases();
        // layoutGroup.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        LayoutGroup[] childLayouts = layoutGroup.transform.GetComponentsInChildren<LayoutGroup>();
        foreach (LayoutGroup l in childLayouts)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(l.GetComponent<RectTransform>());
        }
    }

    public void UpdateTimeDisplay(string timeString)
    {
        if (timeDisplay != null)
        {
            timeDisplay.SetText(timeString);
        }
    }

    public void InitializeTimeDisplay()
    {
        TimeManager.Instance.onDisplayTimeChanged += (hour, minute) =>
        {
            int displayHour = hour % 12;
            if (displayHour == 0) displayHour = 12;
            string period = hour >= 12 ? "PM" : "AM";
            UpdateTimeDisplay($"{displayHour:00}:{minute:00} {period}");
        };
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        PlayerController.Instance.UnlockCursor(); 
    }

    public void ShowShiftResult()
    {
        shiftResultPanel.SetActive(true);
    }

    public void TogglePauseMenu(bool isActive)
    {
        if (pauseMenu == null) return;

        pauseMenu.SetActive(isActive);

        if (isActive)
        {
            GameManager.PauseGame();
            // Rebuild dari root layout pause menu
            LayoutGroup rootLayout = pauseMenu.GetComponentInChildren<LayoutGroup>();
            RefreshLayoutGroup(rootLayout);
        }
        else
        {
            GameManager.ResumeGame();
        }
    }

    public void ContinueButton()
    {
        GameManager.CreditScene();
    }

    public void RestartButton()
    {
        GameManager.ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeAndLoadScene(string sceneName, float fadeDuration)
    {
        float elapsedTime = 0f;
        blackpanel.SetActive(true);
        Image panelImage = blackpanel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            panelImage.color = panelColor;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void SetWrongCarText(string currentMistake, string maxMistake)
    {
        if (wrongCarText != null)
        {
            wrongCarText.SetText($"{currentMistake}/{maxMistake}");
        }
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
