using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCDatabase database;
    [SerializeField] private List<GameObject> carPrefabs = new();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CheckpointManager checkpointManager;

    [Header("Spawn Settings")]
    [SerializeField] private float intervalMin = 5f;
    [SerializeField] private float intervalMax = 15f;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Anomaly Settings")]
    [SerializeField, Range(0f, 1f)] private float anomalyChance = 0.3f;

    [Header("Queue Settings")]
    [Tooltip("Mobil berikutnya hanya spawn setelah mobil sebelumnya selesai di checkpoint")]
    [SerializeField] private bool waitForCheckpointClear = true;

    private Coroutine _spawnRoutine;
    private CarController _currentCar;
    private bool _checkpointOccupied = false;

    private static CarSpawner s_instance;
    public static CarSpawner Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<CarSpawner>();
            return s_instance;
        }
    }

    void Start()
    {
        if (checkpointManager == null)
            checkpointManager = CheckpointManager.Instance;
        
        SpawnCar();

        if (spawnOnStart)
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (_spawnRoutine != null) return;
        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (_spawnRoutine == null) return;
        StopCoroutine(_spawnRoutine);
        _spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = Random.Range(intervalMin, intervalMax);
            yield return new WaitForSeconds(interval);

            if (waitForCheckpointClear)
                yield return new WaitUntil(() => !_checkpointOccupied);

            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        if (carPrefabs.Count == 0 || spawnPoint == null) return;

        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        if (prefab == null) return;

        GameObject go = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        if (!go.TryGetComponent(out CarController car))
        {
            Debug.LogWarning("[CarSpawner] Prefab tidak memiliki CarController.");
            Destroy(go);
            return;
        }

        // Override anomaly berdasarkan chance
        bool isAnomaly = Random.value < anomalyChance;
        car.SetAnomaly(isAnomaly);
        car.SetDatabase(database);
        car.GenerateInstanceData();

        // Subscribe event checkpoint
        car.onArrivedAtCheckpoint += OnCarArrivedAtCheckpoint;
        car.onCarLeft += OnCarLeft;

        _currentCar = car;
        _checkpointOccupied = false;
    }

    private void OnCarArrivedAtCheckpoint(CarController car)
    {
        _checkpointOccupied = true;
        checkpointManager?.OnCarArrived(car);
    }

    private void OnCarLeft(CarController car)
    {
        _checkpointOccupied = false;
        checkpointManager?.OnCarLeft();
        _currentCar = null;
    }
}