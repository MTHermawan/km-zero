using UnityEngine;

public class Door : MonoBehaviour
{
    private PlayerController _player => PlayerController.Instance;
    
    public Transform pivot;
    public float speed = 3f;
    private float targetYRotation;
 
    private float defaultYRotation = 0f;
    private Transform player;
    private bool isOpen;
 
    void Start() {
        defaultYRotation = transform.eulerAngles.y;
    }
 
    void Update() {
        pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(0f, defaultYRotation + targetYRotation, 0f), speed * Time.deltaTime);
    }
 
    private void ToggleDoor(Vector3 pos) {
        isOpen = !isOpen;
 
        if (isOpen) {
            Vector3 dir = (pos - transform.position);
            Debug.Log(dir);
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
 
    public void Interact() {
        ToggleDoor(player.position);
    }
 
    public string GetDescription() {
        if (isOpen) return "Close the door";
        return "Open the door";
    }
}
