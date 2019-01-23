using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructOnCollide : MonoBehaviour {


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Destructor")
        {
            Destroy(this.gameObject);
        }
        
    }
}
