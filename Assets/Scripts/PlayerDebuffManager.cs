using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Multiplayer.GameControls;

public class PlayerDebuffManager : NetworkBehaviour
{
    public DebuffsData debuffs;
    public List<int> debuffsHeld = new List<int>();

    public GameObject debuffUI;
    public GameObject debuffUIHolder;

    public List<DebuffUIScript> playersDebugUIs = new List<DebuffUIScript>();

    private PlayerMovementController playerMovementController;
    private PlayerAimScript playerAimScript;

    public float uiPadding;

    #region defaultValues

    private float defaultAcc;
    private float defaultDec;

    private float defaultSlowdown;


    #endregion

    public override void OnStartAuthority()
    {
        enabled = true;
        debuffsHeld.Clear();
        for(int i = 0; i<debuffs.debuffs.Length; i++)
        {
            debuffsHeld.Add(0);
        }

        playerMovementController = GetComponent<PlayerMovementController>();
        playerAimScript = GetComponent<PlayerAimScript>();

        defaultAcc = playerMovementController.accelerationSpeed;
        defaultDec = playerMovementController.decelerationSpeed;
        defaultSlowdown = playerAimScript.slowDown;
    }

    public void addDebuff(int index, int colorNum)
    {
        debuffsHeld[index]++;
        if (debuffsHeld[index] == 1)
        {
            GameObject newUI = Instantiate(debuffUI, debuffUIHolder.transform);
            DebuffUIScript newScript = newUI.GetComponent<DebuffUIScript>();
            newScript.debuffID = index;
            newScript.SetColor(colorNum);
            playersDebugUIs.Add(newScript);
            LayoutUI();
        }
        else
        {
            foreach(DebuffUIScript uiScript in playersDebugUIs)
            {
                if (uiScript.debuffID != index)
                {
                    break;
                }
                else
                {
                    uiScript.IncrementNumber();
                }

            }
        }

        ApplyThisDebuff(index, debuffsHeld[index]);
    }

    public void ApplyDebuffs()
    {
        for(int i =0; i<debuffsHeld.Count; i++)
        {
            if (debuffsHeld[i] >= 0)
            {
                ApplyThisDebuff(i, debuffsHeld[i]);
            }
        }
    }

    private void LayoutUI()
    {
        for(int i = 0; i<playersDebugUIs.Count; i++)
        {
            RectTransform rt = playersDebugUIs[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3((uiPadding/1.5f)+(i*uiPadding), 0f, 0f);
        }
    }

    private void ApplyThisDebuff(int index, int total)
    {
        switch (index)
        {
            case 0:         //ICE SHOES
                playerMovementController.accelerationSpeed = defaultAcc / (total+1);
                playerMovementController.decelerationSpeed = defaultDec / (total+3);
                break;
            case 1:         //SLOW AIM
                playerAimScript.slowDown = 1f + (total/5f);
                break;
            case 2:         //SLOW AIM
                playerMovementController.bounceBack = true;
                break;
            case 3:         //SLOW AIM
                playerMovementController.inverted = true;
                break;
        }
    }
}
