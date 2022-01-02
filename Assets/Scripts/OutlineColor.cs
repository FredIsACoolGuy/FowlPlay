using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineColor : MonoBehaviour
{
    [HideInInspector] public Color outlineColor;
    [SerializeField] Renderer renderer;
    [SerializeField] bool isBird = false;
    public float lineWeightMultiplier = 1;

    private void Awake() {
        if (isBird) {
            outlineColor = renderer.material.GetColor("_OutlineColor");
            if (lineWeightMultiplier != 1) {
                Material[] mats = renderer.materials;
                float lineWeight = mats[0].GetFloat("_OutlineThickness");
                foreach (Material mat in mats) {
                    mat.SetFloat("_OutlineThickness", lineWeight * lineWeightMultiplier);
                }
            }
        }
        else {
            StartCoroutine(SetColor());
        }
    }

    IEnumerator SetColor() {
        yield return new WaitForEndOfFrame();

        OutlineColor parentScript = transform.parent.GetComponent<OutlineColor>();
        Color col = parentScript.outlineColor;
        float parentMultiplier = parentScript.lineWeightMultiplier;

        if (renderer == null)
            renderer = GetComponent<Renderer>();
        Material[] mats = renderer.materials;

        float lineWeight = mats[0].GetFloat("_OutlineThickness");
        foreach (Material mat in mats) {
            mat.SetColor("_OutlineColor", col);
            mat.SetFloat("_OutlineThickness", lineWeight * parentMultiplier);
        }
    }
}
