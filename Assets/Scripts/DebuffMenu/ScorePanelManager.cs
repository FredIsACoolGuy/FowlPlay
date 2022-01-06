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
        var temp = Screen.width / GetNumOfPlayers() + 1;

        for (int i = 1; i < GetNumOfPlayers() + 1; i++)
        {
            _playerHolders[i-1].transform.position = new Vector2 (temp * i, 100);
        }

        /*var temp = Camera.main.WorldToScreenPoint(Vector2.one*10);
        _playerHolders[0].transform.position = temp;*/
    }
}
