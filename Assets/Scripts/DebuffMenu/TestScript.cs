using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public bool _test = false;
    public GameObject _subject;

    private void Update()
    {
        if (_test)
        {
            _subject.GetComponent<PanelManager>().ActivateNextPanel();
            _test = false;
        }
    }
}
