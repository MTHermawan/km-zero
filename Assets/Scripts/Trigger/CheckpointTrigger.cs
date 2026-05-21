using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CarController car))
        {
            car.ArriveAtCheckpoint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CarController car))
        {
            car.LeaveCheckpoint();
        }
    }
}