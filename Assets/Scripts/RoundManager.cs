using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public GameRulesData gameRules;

    void Start()
    {
        StartCoroutine(RoundTimer());
        StartCoroutine(PickupSpawner());
    }

    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(gameRules.timePerRound);
    }

    private IEnumerator PickupSpawner()
    {
        yield return new WaitForSeconds(gameRules.timePerRound);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
