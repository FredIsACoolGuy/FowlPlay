using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public bool _test = false;
    public GameObject _subject;
    public NetworkManagerOverride _network;

    public int _testNum = 3;

    private void Awake()
    {
        for (int i = 0; i < _testNum; i++)
        {
            _network.GamePlayers.Add(new NetworkGamePlayer());
            _network.GamePlayers[i].DisplayName = "test";
            _network.GamePlayers[i].pickUpsCurrentlyHeld = 3;
        }
    }
    private void Update()
    {
        if (_test)
        {
            _subject.GetComponent<PanelManager>().ActivateNextPanel();
            _test = false;
        }
    }
}
