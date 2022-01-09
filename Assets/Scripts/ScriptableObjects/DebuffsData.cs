using UnityEngine;

[CreateAssetMenu(fileName = "DebuffsData", menuName = "ScriptableObjects/DebuffsData", order = 1)]
public class DebuffsData : ScriptableObject
{
    public debuff[] debuffs;
}
[System.Serializable]
public class debuff
{
    public string name;
    public Sprite image;
    public string description;
}




