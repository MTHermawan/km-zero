using System;
using UnityEngine;

public class AnomalyInstance : MonoBehaviour
{
    private AnomalyData _data;
    private Transform _playerCamera;
    private Camera _camera;
    private SanityManager _sanity;

    [SerializeField] private Collider _visibilityCollider;

    private float _lookTimer = 0f;
    private float _totalLookTime = 0f;
    private readonly Plane[] _frustumPlanes = new Plane[6];

    public void Initialize(AnomalyData data, Transform playerCamera)
    {
        _data = data;
        _playerCamera = playerCamera;
        _camera = playerCamera.GetComponent<Camera>();
        _sanity = SanityManager.Instance;

        if (_visibilityCollider == null)
            _visibilityCollider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        if (_data == null || _playerCamera == null) return;

        FacePlayer();
        bool looked = IsPlayerLooking();

        Debug.DrawLine(
            _camera.transform.position,
            _visibilityCollider.bounds.center,
            looked ? Color.red : Color.green
        );

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _data.despawnTime)
        {
            AnomalySpawner.Instance?.DespawnAnomaly(this);
            return;
        }

        if (looked)
        {
            _totalLookTime += Time.deltaTime;

            _lookTimer += Time.deltaTime;
            if (_lookTimer >= _data.gracePeriod)
                _sanity?.ReduceSanity(_data.sanityDrainPerSecond * Time.deltaTime);

            if (_totalLookTime >= _data.maxLookDuration)
            {
                AnomalySpawner.Instance?.DespawnAnomaly(this);
                return;
            }
        }
        else
        {
            _lookTimer = 0f;
        }
    }

    private void FacePlayer()
    {
        Vector3 dir = _playerCamera.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private bool IsPlayerLooking()
    {
        if (_camera == null || _visibilityCollider == null)
            return false;

        if (!IsBoundsVisibleInViewport())
            return false;

        return HasLineOfSight();
    }

    private bool HasLineOfSight()
    {
        Bounds b = _visibilityCollider.bounds;

        Vector3 origin = _camera.transform.position;

        Vector3 c = b.center;
        Vector3 e = b.extents;

        Span<Vector3> points = stackalloc Vector3[]
        {
        c,

        c + new Vector3( e.x, 0, 0),
        c + new Vector3(-e.x, 0, 0),

        c + new Vector3(0, e.y, 0),
        c + new Vector3(0,-e.y, 0),

        c + new Vector3(0,0, e.z),
        c + new Vector3(0,0,-e.z),

        c + new Vector3( e.x, e.y,0),
        c + new Vector3(-e.x, e.y,0),
        c + new Vector3( e.x,-e.y,0),
        c + new Vector3(-e.x,-e.y,0),

        c + new Vector3( e.x,0, e.z),
        c + new Vector3(-e.x,0, e.z),
        c + new Vector3(0, e.y, e.z),
        c + new Vector3(0,-e.y, e.z),
    };

        int visibleHits = 0;

        foreach (var point in points)
        {
            Vector3 viewport = _camera.WorldToViewportPoint(point);

            // hanya cek titik yang memang masuk layar
            if (viewport.z <= 0 ||
                viewport.x < 0 ||
                viewport.x > 1 ||
                viewport.y < 0 ||
                viewport.y > 1)
                continue;

            Vector3 dir = point - origin;
            float dist = dir.magnitude;

            if (Physics.SphereCast(
    origin,
    0.08f,
    dir.normalized,
    out RaycastHit hit,
    dist + 0.2f
))
            {
                if (hit.collider == _visibilityCollider ||
                    hit.collider.transform.IsChildOf(transform))
                {
                    visibleHits++;

                    // cukup sebagian terlihat
                    if (visibleHits >= 1)
                        return true;
                }
            }
        }

        return false;
    }

    private bool IsBoundsVisibleInViewport()
    {
        Bounds b = _visibilityCollider.bounds;

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        bool hasPointInFront = false;

        foreach (var corner in GetBoundsCorners(b))
        {
            Vector3 view = _camera.WorldToViewportPoint(corner);

            // Abaikan titik di belakang kamera
            if (view.z <= 0)
                continue;

            hasPointInFront = true;

            min.x = Mathf.Min(min.x, view.x);
            min.y = Mathf.Min(min.y, view.y);

            max.x = Mathf.Max(max.x, view.x);
            max.y = Mathf.Max(max.y, view.y);
        }

        if (!hasPointInFront)
            return false;

        // overlap dengan viewport (0..1)
        return
            max.x >= 0f &&
            min.x <= 1f &&
            max.y >= 0f &&
            min.y <= 1f;
    }

    private static Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3 c = bounds.center;
        Vector3 e = bounds.extents;

        return new Vector3[]
        {
        c + new Vector3(-e.x,-e.y,-e.z),
        c + new Vector3( e.x,-e.y,-e.z),
        c + new Vector3(-e.x, e.y,-e.z),
        c + new Vector3( e.x, e.y,-e.z),

        c + new Vector3(-e.x,-e.y, e.z),
        c + new Vector3( e.x,-e.y, e.z),
        c + new Vector3(-e.x, e.y, e.z),
        c + new Vector3( e.x, e.y, e.z),
        };
    }

    private float _spawnTimer = 0f;


}