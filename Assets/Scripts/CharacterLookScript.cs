using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class CharacterLookScript : NetworkBehaviour
{
    public PlayerCustomizationData data;

    public Transform characterMeshHolder;

    public TMP_Text typeText;
    public TMP_Text hatText;

    private GameObject currentHat;

    public Image card;

    public SpriteRenderer circle;

    private GameObject currentBird;

    public override void OnStartAuthority()
    { 
        if(circle != null)
        {
            circle.enabled = true;
        }
    }

    //called when the player starts


    public void playerStart(int playerNum)
    {
        Debug.Log("POOP");
        //checks for Network Game Player, and then uses these stored numbers to update appearance
        if (this.GetComponent<NetworkGamePlayer>() != null)
        {
            this.GetComponent<NetworkGamePlayer>().playerNum = playerNum;
            this.GetComponent<NetworkGamePlayer>().typeNum = NetworkManagerOverride.typeNumbers[playerNum];
            this.GetComponent<NetworkGamePlayer>().hatNum = NetworkManagerOverride.hatNumbers[playerNum];
            changeType(this.GetComponent<NetworkGamePlayer>().typeNum);
            changeHat(this.GetComponent<NetworkGamePlayer>().hatNum);
            setCircleColour(this.GetComponent<NetworkGamePlayer>().playerNum);
            //callChangeAppearence(NetworkManagerOverride.typeNumbers[playerNum], NetworkManagerOverride.hatNumbers[playerNum], playerNum);
            //callChangeAppearence(0, 0, playerNum);

        }
    }

    [Client]
    //changes the material on the mesh renderer
    public void changeType(int typeNum)
    {
        if (currentBird != null)
        {
            Destroy(currentBird);
        }
        
        currentBird = Instantiate(data.typeMeshes[typeNum], characterMeshHolder);

        if (currentHat != null)
        {
            currentHat.transform.position = currentBird.transform.Find("HatPoint").position;
        }

        if (typeText != null)
        {
            typeText.text = data.typeNames[typeNum];
        }
    }
    [Client]
    //enables and disables hats to change the hat the character is wearing
    public void changeHat(int hatNum)
    {

        if (currentHat != null)
        {
            Destroy(currentHat);
        }

        if (currentBird != null)
        {
            currentHat = Instantiate(data.hatMeshes[hatNum], currentBird.transform);
            currentHat.transform.position = currentBird.transform.Find("HatPoint").position;
        }

        if (hatText != null)
        {
            hatText.text = data.hatNames[hatNum];
        }
    }
    [Client]
    public void changeColour(int colour)
    {
        card.color = data.playerColours[colour];
    }
    [Client]
    public void setCircleColour(int colour)
    {
        circle.color = data.playerColours[colour];
    }

    [ClientRpc]
    public void callChangeAppearence(int type, int hat, int num)
    {
        changeType(type);
        changeHat(hat);
        setCircleColour(num);
    }
}
