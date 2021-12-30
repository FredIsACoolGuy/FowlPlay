using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && other.transform != transform.parent.parent)
        {
            Debug.Log("Yay "+ other.gameObject.name);

            other.SendMessage("Knockback", transform.position);
        }
    }
}
