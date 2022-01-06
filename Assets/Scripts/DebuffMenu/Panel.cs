using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField]
    private GameObject _holderPrefab;
    protected List<GameObject> _playerHolders = new List<GameObject>();

    public abstract void InitializePanel();
    public abstract void ExitPanel();

    protected virtual void InstantiatePlayerHolders()
    {
        var numOfPlayers = GetNumOfPlayers();

        for (int i = 0; i < numOfPlayers; i++)
        {
            var temp = Instantiate(_holderPrefab, gameObject.transform);
            _playerHolders.Add(temp);
        }
    }
    protected int GetNumOfPlayers()
    {
        return FindObjectOfType<NetworkManagerOverride>().GamePlayers.Count;
    }
}
