using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : Interactable
{
    [Header("References")]
    [SerializeField] private NPCDatabase database;
    [SerializeField] private List<Renderer> carRenderers = new();
    [SerializeField] private List<Material> colorTargetMaterials = new();
    [SerializeField] private SpriteRenderer driverPortraitRenderer;

    [Header("Settings")]
    [SerializeField] private bool isAnomaly = false;
    public float decisionTime = 150f;

    public CarInstanceData InstanceData { get; private set; }

    public System.Action<CarController> onArrivedAtCheckpoint;
    public System.Action<CarController> onCarLeft;
    public System.Action<float> onTimerTick;   // sisa waktu, untuk UI
    public System.Action onTimerExpired;

    private Vector2 _basePortraitSize;
    private Vector3 _basePortraitScale;
    private bool _portraitInitialized;
    private bool _hasDecided = false;
    private Coroutine _decisionTimer;

    public void SetAnomaly(bool anomaly) => isAnomaly = anomaly;
    public void SetDatabase(NPCDatabase db) => database = db;

    protected override void Start()
    {
        base.Start();
        if (InstanceData == null)
            GenerateInstanceData();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void GenerateInstanceData()
    {
        if (database == null || database.npcs.Count == 0) return;
        NPCData source = database.npcs[Random.Range(0, database.npcs.Count)];
        InstanceData = CarInstanceData.Create(source, isAnomaly, database);
        ApplyVisuals();
    }

    public void ArriveAtCheckpoint()
    {
        _hasDecided = false;    
        _decisionTimer = StartCoroutine(DecisionTimerCoroutine());
        onArrivedAtCheckpoint?.Invoke(this);
    }

    public void LeaveCheckpoint()
    {
        StopDecisionTimer();
        onCarLeft?.Invoke(this);
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }

    private IEnumerator DecisionTimerCoroutine()
    {
        float remaining = decisionTime;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            onTimerTick?.Invoke(Mathf.Max(remaining, 0f));
            yield return null;
        }

        if (!_hasDecided)
        {
            onTimerExpired?.Invoke();
            // Timeout = dianggap mistake apapun kondisinya
            CheckpointScoreManager.Instance?.RegisterMistake();
            LeaveCheckpoint();
        }
    }

    private void StopDecisionTimer()
    {
        if (_decisionTimer != null)
        {
            StopCoroutine(_decisionTimer);
            _decisionTimer = null;
        }
    }

    private void SnapCar()
    {
        if (_hasDecided) return;
        _hasDecided = true;

        if (isAnomaly)
            CheckpointScoreManager.Instance?.RegisterCorrect();
        else
            CheckpointScoreManager.Instance?.RegisterMistake();

        LeaveCheckpoint();
    }

    public override void Interact()
    {
        Debug.Log($"Interacted with car: {InstanceData.sourceData.name}, Anomaly: {InstanceData.isAnomaly}");
        SnapCar();
    }

    private void ApplyPortrait(Sprite sprite)
    {
        if (driverPortraitRenderer == null || sprite == null) return;

        if (!_portraitInitialized)
        {
            Sprite original = driverPortraitRenderer.sprite;
            if (original != null)
            {
                _basePortraitSize = original.bounds.size;
                _basePortraitScale = driverPortraitRenderer.transform.localScale;
            }
            _portraitInitialized = true;
        }

        driverPortraitRenderer.sprite = sprite;

        if (_basePortraitSize == Vector2.zero) return;

        Vector2 newSize = sprite.bounds.size;
        Vector3 scale = _basePortraitScale;
        scale.x *= _basePortraitSize.x / newSize.x;
        scale.y *= _basePortraitSize.y / newSize.y;
        driverPortraitRenderer.transform.localScale = scale;
    }

    private void ApplyVisuals()
    {
        ApplyCarColor(InstanceData.carColor);
        ApplyPortrait(InstanceData.sourceData.portrait);
    }

    private void ApplyCarColor(Color color)
    {
        if (colorTargetMaterials.Count == 0) return;

        foreach (Renderer r in carRenderers)
        {
            if (r == null) continue;

            Material[] mats = r.materials;
            bool changed = false;

            for (int i = 0; i < mats.Length; i++)
            {
                if (IsTargetMaterial(mats[i]))
                {
                    mats[i].color = color;
                    changed = true;
                }
            }

            if (changed) r.materials = mats;
        }
    }

    private bool IsTargetMaterial(Material mat)
    {
        if (mat == null) return false;
        string matName = mat.name.Replace(" (Instance)", "");
        foreach (Material target in colorTargetMaterials)
        {
            if (target == null) continue;
            if (target.name == matName) return true;
        }
        return false;
    }
}