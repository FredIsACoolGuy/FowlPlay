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
        var temp = Camera.main.WorldToScreenPoint(Vector2.zero);
        _playerHolders[0].transform.position = temp;
    }
}
