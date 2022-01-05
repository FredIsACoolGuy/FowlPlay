using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    #region UI Variables

    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    //[SerializeField] private GameObject addPlayerCard = null;

    #endregion

    #region Custom Variables

    [Header("Customization")]
    [SyncVar]
    public int playerNum;
    private CharacterLookScript charLookScript;
    private int maxTypeNum = 3;
    private int maxHatNum = 3;
    public GameObject interactableCard;
    public GameObject nonInteractableCard;

    public PlayerNameInput nameInput;
    public TMP_Text nameText;

    public GameObject readyImage;
    public TMP_Text readyText;
    //can only be updated on the server
    //when variables change these functions are called
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Player";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleTypeStatusChanged))]
    public int typeNum = 0;
    [SyncVar(hook = nameof(HandleHatStatusChanged))]
    public int hatNum = 0;

    #endregion

    
    //bool to store if this client is also the host
    public bool isLeader = false;
    public bool IsLeader
    {
        set
        {
            //sets up UI to only be enabled for the leader
            isLeader = value;
        //    startGameButton.gameObject.SetActive(value);
        //    addPlayerCard.gameObject.SetActive(value);
        }
    }

    //the network manager override script 
    private NetworkManagerOverride room;
    private NetworkManagerOverride Room
    {
        get
        {
            //if not assigned then set room to the Network manager override
            if (room != null)
            {
                return room;
            }
            return room = NetworkManager.singleton as NetworkManagerOverride;
        }
    }

    //called on start only on objects this client has authority over
    public override void OnStartAuthority()
    {
        charLookScript = this.GetComponent<CharacterLookScript>();
        maxTypeNum = charLookScript.data.typeMeshes.Length;
        maxHatNum = charLookScript.data.hatMeshes.Length;

        //update the display name to be that of the input box
        nameInput.callStart();
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        //turn on the lobby UI
        if (GameObject.Find("LeaderUI") != null)
        {
            lobbyUI = GameObject.Find("LeaderUI");
            Debug.Log("Speed");
        }
        if (GameObject.Find("LoadingPanel") != null)
        {
            GameObject.Find("LoadingPanel").SetActive(false);
        }
        if (GameObject.Find("StartGameButton") != null)
        {
           startGameButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        }

        
    }


    //Called at start on client
    public override void OnStartClient()
    {
        //add to the list of room players - the list of all clients in the lobby
        charLookScript = this.GetComponent<CharacterLookScript>();
        maxTypeNum = charLookScript.data.typeMeshes.Length;
        maxHatNum = charLookScript.data.hatMeshes.Length;
        
        Room.RoomPlayers.Add(this);

        //DontDestroyOnLoad(this.gameObject);
        //call update display to show new client
        UpdateDisplay();
    }

    //Called when client stops
    public override void OnStopClient()
    {
        //removes from the list of room players - the list of all clients in the lobby
        Room.RoomPlayers.Remove(this);
        //call update display to stop showing character
        UpdateDisplay();
    }


    // whenever one of these values change the UpdateDisplay() function is called
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleTypeStatusChanged(int oldValue, int newValue) => UpdateDisplay();
    public void HandleHatStatusChanged(int oldValue, int newValue) => UpdateDisplay();

    //calls UpdateDisplayName so the name is updated when the player readies - this in turn causes the update display to be called
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplayName();

    //checks the room is active and then calls the command to change display name
    private void UpdateDisplayName()
    {       
        if (Room.isNetworkActive)
        {
            CmdSetDisplayName(PlayerNameInput.DisplayName);
        }
    }


    //update display refreshes all the visuals for each character
    private void UpdateDisplay()
    {
        //checks if the client has authority to run this code
        if (!hasAuthority)
        {
            interactableCard.SetActive(false);
            nameInput.gameObject.SetActive(false);
            nonInteractableCard.SetActive(true);

            //hides UI for other clients so players can only see their own UI
            //updates the player number incase some players have left since last update
            
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;      
        }

        interactableCard.SetActive(true);
        nameInput.gameObject.SetActive(true);
        nonInteractableCard.SetActive(false);

        if (lobbyUI != null)
        {
            if (isLeader)
            {
                lobbyUI.SetActive(true);
            }
            else
            {
                lobbyUI.SetActive(false);
            }
        }

        //sets all characters to blank - this resets all the characters so they are ready to update
        charLookScript.changeHat(hatNum);
        charLookScript.changeType(typeNum);

        bool canStartGame = true;
        int p = 0;
        foreach (NetworkRoomPlayerLobby roomPlayer in Room.RoomPlayers) 
        {
            roomPlayer.playerNum = p;
            roomPlayer.positionCards(roomPlayer.playerNum, Room.RoomPlayers.Count);
            roomPlayer.charLookScript.changeColour(roomPlayer.playerNum);
            roomPlayer.charLookScript.changeType(roomPlayer.typeNum);
            roomPlayer.charLookScript.changeHat(roomPlayer.hatNum);
            roomPlayer.nameText.text = roomPlayer.DisplayName;
            roomPlayer.readyImage.SetActive(roomPlayer.IsReady);
            if (!roomPlayer.IsReady)
            {
                roomPlayer.readyText.text = "Not Ready";
                canStartGame = false;
            }
            else
            {
                roomPlayer.readyText.text = "Ready!";
            }
            p++;
        }
     

        if (startGameButton != null)
        {
            startGameButton.interactable = canStartGame;
        }

        
        //loop through all character models and if there is a client for each model it is set visible, otherwise it is hidden

    }

    public void positionCards(int num, int total)
    {
        switch (total)
        {
            case 1:
                transform.localScale = new Vector3(1f, 1f, 1f);
                if (num == 0)
                {
                    transform.position = new Vector3(0f, 0f, 0f);
                }
                break;
            case 2:
                transform.localScale = new Vector3(1f, 1f, 1f);
                if (num == 0)
                {
                    transform.position = new Vector3(-55f, 0f, 0f);
                }
                else
                {
                    transform.position = new Vector3(55f, 0f, 0f);
                }
                break;
            case 3:
                transform.localScale = new Vector3(1f, 1f, 1f);
                if (num == 0)
                {
                    transform.position = new Vector3(-94f, 0f, 0f);
                }
                else if (num == 1)
                {
                    transform.position = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    transform.position = new Vector3(94f, 0f, 0f);
                }
                break;
            case 4:
                transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                switch (num)
                {
                    case 0:
                        transform.position = new Vector3(-105f, 0f, 0f);
                        break;
                    case 1:
                        transform.position = new Vector3(-35f, 0f, 0f);
                        break;
                    case 2:
                        transform.position = new Vector3(35f, 0f, 0f);
                        break;
                    case 3:
                        transform.position = new Vector3(105f, 0f, 0f);
                        break;
                }
                break;
            case 5:
                transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);
                switch (num)
                {
                    case 0:
                        transform.position = new Vector3(-66f, 42f, 0f);
                        break;
                    case 1:
                        transform.position = new Vector3(-0f, 42f, 0f);
                        break;
                    case 2:
                        transform.position = new Vector3(66f, 42f, 0f);
                        break;
                    case 3:
                        transform.position = new Vector3(-33f, -42f, 0f);
                        break;
                    case 4:
                        transform.position = new Vector3(33f, -42f, 0f);
                        break;
                }
                break;
            case 6:
                transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);
                switch (num)
                {
                    case 0:
                        transform.position = new Vector3(-66f, 42f, 0f);
                        break;
                    case 1:
                        transform.position = new Vector3(-0f, 42f, 0f);
                        break;
                    case 2:
                        transform.position = new Vector3(66f, 42f, 0f);
                        break;
                    case 3:
                        transform.position = new Vector3(-66f, -42f, 0f);
                        break;
                    case 4:
                        transform.position = new Vector3(0f, -42f, 0f);
                        break;
                    case 5:
                        transform.position = new Vector3(66f, -42f, 0f);
                        break;
                }
                break;
            case 7:
                transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);
                switch (num)
                {
                    case 0:
                        transform.position = new Vector3(-96f, 42f, 0f);
                        break;
                    case 1:
                        transform.position = new Vector3(-32f, 42f, 0f);
                        break;
                    case 2:
                        transform.position = new Vector3(32f, 42f, 0f);
                        break;
                    case 3:
                        transform.position = new Vector3(96f, 42f, 0f);
                        break;
                    case 4:
                        transform.position = new Vector3(-66f, -42f, 0f);
                        break;
                    case 5:
                        transform.position = new Vector3(0f, -42f, 0f);
                        break;
                    case 6:
                        transform.position = new Vector3(66f, -42f, 0f);
                        break;
                }
                break;
            case 8:
                transform.localScale = new Vector3(0.66f, 0.66f, 0.66f);
                switch (num)
                {
                    case 0:
                        transform.position = new Vector3(-96f, 42f, 0f);
                        break;
                    case 1:
                        transform.position = new Vector3(-32f, 42f, 0f);
                        break;
                    case 2:
                        transform.position = new Vector3(32f, 42f, 0f);
                        break;
                    case 3:
                        transform.position = new Vector3(96f, -42f, 0f);
                        break;
                    case 4:
                        transform.position = new Vector3(-96f, -42f, 0f);
                        break;
                    case 5:
                        transform.position = new Vector3(-32f, -42f, 0f);
                        break;
                    case 6:
                        transform.position = new Vector3(32f, -42f, 0f);
                        break;
                    case 7:
                        transform.position = new Vector3(96f, -42f, 0f);
                        break;
                }
                break;
        }
       
    }

    //sets start button active for host when all players are ready
    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

    //    startGameButton.interactable = readyToStart;
    }
 

    #region Commands
    //commands can be called by clients but are only ran on the server

    [Command] //update player display name
    private void CmdSetDisplayName(string displayName)
    {
            DisplayName = displayName;
    }

    [Command] //update player num
    private void CmdSetPlayerNum(int pNum)
    {
        playerNum = pNum;
    }

    [Command] //update player skin num
    public void CmdIncTypeNum()
    {
        typeNum = (typeNum+1) % maxTypeNum;
    }

    [Command] //update player skin num
    public void CmdDecTypeNum()
    {
        typeNum = (typeNum - 1 + maxTypeNum) % maxTypeNum;
    }

    [Command] //update player hat num
    public void CmdIncHatNum()
    {
        hatNum = (hatNum + 1) % maxHatNum;
    }

    [Command] //update player hat num
    public void CmdDecHatNum()
    {
        hatNum = (hatNum - 1 + maxHatNum) % maxHatNum;
    }

    [Command] //update player ready
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        UpdateDisplay();
        if (IsReady)
        {
            foreach (NetworkRoomPlayerLobby roomPlayer in Room.RoomPlayers)
            {
                roomPlayer.callUpdate();
            }
        }
            Room.NotifyPlayersOfReadyState();
        UpdateDisplay();

    }

    [ClientRpc]
    public void callUpdate()
    {
        UpdateDataHolder();     
    }

    [Command] //Starts the actual game
    public void CmdStartGame()
    {
        //check that the player is the host
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }

        //start game
        Room.StartGame();
    }
    #endregion




    [Client]
    public void UpdateDataHolder()
    {
        if (hasAuthority)
        {
            PlayerDataHolder pdh = GameObject.Find("PlayerDataHolder").GetComponent<PlayerDataHolder>();

            pdh.typeNumbers.Clear();
            pdh.hatNumbers.Clear();
            pdh.playerNames.Clear();

            for (int i = 0; i < Room.RoomPlayers.Count; i++)
            {
                Debug.Log(Room.RoomPlayers[0].typeNum);
                pdh.typeNumbers.Add(Room.RoomPlayers[i].typeNum);
                pdh.hatNumbers.Add(Room.RoomPlayers[i].hatNum);
                pdh.playerNames.Add(Room.RoomPlayers[i].DisplayName);
            }
        }
    }
}
