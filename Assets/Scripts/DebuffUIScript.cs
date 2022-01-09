using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebuffUIScript : MonoBehaviour
{
    public Image circle;
    public Image debuffPic;
    public GameObject numberHolder;
    public TMP_Text number;
    private int num = 1;
    public int debuffID;

    public PlayerCustomizationData playerData;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(int colour)
    {
        circle.color = playerData.playerColours[colour];
    }

    public void IncrementNumber()
    {
        numberHolder.SetActive(true);
        num++;
        number.text = num.ToString();
    }
}
