using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Panel : MonoBehaviour
{
    [SerializeField]
    private GameObject _holderPrefab;
    protected List<GameObject> _playerHolders = new List<GameObject>();
    protected const float _scoreHolderOffset = 55f;
    protected const float _debuffHolderOffset = 24f;

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

    //Lines up the players
    protected void InitLineUp(float offset)
    {
        var tempX = (Screen.width - (2 * offset)) / (GetNumOfPlayers());
        var tempY = Screen.height / 5;

        for (int i = 1; i < GetNumOfPlayers() + 1; i++)
        {
            //_playerHolders[i - 1].transform.position = new Vector2(tempX * i - (tempX / 2) + offset, tempY);

            _playerHolders[i - 1].transform.position = new Vector3((tempX * i - (tempX / 2) + offset)-380f, tempY-200f, 360);
        }
    }
}
