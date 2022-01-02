using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;
using Mirror.FizzySteam;


public class DisconnectFromGameButton : MonoBehaviour
{
    //number of scene to load
    public int sceneNum;

    //stores whether the client is also the host
    public bool isLeader = false;
    //the network manager
    private GameObject networkManager;

    void Start()
    {
        //finds the network manager
        networkManager = GameObject.Find("NetworkManager");
    }

    //called when the button is pressed
    public void clicked()
    {

        //this check is here because the game can still be played locally which doesnt use Steamworks
        if (networkManager.GetComponent<SteamLobby>() != null)
        {
            networkManager.GetComponent<SteamLobby>().ClientDisconnect();
        }

        //if the client is also the host this shuts down the server aswell
        if (isLeader)
        {
            //networkManager.GetComponent<NetworkManagerOverride>().StopClient();
            //networkManager.GetComponent<NetworkManagerOverride>().StopServer();
            
            NetworkManager.singleton.StopHost();
            NetworkServer.Shutdown();
            //networkManager.GetComponent<FizzySteamworks>().Shutdown();
        }
        else
        {
            //networkManager.GetComponent<NetworkManagerOverride>().StopClient();
            NetworkManager.singleton.StopClient();
        }

        //if the network manager has the steam lobby component then the steam lobby has to be disconnected



        //destroys this old network manager so a new one can be initialize
        //Destroy(networkManager);
        //load the scene 
        SceneManager.LoadScene(sceneNum, LoadSceneMode.Single);
    }
}
