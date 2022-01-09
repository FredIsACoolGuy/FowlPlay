using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public bool _test = false;
    public NetworkManagerOverride _network;

    public int _testNum = 3;

    private void Awake()
    {
        for (int i = 0; i < _testNum; i++)
        {
            _network.GamePlayers.Add(new NetworkGamePlayer());
        }

        _network.GamePlayers[0].pickUpsCurrentlyHeld = 3;
        _network.GamePlayers[1].pickUpsCurrentlyHeld = 4;
        _network.GamePlayers[2].pickUpsCurrentlyHeld = 5;
        _network.GamePlayers[3].pickUpsCurrentlyHeld = 4;
        _network.GamePlayers[4].pickUpsCurrentlyHeld = 3;


    }
    private void Update()
    {
        if (_test)
        {
            FindObjectOfType<PanelManager>().ActivateNextPanel();
            _test = false;
        }
    }
}
