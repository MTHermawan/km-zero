using UnityEngine;

public class Door : Interactable
{
    private PlayerController _player => PlayerController.Instance;
    
    public Transform pivot;
    public float speed = 3f;
    private float targetYRotation;
 
    private float defaultYRotation = 0f;
    private bool isOpen;
 
    protected override void Start() {
        base.Start();
        defaultYRotation = transform.eulerAngles.y;
    }
 
    protected override void Update() {
        base.Update();
        pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f), speed * Time.deltaTime);
    }
 
    private void ToggleDoor(Vector3 pos) {
        isOpen = !isOpen;
 
        if (isOpen) {
            Vector3 dir = pos - transform.position;
            targetYRotation = -Mathf.Sign(Vector3.Dot(-transform.forward, dir)) * 90f; 
        } else {
            targetYRotation = 0f;
        }
    }
 
    public void Open(Vector3 pos) {
        if (!isOpen) {
            ToggleDoor(pos);
        }
    }
    public void Close(Vector3 pos) {
        if (isOpen) {
            ToggleDoor(pos);
        }
    }

    public override string GetInteractionName() {
        if (isOpen) return "Close";
        return "Open";
    }
    
    public override void Interact()
    {
        ToggleDoor(_player.transform.position);
    }
}
