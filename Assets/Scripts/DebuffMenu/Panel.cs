using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerHolder;

    public abstract void InitializePanel();
    public abstract void ExitPanel();

    protected virtual void InstantiatePlayerHolders()
    {
        var numOfPlayers = GetNumOfPlayers();

        for (int i = 0; i < numOfPlayers; i++)
        {
            Instantiate(_playerHolder, gameObject.transform);
        }
    }
    protected int GetNumOfPlayers()
    {
        return FindObjectOfType<NetworkManagerOverride>().GamePlayers.Count;
    }
}
