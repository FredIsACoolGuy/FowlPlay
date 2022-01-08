using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameRulesButton : MonoBehaviour
{

    public GameRulesData gameRulesData;

    public TMP_Text pickupsText;
    public TMP_Text roundsText;
    public TMP_Text secondsText;

    private int numOfRounds = 15;
    private int secondsOfRounds = 60;
    private int howCommonArePowerups = 2;

    void Start()
    {
        numOfRounds = gameRulesData.numOfRounds;
        secondsOfRounds = gameRulesData.timePerRound;
        howCommonArePowerups = gameRulesData.howCommonArePowerups;
        updatePickupsText();
        updateRoundText();
        updateSecondsText();
    }


    public void clicked()
    {
        gameRulesData.numOfRounds = numOfRounds;
        gameRulesData.timePerRound = secondsOfRounds;
        gameRulesData.howCommonArePowerups = howCommonArePowerups;
    }

    public void incSeconds()
    {
        if (secondsOfRounds == 300)
        {
            secondsOfRounds = 60;
        }
        else
        {
            secondsOfRounds += 30;
        }
        updateSecondsText();
    }
    public void decSeconds()
    {
        if (secondsOfRounds == 60)
        {
            secondsOfRounds = 300;
        }
        else
        {
            secondsOfRounds -= 30;
        }
        updateSecondsText();
    }

    private void updateSecondsText()
    {
        if(secondsOfRounds == 60)
        {
            secondsText.text = "1 minute";
        }
        else
        {
            int halfMins = secondsOfRounds / 30;
            secondsText.text = (halfMins / 2).ToString();
            if (halfMins % 2 == 1)
            {
                secondsText.text = secondsText.text + ".5";
            }
            secondsText.text = secondsText.text + " minutes";
        }
    }


    #region rounds
    public void incNumRounds()
    {
        if(numOfRounds == 1)
        {
            numOfRounds = 5;
        }
        else if(numOfRounds == 50)
        {
            numOfRounds = 1;
        }
        else
        {
            numOfRounds += 5;
        }
        updateRoundText();
    }
    public void decNumRounds()
    {
        if (numOfRounds == 1)
        {
            numOfRounds = 50;
        }
        else if (numOfRounds == 5)
        {
            numOfRounds = 1;
        }
        else
        {
            numOfRounds -= 5;
        }
        updateRoundText();
    }

    private void updateRoundText()
    {
        if (numOfRounds == 1)
        {
            roundsText.text = numOfRounds + " Round";
        }
        else
        {
            roundsText.text = numOfRounds + " Rounds";
        }
    }
    #endregion

    #region pickups
    public void incPickups()
    {
        howCommonArePowerups = (howCommonArePowerups+1) % 5;
        updatePickupsText();
    }
    public void decPickups()
    {
        howCommonArePowerups = (howCommonArePowerups + 4) % 5;
        updatePickupsText();
    }

    private void updatePickupsText()
    {
        switch (howCommonArePowerups)
        {
            case 0:
                pickupsText.text = "None";
                break;
            case 1:
                pickupsText.text = "A few";
                break;
            case 2:
                pickupsText.text = "A good amount";
                break;
            case 3:
                pickupsText.text = "A lot";
                break;
            case 4:
                pickupsText.text = "Too many";
                break;
        }
    }

#endregion
}
