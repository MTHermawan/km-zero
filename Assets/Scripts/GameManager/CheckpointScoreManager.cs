using System;
using UnityEngine;

public class CheckpointScoreManager : MonoBehaviour
{
    private PlayerUIController playerUI => PlayerUIController.Instance;

    [Header("Settings")]
    [SerializeField] private int maxCorrect = 99;
    [SerializeField] private int maxMistakes = 5;

    public int CorrectCount { get; private set; }
    public int MistakeCount { get; private set; }
    public int MaxCorrect => maxCorrect;
    public int MaxMistakes => maxMistakes;

    public event Action<int, int> onCorrectChanged;  // current, max
    public event Action<int, int> onMistakeChanged;  // current, max
    public event Action onGameClear;
    public event Action onGameOver;

    private static CheckpointScoreManager s_instance;
    public static CheckpointScoreManager Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<CheckpointScoreManager>();
            return s_instance;
        }
    }

    void Start()
    {
        Reset();
        onGameOver += playerUI.ShowGameOver;
    }

    void OnDestroy()
    {
        onGameOver -= playerUI.ShowGameOver;
    }

    public void RegisterCorrect()
    {
        if (IsGameOver()) return;

        CorrectCount++;
        onCorrectChanged?.Invoke(CorrectCount, maxCorrect);

        if (CorrectCount >= maxCorrect)
            onGameClear?.Invoke();
    }

    public void RegisterMistake()
    {
        if (IsGameOver()) return;
        MistakeCount++;
        onMistakeChanged?.Invoke(MistakeCount, maxMistakes);
        playerUI?.SetWrongCarText(MistakeCount.ToString(), maxMistakes.ToString()); // ← fix
        if (MistakeCount >= maxMistakes)
            onGameOver?.Invoke();
    }

    public void Reset()
    {
        CorrectCount = 0;
        MistakeCount = 0;
        onCorrectChanged?.Invoke(CorrectCount, maxCorrect);
        onMistakeChanged?.Invoke(MistakeCount, maxMistakes);

        PlayerUIController.Instance?.SetWrongCarText(MistakeCount.ToString(), maxMistakes.ToString()); // ← fix
    }

    private bool IsGameOver() => /* CorrectCount >= maxCorrect || */ MistakeCount >= maxMistakes;
}