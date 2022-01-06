using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Mirror;

public class NetworkManagerOverride : NetworkManager
{
    //name of menu scene
    [Scene][SerializeField] private string menuScene = string.Empty;
    //minimum number of players needed to start the game
    [SerializeField] private int minPlayers = 0;
    //the prefab for the player display in the lobby
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    //prefab for in game
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    //the player spawn system prefab
    [SerializeField] private GameObject playerSpawnSystem = null;

    //events for different cases
    //functions can be added to these events to run when called
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    //public static List<int> typeNumbers = new List<int>();
    //public static List<int> hatNumbers = new List<int>();
    //public static List<string> playerNames = new List<string>();



    //in lobby list
    public List<NetworkRoomPlayerLobby> RoomPlayers { get; set; } = new List<NetworkRoomPlayerLobby>();   
    //in game list
    public List<NetworkGamePlayer> GamePlayers { get; set;} = new List<NetworkGamePlayer>();

    //load all prefabs that can be spawned into the networked scene
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    //called on client when connecting to the server
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("HOOOWEEE BOYS");

        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();

    }

    //called on client when disconnecting from the server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        //if there is a steam lobby attached that means this is using Steamworks, so the client has to be disconnected from the steam lobby
        if (GetComponent<SteamLobby>() != null)
        {
            GetComponent<SteamLobby>().ClientDisconnect();
        }
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    //called when a client connects to the server
    public override void OnServerConnect(NetworkConnection conn)
    {
        //if there are too many players then disconnect
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        //if they arent shown the menu scene then disconnect
        //stops players joining mid-game
        if(SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    //spawns in player prefab and links the object to this connection
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
            //if there are no other players in the list then this is set to the leader
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;
            //connects spawned prefab to this specific connection
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            Debug.Log("CONN ID:" + conn.connectionId);
           // roomPlayerInstance.conn = conn;
        }
    }

    //called on the server whenever a client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        //if the connection is linked to a gameobject
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            //removes this gameobject from the list
            RoomPlayers.Remove(player);
            //resets the display
            NotifyPlayersOfReadyState();

        }
        //does the base code fot server disconnects
        base.OnServerDisconnect(conn);
    }

    //when ever the server stops
    public override void OnStopServer()
    {
        //clears list of players
        Debug.Log("FRED IS GREAT AT CODE");
        RoomPlayers.Clear();
    }

    //loops through all player in list and checks if they are ready to start
    public void NotifyPlayersOfReadyState()
    {
        foreach(NetworkRoomPlayerLobby player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    //checks if the players are readied
    private bool IsReadyToStart()
    {
        //if there aren't enough players then return false - the game cant start
        //if (numPlayers < minPlayers)
        //{
        //    return false;
        //}
        //checks each player in the list
        //foreach (NetworkRoomPlayerLobby player in RoomPlayers)
        //{
        //    if (!player.IsReady)
        //    {
        //        return false;
        //    }
        //}
        //if all the players are ready then return true
        Debug.Log(numPlayers);
        return true;
    }

    //Starts the game - switches to the other scene
    public void StartGame()
    {
        if(SceneManager.GetActiveScene().path == menuScene)
        {
           if (!IsReadyToStart()) { 
                return; 
            }

            Debug.Log("LIST LENGTH: " + RoomPlayers.Count);

            ServerChangeScene("Map_01");
        }
    }




    //changes the scene on the server
    public override void ServerChangeScene(string newSceneName)
    {
        if(SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("Map"))
        {
           for (int i = 0; i< RoomPlayers.Count; i++)
            {
                RoomPlayers[i].UpdateDataHolder();
                //var conn = RoomPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                //gameplayerInstance.SetHatNum(RoomPlayers[i].hatNum);
                //gameplayerInstance.SetTypeNum(RoomPlayers[i].typeNum);
                //gameplayerInstance.GetComponent<CharacterLookScript>().playerStart(i);
                //NetworkServer.Destroy(conn.identity.gameObject);
                //NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }

            Debug.Log("Peenus");
            //Debug.Log(GamePlayers[0].gameObject + "  " + GamePlayers[1].gameObject);
        }
        //does base code for server change scene
        base.ServerChangeScene(newSceneName);
    }


    //called when server scene has succesfully changed 
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("Peen");
        //if the scene starts with "Map" - if the scene is a game scene and not a menu
        if (sceneName.StartsWith("Map"))
        {
            //instantiates player spawn system prefab
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            //spawns it on the network server
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }

    }

    //called on server when client is ready
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    //only runs on the server
    [Server]
    public void ChangeType(int playerNum)
    {
        Debug.Log(playerNum);
    }


}
