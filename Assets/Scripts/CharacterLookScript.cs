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
    public SpriteRenderer arrow;

    public TMP_Text nameText;

    private GameObject currentBird;

    private int currentTypeNum = 100;
    private int currentHatNum = 100;

    public override void OnStartAuthority()
    { 
        if(circle != null)
        {
            circle.enabled = true;
        }
        changeHat(0);
        
    }

    //called when the player starts


    public void playerStart(int playerNum)
    {
        Debug.Log("Only in game");
        //checks for Network Game Player, and then uses these stored numbers to update appearance
        if (this.GetComponent<NetworkGamePlayer>() != null)
        {
            
           PlayerDataHolder pdh = GameObject.Find("PlayerDataHolder").GetComponent<PlayerDataHolder>();

            Debug.Log(pdh.typeNumbers.Count);
            //get values from lists
            this.GetComponent<NetworkGamePlayer>().playerNum = playerNum;
            this.GetComponent<NetworkGamePlayer>().typeNum = pdh.typeNumbers[playerNum];
            this.GetComponent<NetworkGamePlayer>().hatNum = pdh.hatNumbers[playerNum];
            this.GetComponent<NetworkGamePlayer>().DisplayName = pdh.playerNames[playerNum];

            //Actual appearance changing
            changeType(this.GetComponent<NetworkGamePlayer>().typeNum);
            changeHat(this.GetComponent<NetworkGamePlayer>().hatNum);
            setCircleColour(this.GetComponent<NetworkGamePlayer>().playerNum);
            nameText.text = this.GetComponent<NetworkGamePlayer>().DisplayName;

            //callChangeAppearence(NetworkManagerOverride.typeNumbers[playerNum], NetworkManagerOverride.hatNumbers[playerNum], playerNum);
            //callChangeAppearence(0, 0, playerNum);
        }

    }

    [Client]
    //changes the material on the mesh renderer
    public void changeType(int typeNum) 
    {
        if (currentTypeNum == typeNum)
        {
            return;
        }

        if (currentBird != null) {
            Destroy(currentBird);
        }

        currentBird = Instantiate(data.typeMeshes[typeNum], characterMeshHolder);

        OutlineColor hatAnchor = currentBird.transform.GetComponentInChildren<OutlineColor>();

        if (GetComponent<NetworkGamePlayer>() == null) { 
            hatAnchor.MultiplyLineWeight(16);
        }

        if (currentHat != null)
        {
            currentHat.transform.SetParent(hatAnchor.transform);
            currentHat.transform.localPosition = Vector3.zero;
            StartCoroutine(currentHat.GetComponent<OutlineColor>().SetColorNoScale());
            currentHat.transform.localRotation = data.hatMeshes[currentHatNum].transform.rotation;
        }

        if (typeText != null)
        {
            typeText.text = data.typeNames[typeNum];
        }

        currentTypeNum = typeNum;
    }
    [Client]
    //enables and disables hats to change the hat the character is wearing
    public void changeHat(int hatNum)
    {
        if (currentHatNum == hatNum)
        {
            return;
        }
        Debug.Log("IS RUNNIG" + hatNum);

        if (currentHat != null)
        {
            Destroy(currentHat);
        }

        if (currentBird != null)
        {
            Debug.Log("BIRD IS OK");
            Transform hatPos = currentBird.transform.GetComponentInChildren<OutlineColor>().transform;
            currentHat = Instantiate(data.hatMeshes[hatNum], hatPos.position, data.hatMeshes[hatNum].transform.rotation);

            currentHat.transform.localScale *= currentBird.transform.lossyScale.x;
            currentHat.transform.SetParent(hatPos);

            currentHat.transform.localPosition = Vector3.zero;
            currentHat.transform.localRotation = data.hatMeshes[hatNum].transform.localRotation;
            currentHatNum = hatNum;
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
        if (arrow != null)
        {
            arrow.color = data.playerColours[colour];
        }
    }

    [ClientRpc]
    public void callChangeAppearence(int type, int hat, int num)
    {
        changeType(type);
        changeHat(hat);
        setCircleColour(num);
    }
}
