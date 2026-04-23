using UnityEngine;

public class PortalBarrier : MonoBehaviour
{
    public Transform pivot;
    public Transform stopTrigger;
    public float speed = 0;
    public float openAngle = 90f;
    private float defaultZRotation = 3f;
    private float targetZrotation = 0f;
    private bool isOpen = false;

    void Start()
    {
        defaultZRotation = pivot.eulerAngles.z;
    }

    void Update()
    {
        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(0f, 0f, defaultZRotation + targetZrotation), speed * Time.deltaTime);
        stopTrigger?.gameObject.SetActive(pivot.eulerAngles.z < 50f);
    }

    private void TogglePortal()
    {
        if (pivot == null) return;
        Debug.Log("Toggle Portal.");
        isOpen = !isOpen;

        if (isOpen)
        {
            targetZrotation = defaultZRotation + openAngle;
        }
        else
        {
            targetZrotation = defaultZRotation;
        }
    }

    public void Open(Vector3 pos)
    {
        if (!isOpen)
        {
            TogglePortal();
        }
    }
    public void Close(Vector3 pos)
    {
        if (isOpen)
        {
            TogglePortal();
        }
    }

    public void Interact()
    {
        TogglePortal();
    }
}
