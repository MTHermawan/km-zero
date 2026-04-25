using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    [Serializable]
    public class NPCInfo
    {
        [SerializeField] private int _id;
        [SerializeField] private string _npcName;
        [SerializeField] private GameObject _prefab;

        public int GetID() => _id;
        public string GetName() => _npcName; 
        public GameObject GetPrefab() => _prefab;
    }

    [SerializeField] private NPCInfo[] npcs;

    public NPCInfo[] GetAllNPC() => npcs;
    
    public NPCInfo GetRandomNPC()
    {
        return npcs[UnityEngine.Random.Range(0, npcs.Length)];
    }

    public GameObject GetRandomNPCObject()
    {
        return npcs[UnityEngine.Random.Range(0, npcs.Length)].GetPrefab();
    }

    public NPCInfo GetNPCById(int id)
    {
        return npcs.FirstOrDefault(x => x.GetID() == id);
    }
}
