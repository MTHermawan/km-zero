using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDCardController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Renderer portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private TMP_Text genderText;
    [SerializeField] private TMP_Text licensePlateText;

    private static IDCardController s_instance;
    public static IDCardController Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<IDCardController>();
            return s_instance;
        }
    }

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ShowCard(CarInstanceData data)
    {
        if (portraitImage != null)   portraitImage.material.mainTexture = data.displayPortrait.texture;
        if (nameText != null)        nameText.SetText(data.displayName);
        if (codeText != null)        codeText.SetText(data.displayCode);
        if (genderText != null)      genderText.SetText(data.displayGender.ToString());
        if (licensePlateText != null) licensePlateText.SetText(data.displayLicensePlate);

        gameObject.SetActive(true);
    }

    public void HideCard()
    {
        gameObject.SetActive(false);
    }
}