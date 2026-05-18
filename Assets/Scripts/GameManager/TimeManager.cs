using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float _gameDurationSeconds = 600f; // durasi real-time game
    [SerializeField, Range(0, 100)] private int _timeProgress = 0;
    [SerializeField] private bool _isRunning = false;

    private const int StartHour = 18;
    private const int TotalMinutes = 720;
    private const int StepMinutes = 30;
    private const int TotalSteps = TotalMinutes / StepMinutes;

    private float _elapsedSeconds = 0f;

    public int TimeProgress
    {
        get => _timeProgress;
        set
        {
            int clamped = Mathf.Clamp(value, 0, 100);
            if (_timeProgress == clamped) return;
            _timeProgress = clamped;
            _elapsedSeconds = (_timeProgress / 100f) * _gameDurationSeconds;
            RefreshDisplayTime();
            onProgressChanged?.Invoke(_timeProgress);
        }
    }

    public bool IsRunning => _isRunning;
    public int DisplayHour { get; private set; }
    public int DisplayMinute { get; private set; }
    public string DisplayTimeString { get; private set; }

    public event Action<int> onProgressChanged;
    public event Action<int, int> onDisplayTimeChanged;
    public event Action onTimeEnd;

    private static TimeManager s_instance;
    public static TimeManager Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<TimeManager>();
            return s_instance;
        }
    }

    void Awake()
    {
        if (s_instance != null && s_instance != this) { Destroy(gameObject); return; }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _elapsedSeconds = (_timeProgress / 100f) * _gameDurationSeconds;
        RefreshDisplayTime();
    }

    void Update()
    {
        TickTime();
    }

    private void TickTime()
    {
        if (!_isRunning) return;

        _elapsedSeconds += Time.deltaTime;
        _elapsedSeconds = Mathf.Clamp(_elapsedSeconds, 0f, _gameDurationSeconds);

        int newProgress = Mathf.RoundToInt((_elapsedSeconds / _gameDurationSeconds) * 100);
        if (newProgress != _timeProgress)
        {
            _timeProgress = newProgress;
            RefreshDisplayTime();
            onProgressChanged?.Invoke(_timeProgress);
        }

        if (_elapsedSeconds >= _gameDurationSeconds)
        {
            _isRunning = false;
            onTimeEnd?.Invoke();
        }
    }

    public void StartTime() => _isRunning = true;
    public void StopTime() => _isRunning = false;

    public void ResetTime()
    {
        _isRunning = false;
        _elapsedSeconds = 0f;
        _timeProgress = 0;
        RefreshDisplayTime();
        onProgressChanged?.Invoke(_timeProgress);
    }

    private void RefreshDisplayTime()
    {
        float exactMinutes = (_timeProgress / 100f) * TotalMinutes;
        int step = Mathf.Clamp(Mathf.RoundToInt(exactMinutes / StepMinutes), 0, TotalSteps);
        int snappedMinutes = step * StepMinutes;

        int totalFromStart = StartHour * 60 + snappedMinutes;
        int hour = (totalFromStart / 60) % 24;
        int minute = totalFromStart % 60;

        if (hour == DisplayHour && minute == DisplayMinute) return;

        DisplayHour = hour;
        DisplayMinute = minute;
        DisplayTimeString = $"{DisplayHour:D2}:{DisplayMinute:D2}";

        Debug.Log($"[TimeManager] {_timeProgress}% → {DisplayTimeString}");
        onDisplayTimeChanged?.Invoke(DisplayHour, DisplayMinute);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) return;
        _elapsedSeconds = (_timeProgress / 100f) * _gameDurationSeconds;
        RefreshDisplayTime();
    }
#endif
}