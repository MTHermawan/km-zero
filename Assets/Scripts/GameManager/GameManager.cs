using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerUIController PlayerUI => PlayerUIController.Instance;
    private Transform _player;
    public Transform player
    {
        get
        {
            if (_player == null)
            {
                _player = FindFirstObjectByType<PlayerController>().transform;
            }
            return _player;
        }
    }

    private Dictionary<string, Coroutine> _runningCoroutines = new();
    private bool _isPausing = false;
    public bool IsPausing
    {
        get => _isPausing;
        set
        {
            if (_isPausing != value)
            {
                _isPausing = value;
            }
        }
    }

    void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        InitializeTimeDisplay();
        TimeManager.Instance.StartTime();
        TimeManager.Instance.onTimeEnd += PlayerUI.ShowShiftResult;
    }

    public int GetMaskLayers(int layerMask)
    {
        for (int i = 0; i < 32; i++)
        {
            int value = 1 << i;

            if ((layerMask & value) == value)
                return i;
        }
        return 0;
    }

    public void UniqueCoroutine(string id, IEnumerator newCoroutine)
    {
        KillCoroutine(id);
        _runningCoroutines[id] = StartCoroutine(newCoroutine);
    }

    public void KillCoroutine(string id)
    {
        if (!_runningCoroutines.ContainsKey(id)) return;

        StopCoroutine(_runningCoroutines[id]);
        _runningCoroutines.Remove(id);
    }

    public void InitializeTimeDisplay()
    {
        TimeManager.Instance.onDisplayTimeChanged += (hour, minute) =>
        {
            int displayHour = hour % 12;
            if (displayHour == 0) displayHour = 12;
            string period = hour >= 12 ? "PM" : "AM";
            PlayerUI.UpdateTimeDisplay($"{displayHour:00}:{minute:00} {period}");
        };
    }

    public void PauseGame()
    {
        IsPausing = true;
        PlayerController.Instance.DisableMovement(nameof(IsPausing) + GetInstanceID());
        PlayerController.Instance.UnlockCursor();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPausing = false;
        Time.timeScale = 1f;
        PlayerController.Instance.EnableMovement(nameof(IsPausing) + GetInstanceID());
        PlayerController.Instance.LockCursor();
    }

    public void CreditScene()
    {
        SceneManager.LoadScene("CreditScene");
    }

    public void MainMenu()
    {
        // SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private static GameManager s_instance;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<GameManager>();
                if (s_instance == null)
                {
                    GameObject go = new("GameManager");
                    s_instance = go.AddComponent<GameManager>();
                }
            }
            return s_instance;
        }
    }
}
