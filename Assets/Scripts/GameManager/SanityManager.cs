using System;
using UnityEngine;

public class SanityManager : MonoBehaviour
{
    [Header("Sanity Settings")]
    [SerializeField, Range(0f, 100f)] private float _sanity = 100f;
    [SerializeField] private float _maxSanity = 100f;

    public float Sanity => _sanity;
    public float SanityPercent => _sanity / _maxSanity;

    public event Action<float> onSanityChanged;
    public event Action onSanityDepleted;

    public void ReduceSanity(float amount)
    {
        if (_sanity <= 0f) return;
        _sanity = Mathf.Clamp(_sanity - amount, 0f, _maxSanity);
        onSanityChanged?.Invoke(_sanity);
        if (_sanity <= 0f) onSanityDepleted?.Invoke();
    }

    public void RestoreSanity(float amount)
    {
        _sanity = Mathf.Clamp(_sanity + amount, 0f, _maxSanity);
        onSanityChanged?.Invoke(_sanity);
    }

    private static SanityManager s_instance;
    public static SanityManager Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<SanityManager>();
            return s_instance;
        }
    }
}