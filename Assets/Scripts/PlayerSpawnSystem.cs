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
        // Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
        Transform spawnPoint = spawnPoints[nextIndex];

        //if there isnt a spawnpoint then return an error
        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player{nextIndex}");
            return;
        }

        //connects the playerInstance to the connection
        //GameObject playerInstance = conn.identity.gameObject;
        //playerInstance.transform.position = spawnPoints[nextIndex].position;
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, Quaternion.identity);
            //change player appearance
            //NetworkServer.Spawn(playerInstance, conn);
            NetworkServer.Spawn(playerInstance);
            NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject, true);
            playerInstance.GetComponent<CharacterLookScript>().playerStart();


       // CallLooks(playerInstance.GetComponent<NetworkGamePlayer>());
        

        nextIndex++;
    }

    public void CallLooks(NetworkGamePlayer ngp)
    {
        //sets player position
        Debug.Log("Its happening");
        //playerInstance.transform.position = spawnPoints[posNum].position;
        //Debug.Log(playerInstance.transform.position);
        //Room.GamePlayers[nextIndex].transform.position = spawnPoints[posNum].position;
        //Debug.Break();
        //calls player start to set up looks

        ngp.SetDisplayName(NetworkManagerOverride.playerNames[nextIndex]);
        ngp.SetSkinNum(NetworkManagerOverride.typeNumbers[nextIndex]);
        ngp.SetHatNum(NetworkManagerOverride.hatNumbers[nextIndex]);
        ngp.SetPlayerNum(nextIndex);
        //AddToGamePlayers(ngp);
        Debug.Log(Room.GamePlayers.Count);
        ngp.GetComponent<CharacterLookScript>().playerStart();
    }

    [Command]
    public void AddToGamePlayers(NetworkGamePlayer ngp)
    {
        Room.GamePlayers.Add(ngp);
    }
}
