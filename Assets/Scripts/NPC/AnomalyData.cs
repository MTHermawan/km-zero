using UnityEngine;

[CreateAssetMenu(fileName = "AnomalyData", menuName = "Horror/Anomaly Data")]
public class AnomalyData : ScriptableObject
{
    public string anomalyName;
    public GameObject prefab;

    [Header("Sanity Drain")]
    [Tooltip("Waktu grace period sebelum sanity mulai berkurang (detik)")]
    public float gracePeriod = 1f;
    [Tooltip("Sanity berkurang per detik setelah grace period")]
    public float sanityDrainPerSecond = 5f;
    [Tooltip("Total waktu maksimal anomali bisa dilihat sebelum menghilang (detik)")]
    public float maxLookDuration = 5f;
    [Header("Despawn")]
public float despawnTime = 10f;
}