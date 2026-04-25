using System;
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
    public ComputerController computerController;

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
