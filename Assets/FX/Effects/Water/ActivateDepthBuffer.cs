using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDepthBuffer : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
