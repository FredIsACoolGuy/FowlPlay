using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerDataHolder : NetworkBehaviour
{

    public List<int> typeNumbers = new List<int>();

    public List<int> hatNumbers = new List<int>();

    public List<string> playerNames = new List<string>();


    public string testString = null;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    //private NetworkManagerOverride room;
    //private NetworkManagerOverride Room
    //{
    //    get
    //    {
    //        //if room is currently null it finds it and assigns to room
    //        if (room != null)
    //        {
    //            return room;
    //        }
    //        return room = NetworkManager.singleton as NetworkManagerOverride;
    //    }
    //}



    //[ClientRpc]
    //public void SavePlayerData()
    //{
    //    UpdateDataHolder();
    //}

    //[Client]
    //public void UpdateDataHolder()
    //{
    //    Debug.Log("JISS");

    //    typeNumbers.Clear();
    //    hatNumbers.Clear();
    //    playerNames.Clear();

    //    for (int i = 0; i < Room.RoomPlayers.Count; i++)
    //    {
    //        Debug.Log(Room.RoomPlayers[0].typeNum);
    //        typeNumbers.Add(Room.RoomPlayers[i].typeNum);
    //        hatNumbers.Add(Room.RoomPlayers[i].hatNum);
    //        playerNames.Add(Room.RoomPlayers[i].DisplayName);
    //    }

    //    Debug.Log("JISS");
    //}
}
