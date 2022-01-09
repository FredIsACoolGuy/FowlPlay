using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanelManager : Panel
{
    public float _speed;
    public float _timer;

    public override void InitializePanel()
    {
        InstantiatePlayerHolders();
    }

    public override void ExitPanel()
    {

    }

    //Instantiates all players
    protected override void InstantiatePlayerHolders()
    {
        base.InstantiatePlayerHolders();
        InitLineUp(_scoreHolderOffset);
        GrabLook();
        DisplayScore();
    }

    //for any amount of players, grab their corresponding look
    private void GrabLook()
    {
        for (int i = 0; i < _playerHolders.Count; i++)
        {
            _playerHolders[i].GetComponent<CharacterLookScript>().changeType(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].typeNum);
            _playerHolders[i].GetComponent<CharacterLookScript>().changeHat(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].hatNum);
            _playerHolders[i].GetComponent<CharacterLookScript>().nameText.text = FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].DisplayName;
            _playerHolders[i].GetComponent<CharacterLookScript>().setCircleColour(FindObjectOfType<NetworkManagerOverride>().GamePlayers[i].playerNum);
        }
    }

    private void DisplayScore()
    {
        ErrorHandling.CheckActive(gameObject);

        float currentMax = 0;

        //find max num of debuffs first
        for (int i = 0; i < _playerHolders.Count; i++)
        {
            float temp  = FindNumOfDebuffs(i);
            if (temp > currentMax)
            {
                currentMax = temp;
            }
        }

        for (int i = 0; i < _playerHolders.Count; i++)
        {
            _playerHolders[i].GetComponentInChildren<ScoreBar>().SetScore(FindNumOfDebuffs(i), currentMax, _speed);
        }
    }

    //find number of debuffs player holds
    private float FindNumOfDebuffs(int index)
    {
        return FindObjectOfType<NetworkManagerOverride>().GamePlayers[index].pickUpsCurrentlyHeld;
    }
}
