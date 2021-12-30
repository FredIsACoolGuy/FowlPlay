using UnityEngine;

[CreateAssetMenu(fileName ="Data", menuName ="ScriptableObjects/PlayerCustomizationData", order = 1)]
public class PlayerCustomizationData : ScriptableObject
{
    public Color[] playerColours;

    [Header("Bird Type")]
    public GameObject[] typeMeshes;
    public string[] typeNames;

    [Header("Hats")]
    public GameObject[] hatMeshes;
    public string[] hatNames;
}
