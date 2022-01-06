using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsScrollScript : MonoBehaviour
{
    RectTransform rTransform;
    public Vector2 startPos;

    public float bottomPos;
    public float topPos;

    public float scrollSpeed;
    public float fadeSpeed;

    public void Start()
    {
        rTransform = GetComponent<RectTransform>();
        rTransform.anchoredPosition = startPos;

        foreach (RectTransform child in rTransform)
        {
            TMP_Text textMesh = child.GetComponent<TMP_Text>();
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rTransform.anchoredPosition = new Vector2(rTransform.anchoredPosition.x, rTransform.anchoredPosition.y+(scrollSpeed*Time.deltaTime));
        foreach(RectTransform child in rTransform)
        {
            TMP_Text textMesh = child.GetComponent<TMP_Text>();
            if (rTransform.anchoredPosition.y + child.anchoredPosition.y>bottomPos && rTransform.anchoredPosition.y + child.anchoredPosition.y < topPos)
            {
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, Mathf.Min(textMesh.color.a + (fadeSpeed * Time.deltaTime), 1f));
            }
            else
            {
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, Mathf.Max(textMesh.color.a - (fadeSpeed * Time.deltaTime), 0f));
            }
        }
    }
}
