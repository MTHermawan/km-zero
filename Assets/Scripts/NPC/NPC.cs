using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f; 
    private Rigidbody rb;
    private bool isMoving = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        if (rb == null || !isMoving) return;

        rb.Move(transform.position + transform.right * (speed / 5), Quaternion.identity);
    }

    public void EnableMove() => isMoving = true;
    public void DisableMove() => isMoving = false;
}
