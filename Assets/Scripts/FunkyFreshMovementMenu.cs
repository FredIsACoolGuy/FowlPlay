using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer.GameControls;
using Mirror;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class FunkyFreshMovementMenu : MonoBehaviour
{
    public GameObject holder;
    private GameControls controls;

    public PlayerMovementController movementController;
    public PlayerAimScript aimScript;

    public Slider speedSlider;
    public Slider accSlider;
    public Slider decSlider;
    public Slider lineSpeedSlider;
    public Slider maxLineTimeSlider;
    public Slider attackSpeedSlider;
    public Slider attackTimeSlider;
    public Slider knockPowerSlider;
    public Slider knockTimeSlider;

    private bool canEffectStuff;
    private GameControls Controls
    {
        get
        {
            if (controls != null)
            {
                return controls;
            }
            return controls = new GameControls();
        }
    }


    public void Start()
    {
        Controls.Enable();
        holder.SetActive(false);
        Controls.Player.DebugButton.performed += ctx => ShowCanvas(ctx.ReadValue<float>());
        GetValues();
    }


    // Update is called once per frame
    private void GetValues()
    {
        speedSlider.value = movementController.normalMovementSpeed;
        accSlider.value = movementController.accelerationSpeed;
        decSlider.value = movementController.decelerationSpeed;
        lineSpeedSlider.value = aimScript.camMoveSpeed;
        maxLineTimeSlider.value = aimScript.maxTimeHeld;
        attackSpeedSlider.value = movementController.attackSpeed;
        attackTimeSlider.value = movementController.attackTimeMultiplier;
        knockPowerSlider.value = movementController.knockPowerMultiplier;
        knockTimeSlider.value = movementController.knockTimeMultiplier;
    }

    public void SetValues()
    {
        movementController.normalMovementSpeed = speedSlider.value;
        movementController.movementSpeed = speedSlider.value;
        movementController.accelerationSpeed = accSlider.value;
        movementController.decelerationSpeed = decSlider.value;
        aimScript.camMoveSpeed = lineSpeedSlider.value;
        aimScript.maxTimeHeld = maxLineTimeSlider.value;
        movementController.attackSpeed = attackSpeedSlider.value;
        movementController.attackTimeMultiplier = attackTimeSlider.value;
        movementController.knockPowerMultiplier = knockPowerSlider.value;
        movementController.knockTimeMultiplier = knockTimeSlider.value;
    }

    public void SetNums()
    {
        speedSlider.transform.Find("Number").GetComponent<Text>().text = speedSlider.value.ToString();
        accSlider.transform.Find("Number").GetComponent<Text>().text = accSlider.value.ToString();
        decSlider.transform.Find("Number").GetComponent<Text>().text = decSlider.value.ToString();
        lineSpeedSlider.transform.Find("Number").GetComponent<Text>().text = lineSpeedSlider.value.ToString();
        maxLineTimeSlider.transform.Find("Number").GetComponent<Text>().text = maxLineTimeSlider.value.ToString();
        attackSpeedSlider.transform.Find("Number").GetComponent<Text>().text = attackSpeedSlider.value.ToString();
        attackTimeSlider.transform.Find("Number").GetComponent<Text>().text = attackTimeSlider.value.ToString();
        knockPowerSlider.transform.Find("Number").GetComponent<Text>().text = knockPowerSlider.value.ToString();
        knockTimeSlider.transform.Find("Number").GetComponent<Text>().text = knockTimeSlider.value.ToString();
    }

    private void ShowCanvas(float num)
    {
        SceneManager.LoadScene(4);
        if (holder.activeSelf)
        {
            SetValues();
        }
        else
        {
            GetValues();
        }

        holder.SetActive(!holder.activeSelf);
    }

    [ClientCallback]
    private void OnEnable()
    {
        Controls.Enable();
    }
    [ClientCallback]
    private void OnDisable()
    {
        Controls.Disable();
    }
}
