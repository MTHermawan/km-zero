using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum ForwardAxis
    {
        Right,
        Left,
        Forward,
        Back,
        Up,
        Down
    }

    [SerializeField] private float speed = 0.1f;

    [Header("Movement")]
    [SerializeField] private ForwardAxis forwardAxis = ForwardAxis.Right;

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
        if (rb == null || !isMoving)
            return;

        Vector3 moveDir = GetMoveDirection();

        rb.Move(
            transform.position + moveDir * (speed / 5f),
            transform.rotation
        );
    }

    private Vector3 GetMoveDirection()
    {
        return forwardAxis switch
        {
            ForwardAxis.Right => transform.right,
            ForwardAxis.Left => -transform.right,

            ForwardAxis.Forward => transform.forward,
            ForwardAxis.Back => -transform.forward,

            ForwardAxis.Up => transform.up,
            ForwardAxis.Down => -transform.up,

            _ => transform.forward
        };
    }

    public void EnableMove()
    {
        isMoving = true;
    }

    public void DisableMove()
    {
        isMoving = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isMoving   )
        {
            Time.timeScale = 0f;
            PlayerUIController.Instance?.ShowGameOver();
        }
    }
}