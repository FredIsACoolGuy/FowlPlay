using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RoundManager : NetworkBehaviour
{
    public GameRulesData gameRules;
    public GameObject pickup;
    private List<Transform> pickupSpawnPoints = new List<Transform>(); 
    int pickupSpawnInterval;
    int currentSeconds = 0;
    public TMP_Text timer;
    private int totalPlayers;
    public int playersInPit = 0;

    [ServerCallback]
    void Start()
    {
        foreach(Transform child in this.transform)
        {
            pickupSpawnPoints.Add(child);
        }

        totalPlayers = GameObject.Find("NetworkManager").GetComponent<NetworkManagerOverride>().GamePlayers.Count;

        currentSeconds = gameRules.timePerRound;

        StartCoroutine(RoundTimer());

        pickupSpawnInterval = gameRules.timePerRound / (gameRules.howCommonArePowerups+1);

        StartCoroutine(PickupSpawner());
    }

    

    [Server]
    private IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(1f);
        currentSeconds--;
        if (currentSeconds >= 0)
        {
            timer.text = displayTime(currentSeconds);
            StartCoroutine(RoundTimer());
        }
    }

    private string displayTime(int seconds)
    {
        string output;
        if (seconds / 60 != 0)
        {
            if(seconds / 60 > 9)
            {
                output = (seconds / 60).ToString() + ":";
            }
            else
            {
                output = "0"+(seconds / 60).ToString() + ":";
            }
        }
        else
        {
            output = "00:";
        }

        if (seconds % 60 > 9)
        {
            output = output + (seconds % 60).ToString();
        }
        else
        {
            output = output + "0" + (seconds % 60).ToString();
        }

        return output;
    }


    public void AnotherBirdPitted()
    {
        playersInPit++;
        if (playersInPit >= totalPlayers)
        {
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerOverride>().EndRound();
        }
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
