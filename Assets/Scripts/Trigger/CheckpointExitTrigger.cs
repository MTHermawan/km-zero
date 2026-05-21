using UnityEngine;

public class CheckpointExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CarController car)) return;
        if (car.InstanceData == null) return;
        Debug.Log($"Car exited checkpoint: {car.InstanceData.sourceData.name}, Anomaly: {car.InstanceData.isAnomaly}");

        if (car.InstanceData.isAnomaly)
            CheckpointScoreManager.Instance?.RegisterMistake();
        else
            CheckpointScoreManager.Instance?.RegisterCorrect();
    }
}