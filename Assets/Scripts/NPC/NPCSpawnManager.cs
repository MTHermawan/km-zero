using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NPCSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private NPCData npcData;
    private List<GameObject> npcInstances = new();
    private float currentTimer;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        
    }

    void SpawnLogic()
    {
        
    }

    public void Spawn()
    {
        if (spawnPoint == null) return;

        GameObject npc = Instantiate(npcData.GetRandomNPCObject(), spawnPoint.transform.position, Quaternion.identity);
        npcInstances.Add(npc);
    }

    private static NPCSpawnManager s_instance;
    public static NPCSpawnManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<NPCSpawnManager>();
                if (s_instance == null)
                {
                    GameObject go = new("NPC_SpawnManaager");
                    s_instance = go.AddComponent<NPCSpawnManager>();
                }
            }
            return s_instance;
        }
    }
}
