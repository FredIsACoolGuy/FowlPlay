using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupScript : NetworkBehaviour
{
    public float bounceHeight=1;
    public float bounceTime=10;

    private Vector3 offset;
    
    [Server]
    void Update()
    {
        offset = new Vector3(0f,Mathf.Sin(Time.frameCount/bounceTime)*bounceHeight, 0f);
        transform.position = transform.position + offset;
    }

    //[Server]
    //private void OnTriggerEnter(Collider other)
    //{
    //    Destroy(this.gameObject);
    //}
}
