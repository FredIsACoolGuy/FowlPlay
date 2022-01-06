using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplashScript : MonoBehaviour
{
    public GameObject splash;
    public float yOffset; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("SPLASHHHHING");
        Instantiate(splash, new Vector3(other.transform.position.x,transform.position.y+yOffset, other.transform.position.z), Quaternion.identity);
    }
}
