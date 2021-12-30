using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "ScriptableObjects/GameRulesData", order = 1)]
public class GameRulesData : ScriptableObject
{
    public int numOfRounds;
    public int timePerRound;
    public int howCommonArePowerups;
}
