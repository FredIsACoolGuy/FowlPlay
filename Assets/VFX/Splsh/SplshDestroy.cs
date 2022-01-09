using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplshDestroy : MonoBehaviour
{
    private void Awake() {
        Material mat = GetComponent<Renderer>().material;

        mat.SetFloat("_StartTime", Time.timeSinceLevelLoad);

        Vector4 durations = mat.GetVector("_Duration");
        float duration = Mathf.Max(durations.x, durations.y);
        Destroy(gameObject, duration);
    }
}
