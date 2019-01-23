using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour {

    public float Speed = 1.0f;
    public float EndWorldY = -10.0f;
    public float StartWorldY = 10.0f;
    private bool bActive = true;
	
	// Update is called once per frame
    private void FixedUpdate()
    {
        if (bActive)
        {
            transform.Translate(Vector3.down * Speed * Time.deltaTime);
        }
    }

    public void Stop()
    {
        bActive = false;
    }

    public void ReStart()
    {
        bActive = true;
    }
}
