using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundManager : NetworkBehaviour
{
    public GameRulesData gameRules;
    public GameObject pickup;
    private List<Transform> pickupSpawnPoints = new List<Transform>(); 
    int pickupSpawnInterval;

    [Server]
    void Start()
    {
        foreach(Transform child in this.transform)
        {
            pickupSpawnPoints.Add(child);
        }

        StartCoroutine(RoundTimer());

        pickupSpawnInterval = gameRules.timePerRound / (gameRules.howCommonArePowerups+1);

        StartCoroutine(PickupSpawner());
    }

    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(gameRules.timePerRound);
    }

    private IEnumerator PickupSpawner()
    {
        float waitTime = pickupSpawnInterval + Random.Range(-pickupSpawnInterval / 4f, pickupSpawnInterval / 4f);
        yield return new WaitForSeconds(waitTime);
        GameObject pickupObject = Instantiate(pickup, pickupSpawnPoints[Random.Range(0, pickupSpawnPoints.Count)]);
        NetworkServer.Spawn(pickupObject);
        StartCoroutine(PickupSpawner());
    }
}
