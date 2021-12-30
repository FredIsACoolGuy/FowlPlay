using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{
    //stores the player prefab
    [SerializeField] private GameObject playerPrefab = null;

    //holds a list of all possible spawn points
    private static List<Transform> spawnPoints = new List<Transform>();

    //holds indexx for which spawn point to use next
    private int nextIndex = 0;

    //adds a spwan point to the list
    public static void AddSpawnPoint(Transform transform)
    {       
        spawnPoints.Add(transform);
        //puts spawn points in order based on Index
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    //remove spawn point from list
    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    //called on the server at start
    public override void OnStartServer()
    {
        NetworkManagerOverride.OnServerReadied += SpawnPlayer;
    }

    //removes player when destroyed
    [ServerCallback]
    private void OnDestroy()
    {
        NetworkManagerOverride.OnServerReadied -= SpawnPlayer;
    }

    //only runs on server
    [Server]
    public void SpawnPlayer(NetworkConnection conn) 
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        //if there isnt a spawnpoint then return an error
        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

        //connects the playerInstance to the connection
        GameObject playerInstance = conn.identity.gameObject;

        //change player appearance
        CallLooks(playerInstance, nextIndex);

        nextIndex++;
    }

    //runs on client
    [ClientRpc]
    public void CallLooks(GameObject playerInstance, int posNum)
    {
        //sets player position
        Debug.Log("Its happening");
        playerInstance.transform.position = spawnPoints[posNum].position;
        //calls player start to set up looks
        playerInstance.GetComponent<CharacterLookScript>().playerStart();
    }
}
