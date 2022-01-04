using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ErrorHandling
{
    public static void CheckActive(GameObject gameObject)
    {
        if (gameObject.activeInHierarchy == true)
        {
            Debug.Log(gameObject.name + " is active");
        }
        else
        {
            Debug.Log(gameObject.name + " is inactive");
        }
    }
}
