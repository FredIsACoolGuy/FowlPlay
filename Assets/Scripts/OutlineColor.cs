using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineColor : MonoBehaviour
{
    [HideInInspector] public Color outlineColor;
    [SerializeField] Renderer renderer;
    [SerializeField] bool isBird = false;

    private void Awake() {
        if (isBird) {
            outlineColor = renderer.material.GetColor("_OutlineColor");
        }
        else {
            StartCoroutine(SetColor());
        }
    }

    IEnumerator SetColor() {
        yield return new WaitForEndOfFrame();

        Color col = transform.parent.GetComponent<OutlineColor>().outlineColor;

        if (renderer == null)
            renderer = GetComponent<Renderer>();
        Material[] mats = renderer.materials;
        foreach (Material mat in mats) {
            mat.SetColor("_OutlineColor", col);
        }
    }
}
