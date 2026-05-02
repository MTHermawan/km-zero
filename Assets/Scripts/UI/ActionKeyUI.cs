using TMPro;
using UnityEngine;

public class ActionKeyUI : MonoBehaviour
{

    [SerializeField] private TMP_Text textActionKey;
    [SerializeField] private TMP_Text textActionLabel;
    private string _key = "";
    public string Key
    {
        get => _key;
        private set
        {
            if (_key != value)
            {
                _key = value;
                textActionKey.SetText(Key);
            }
        }
    }
    private string _label = "";
    public string Label
    {
        get => _label;
        private set
        {
            if (_label != value)
            {
                _label = value;
                textActionLabel.SetText(Label);
            }
        }
    }

    public void Setup(string actionKey, string actionLabel)
    {
        Key = actionKey;
        Label = actionLabel;
    }

    public void UpdateLabel(string newLabel)
    {
        Label = newLabel;
    }
}
