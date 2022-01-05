using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PanelManager : MonoBehaviour
{
    #region Variables

    //Panel related variables
    private Panel[] _panels;
    private int _currentPanel = -1;

    //Player related variables
    private int _numOfPlayers; //might not need

    #endregion

    private void Start()
    {
        _panels = GetComponentsInChildren<Panel>();

        //begin with first panel in the list
        ActivateNextPanel();
    }

    public void ActivateNextPanel()
    {
        if (_currentPanel < _panels.Length - 1)
        {
            _currentPanel++;

            if (_panels.Length > 0)
            {
                //turn off all panels first
                for (int i = 0; i < _panels.Length; i++)
                {
                    DeactivatePanel(i);
                }

                //activate next panel
                ActivatePanel(_currentPanel);
            }
        }
        else
        {
            Debug.LogWarning("No such panel exists!");
        }
    }

    //the order is correct here
    private void ActivatePanel(int index)
    {
        _panels[index].gameObject.SetActive(true);
        _panels[index].InitializePanel();
    }

    //the order is correct here
    private void DeactivatePanel(int index)
    {
        _panels[index].ExitPanel();
        _panels[index].gameObject.SetActive(false);
    }
}
