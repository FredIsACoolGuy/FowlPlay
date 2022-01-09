using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "ScriptableObjects/SettingsData", order = 1)]
public class SettingsHolder : ScriptableObject
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
}
