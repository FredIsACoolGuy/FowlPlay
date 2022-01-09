using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineColor : MonoBehaviour
{
    [HideInInspector] public Color outlineColor;
    [SerializeField] Renderer renderer;
    [SerializeField] bool isBird = false;
    private float lineWeightMultiplier = 1;

    private void Awake() {
        if (isBird) {
            outlineColor = renderer.material.GetColor("_OutlineColor");
        }
        else {
            StartCoroutine(SetColor());
        }
    }

    public void MultiplyLineWeight(float lineWeightMultiplier) {
        this.lineWeightMultiplier = lineWeightMultiplier;

        Material[] mats = renderer.materials;
        float lineWeight = mats[0].GetFloat("_OutlineThickness");

        foreach (Material mat in mats) {
            mat.SetFloat("_OutlineThickness", lineWeight * lineWeightMultiplier);
        }
    }

    private IEnumerator SetColor() {
        yield return new WaitForEndOfFrame();
        if (transform.parent != null)
        {
            OutlineColor parentScript = transform.parent.GetComponent<OutlineColor>();
            Color col = parentScript.outlineColor;
            float parentMultiplier = parentScript.lineWeightMultiplier;

            if (renderer == null)
                renderer = GetComponent<Renderer>();
            Material[] mats = renderer.materials;

            float lineWeight = mats[0].GetFloat("_OutlineThickness");
            foreach (Material mat in mats)
            {
                mat.SetColor("_OutlineColor", col);
                mat.SetFloat("_OutlineThickness", lineWeight * parentMultiplier);
            }
        }
    }

    public IEnumerator SetColorNoScale()
    {
        yield return new WaitForEndOfFrame();
        if (transform.parent.GetComponent<OutlineColor>() != null)
        {
            OutlineColor parentScript = transform.parent.GetComponent<OutlineColor>();
            Color col = parentScript.outlineColor;

            if (renderer == null)
                renderer = GetComponent<Renderer>();
            Material[] mats = renderer.materials;

            foreach (Material mat in mats)
            {
                mat.SetColor("_OutlineColor", col);
            }
        }
       
    }
}
