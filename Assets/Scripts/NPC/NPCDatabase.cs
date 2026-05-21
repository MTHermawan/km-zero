using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCDatabase", menuName = "Checkpoint/NPC Database")]
public class NPCDatabase : ScriptableObject
{
    public List<NPCData> npcs = new();

    public NPCData GetByName(string name) =>
        npcs.Find(n => n.fullName.ToLower().Contains(name.ToLower()));

    public NPCData GetByCode(string code) =>
        npcs.Find(n => n.code == code);

    public List<NPCData> SearchByName(string query) =>
        npcs.FindAll(n => n.fullName.ToLower().Contains(query.ToLower()));
}