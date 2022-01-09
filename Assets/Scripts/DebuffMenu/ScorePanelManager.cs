using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanelManager : Panel
{
    private const float _offset = 55f;

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
        LineUp(_offset);
        GrabLook();
    }

    //Lines up the players
    private void LineUp(float offset)
    {
        var tempX = (Screen.width - (2 * offset)) / (GetNumOfPlayers());
        var tempY = Screen.height / 5;

        for (int i = 1; i < GetNumOfPlayers() + 1; i++)
        {
            _playerHolders[i-1].transform.position = new Vector2 (tempX * i - (tempX/2) + offset, tempY);
        }
    }

    //for any amount of players, grab their corresponding look
    private void GrabLook()
    {
        for (int i = 0; i < _playerHolders.Count; i++)
        {
            _playerHolders[i].GetComponent<CharacterLookScript>().changeType(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].typeNum);
            _playerHolders[i].GetComponent<CharacterLookScript>().changeHat(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].hatNum);
            _playerHolders[i].GetComponent<CharacterLookScript>().nameText.text = FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].DisplayName;    //bug? can't find
            _playerHolders[i].GetComponent<CharacterLookScript>().setCircleColour(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].playerNum);
        }

        /*_playerHolders[0].GetComponent<CharacterLookScript>().changeType(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].typeNum);
        _playerHolders[0].GetComponent<CharacterLookScript>().changeHat(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].hatNum);
        _playerHolders[0].GetComponent<CharacterLookScript>().nameText.text = FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].DisplayName;
        _playerHolders[0].GetComponent<CharacterLookScript>().setCircleColour(FindObjectOfType<NetworkManagerOverride>().GamePlayers[0].playerNum);*/
    }

    private void DisplayScore()
    {
        for (int i = 0; i < _playerHolders.Count; i++)
        {

        }
    }
}
