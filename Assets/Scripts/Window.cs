using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    public Transform pivot;
    // public float speed = 0;
    public float duration = 3f;
    private Vector3 defaultScale;
    // private float targetScale;
    private float _toggleTimer = 0f;
    private float toggleTimer
    {
        get => _toggleTimer;
        set
        {
            _toggleTimer = Mathf.Clamp(value, 0, duration);
        }
    }

    private bool isOpen = true;

    void Start()
    {
        defaultScale = pivot.localScale;
    }

    void Update()
    {
        if (isOpen)
        {
            toggleTimer += Time.deltaTime;
        }
        else
        {
            toggleTimer -= Time.deltaTime;
        }

        pivot.localScale = Vector3.Lerp(defaultScale, new Vector3(pivot.localScale.x, 0, pivot.localScale.z), toggleTimer / duration);
    }

    void ToggleWindow()
    {
        if (pivot == null) return;
        isOpen = !isOpen;
        Debug.Log("Toggle Window");

        // if (isOpen)
        // {
        //     targetScale = 0f;
        // }
        // else
        // {
        //     targetScale = defaultScale;
        // }
    }

    public void Interact()
    {
        ToggleWindow();
    }
}
