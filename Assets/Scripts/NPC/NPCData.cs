using UnityEngine;

public enum Gender { Male, Female }

[CreateAssetMenu(fileName = "NPCData", menuName = "Checkpoint/NPC Data")]
public class NPCData : ScriptableObject
{
    public string fullName;
    public Sprite portrait;
    public string code;         // seperti NIK
    public Gender gender;
    public string licensePlate;
}