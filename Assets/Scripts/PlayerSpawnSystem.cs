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

    private NetworkManagerOverride room;
    private NetworkManagerOverride Room
    {
        get
        {
            //if room is currently null it finds it and assigns to room
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerOverride;
        }
    }

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
        Debug.Log("BUTTS");
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
        //Transform spawnPoint = spawnPoints[nextIndex];

        //if there isnt a spawnpoint then return an error
        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

       GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, Quaternion.identity);

        StartCoroutine(delay(playerInstance, nextIndex, conn));
       nextIndex++;
    }


    [Server]
    public IEnumerator delay(GameObject playerInstance, int num, NetworkConnection con)
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Spawn(playerInstance, con);
        //GameObject playerInstance = conn.identity.gameObject;
        CallLooks(playerInstance.GetComponent<NetworkGamePlayer>(), num);
    }

    [ClientRpc]
    public void CallLooks(NetworkGamePlayer ngp, int num)
    {
        //sets player position
        Debug.Log("Its happening");

        ngp.SetDisplayName(NetworkManagerOverride.playerNames[num]);
        ngp.SetTypeNum(NetworkManagerOverride.typeNumbers[num]);
        ngp.SetHatNum(NetworkManagerOverride.hatNumbers[num]);
        ngp.SetPlayerNum(num);
        ngp.GetComponent<CharacterLookScript>().playerStart(num);
    }

}
