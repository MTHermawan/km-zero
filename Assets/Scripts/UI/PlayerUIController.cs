using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private GameObject blackscreen;
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


    void Awake()
    {
        RefreshUIState();
    }

    void Start()
    {
        ClearActionKeys();
    }

    public void RefreshUIState()
    {
        crosshairPanel?.SetActive(_crosshairDisableSet.Count <= 0);

        ClearActionKeys();

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
        if (!keyList.ContainsKey(actionKey)) return;
        Destroy(GetKeyUI(actionKey).gameObject);
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
