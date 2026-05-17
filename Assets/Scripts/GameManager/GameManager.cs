using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    }

    // Update is called once per frame
    void Update()
    {

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
