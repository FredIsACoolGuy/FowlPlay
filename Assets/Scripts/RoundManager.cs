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

    [ServerCallback]
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

    [Server]
    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(gameRules.timePerRound);
    }

    [Server]

    private IEnumerator PickupSpawner()
    {
        float waitTime = pickupSpawnInterval + Random.Range(-pickupSpawnInterval / 4f, pickupSpawnInterval / 4f);
        yield return new WaitForSeconds(waitTime);
        int posNum = Random.Range(0, pickupSpawnPoints.Count);
        GameObject pickupObject = Instantiate(pickup, pickupSpawnPoints[posNum].position, Quaternion.identity);
        NetworkServer.Spawn(pickupObject);
        pickupObject.transform.position = pickupSpawnPoints[posNum].position;
        StartCoroutine(PickupSpawner());
    }
}
