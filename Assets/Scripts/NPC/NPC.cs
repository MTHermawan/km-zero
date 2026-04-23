using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f; 
    private Rigidbody rb;
    private bool isStopping = false;

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
        if (rb == null || isStopping) return;

        rb.Move(transform.position + transform.right * (speed / 5), Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("StopTrigger"))
        {
            Debug.Log("Stop");
            isStopping = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("StopTrigger"))
        {
            isStopping = false;
        }
    }
}
