using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanelManager : Panel
{
    public override void InitializePanel()
    {
        InstantiatePlayerHolders();
        ErrorHandling.CheckActive(gameObject);
    }

    public override void ExitPanel()
    {

    }

    //Instantiates all players
    protected override void InstantiatePlayerHolders()
    {
        base.InstantiatePlayerHolders();
        LineUp();
    }

    //Lines up the players
    private void LineUp()
    {
        var tempX = Screen.width / GetNumOfPlayers() + 1;
        var tempY = Screen.height / 3;

        for (int i = 1; i < GetNumOfPlayers() + 1; i++)
        {
            _playerHolders[i-1].transform.position = new Vector2 (tempX * i - (tempX/2), tempY);
        }
    }

    private void GrabLook()
    {
        /*_playerHolders[0].GetComponent<CharacterLookScript>().changeType(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].typeNum);
        _playerHolders[0].GetComponent<CharacterLookScript>().changeHat(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].hatNum);
        _playerHolders[0].GetComponent<CharacterLookScript>().nameText.text = FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].DisplayName;
        _playerHolders[0].GetComponent<CharacterLookScript>().setCircleColour(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].playerNum);*/
    }
}
