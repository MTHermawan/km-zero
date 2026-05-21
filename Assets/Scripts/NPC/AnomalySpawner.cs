using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalySpawner : MonoBehaviour
{
    [Header("Dataset")]
    [SerializeField] private List<AnomalyData> _anomalyDataset = new();

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnIntervalMin = 8f;
    [SerializeField] private float _spawnIntervalMax = 20f;
    [SerializeField] private float _spawnRadiusMin = 5f;
    [SerializeField] private float _spawnRadiusMax = 18f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Aggressiveness")]
    [Tooltip("Berapa banyak anomali yang bisa aktif sekaligus")]
    [SerializeField, Range(1, 10)] private int _maxConcurrentAnomalies = 1;
    [Tooltip("Probabilitas spawn di depan player (0 = selalu di luar sudut, 1 = selalu di depan)")]
    [SerializeField, Range(0f, 1f)] private float _inFrontChance = 0.3f;

    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Camera _playerCamera;

    [Header("Blocked Areas")]
    [SerializeField] private List<Collider> _blockedAreas = new();

    [Header("Despawn")]
    public float despawnTime = 10f;

    private readonly List<AnomalyInstance> _activeAnomalies = new();
    private Coroutine _spawnRoutine;

    void Start()
    {
        if (_player == null) _player = PlayerController.Instance?.transform;
        if (_playerCamera == null) _playerCamera = Camera.main;

        _spawnRoutine = StartCoroutine(SpawnLoop());
    }



    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(_spawnIntervalMin, _spawnIntervalMax);
            yield return new WaitForSeconds(wait);

            int toSpawn = _maxConcurrentAnomalies - _activeAnomalies.Count;
            for (int i = 0; i < toSpawn; i++)
            {
                SpawnAnomaly();
            }
        }
    }

    private void SpawnAnomaly()
    {
        if (_anomalyDataset == null || _anomalyDataset.Count == 0) return;

        AnomalyData data = _anomalyDataset[Random.Range(0, _anomalyDataset.Count)];
        if (data?.prefab == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        GameObject go = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        AnomalyInstance instance = go.GetComponent<AnomalyInstance>();
        if (instance == null) instance = go.AddComponent<AnomalyInstance>();

        instance.Initialize(data, _playerCamera.transform);
        _activeAnomalies.Add(instance);

        // Fallback despawn dari spawner — jaga-jaga jika AnomalyInstance gagal despawn sendiri
        StartCoroutine(FallbackDespawn(instance, data.despawnTime));
    }

    private IEnumerator FallbackDespawn(AnomalyInstance instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instance != null)
            DespawnAnomaly(instance);
    }

    private Vector3 GetSpawnPosition()
    {
        const int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            bool spawnInFront = Random.value < _inFrontChance;
            float radius = Random.Range(_spawnRadiusMin, _spawnRadiusMax);

            Vector3 direction;
            if (spawnInFront)
            {
                float angle = Random.Range(-45f, 45f);
                direction = Quaternion.Euler(0, angle, 0) * _playerCamera.transform.forward;
            }
            else
            {
                float angle = Random.Range(100f, 260f);
                direction = Quaternion.Euler(0, angle, 0) * _playerCamera.transform.forward;
            }

            direction.y = 0f;
            direction.Normalize();

            Vector3 xzPos = _player.position + direction * radius;
            Vector3 rayOrigin = new Vector3(xzPos.x, _player.position.y + 50f, xzPos.z);

            Vector3 candidatePos;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f, _groundLayer))
            {
                candidatePos = hit.point;
            }
            else if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitFallback, 100f))
            {
                candidatePos = hitFallback.point;
            }
            else
            {
                continue;
            }

            if (IsInsideBlockedArea(candidatePos))
            {
                Debug.Log($"[AnomalySpawner] Attempt {attempt + 1}: posisi di blocked area, retry...");
                continue;
            }

            return candidatePos;
        }

        Debug.LogWarning("[AnomalySpawner] Gagal menemukan posisi valid setelah max attempts.");
        return _player.position;
    }

    public void DespawnAnomaly(AnomalyInstance instance)
    {
        if (_activeAnomalies.Contains(instance))
            _activeAnomalies.Remove(instance);

        if (instance != null)
            Destroy(instance.gameObject);
    }

    public void DespawnAll()
    {
        foreach (var a in _activeAnomalies)
        {
            if (a != null) Destroy(a.gameObject);
        }
        _activeAnomalies.Clear();
    }

    private bool IsInsideBlockedArea(Vector3 pos)
    {
        foreach (Collider area in _blockedAreas)
        {
            if (area == null) continue;
            if (area.bounds.Contains(pos)) return true;
        }
        return false;
    }

    private static AnomalySpawner s_instance;
    public static AnomalySpawner Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<AnomalySpawner>();
            return s_instance;
        }
    }
}