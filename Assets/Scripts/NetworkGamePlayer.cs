using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkGamePlayer : NetworkBehaviour
{
    #region Customization Values
    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar]
    public string DisplayName = "Bird";
    [SyncVar]
    public int typeNum = 0;
    [SyncVar]
    public int hatNum = 0;
    [SyncVar]
    public int playerNum=0;
    [SyncVar]
    public int pickUpsCurrentlyHeld = 0;
    [SyncVar]
    public int currentState = 0;
    #endregion
    private CharacterLookScript charLookScript;


    public NetworkConnection connection;
    //stores the network manager override
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
    
    [Client]
    //updates all player looks so they match the custom options picked by the players
    public void UpdateLooks()
    {
        for (int i = 0; i < Room.GamePlayers.Count; i++)
        {
            Debug.Log("UPDATE LOOKS: " + i + Room.GamePlayers[i]);
            UpdateDisplay();
            Room.GamePlayers[i].GetComponent<CharacterLookScript>().playerStart(i);
        }
    }

    [Server]
    private void UpdateDisplay()
    {
        //checks if the client has authority to run this code
        if (!hasAuthority)
        {
            //hides UI for other clients so players can only see their own UI
            //updates the player number incase some players have left since last update

            foreach (var player in Room.GamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        int p = 0;
        foreach (NetworkGamePlayer roomPlayer in Room.GamePlayers)
        {
            Debug.Log(roomPlayer.gameObject.name);
            
            roomPlayer.playerNum = p;
            roomPlayer.charLookScript.changeType(roomPlayer.typeNum);
            roomPlayer.charLookScript.changeHat(roomPlayer.hatNum);
            p++;
        }
    }

    //overrides the OnStartClient which is called when the client starts
    public override void OnStartClient()
    {
        charLookScript = this.GetComponent<CharacterLookScript>();
        //keep this gameobject from being destroyed between scenes
        DontDestroyOnLoad(this.gameObject);
        Room.GamePlayers.Add(this);
        //updates the players appearence
        UpdateLooks();
    }

    //called when the client stops
    public override void OnStopClient()
    {
        //removes from list of player in game
        Room.GamePlayers.Remove(this);
    }

    #region Server Functions
    //only ever run on the server - these are called when the game starts to pass customization from lobby scene to game scene
    [Server] //updates display name
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    [Server] //updates skin num
    public void SetTypeNum(int num)
    {
        this.typeNum = num;      
    }

    [Server] //updates hat num
    public void SetHatNum(int num)
    {
        this.hatNum = num;
    }

    [Server] //updates hat num
    public void SetPlayerNum(int num)
    {
        this.playerNum = num;
    }


    [Command] //updates playerState num
    public void SetPlayerState(int num)
    {
        this.currentState = num;
    }
    #endregion
}
