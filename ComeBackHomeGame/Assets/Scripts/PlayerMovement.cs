using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float Speed = 100.0f;
    public Vector2 HorizontalOffset = new Vector2(-300.0f, 300.0f);
    public Vector2 VerticalOffset = new Vector2(-5.0f, 5.0f);

    private float NewHMovement = 0;
    private float NewVMovement = 0;

    private bool bMove = true;
    // Update is called once per frame
    void Update()
    {
        NewHMovement = Input.GetAxis("Horizontal");
        NewVMovement = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if(bMove)
        {
            float NewXPosition = (NewHMovement * Time.fixedDeltaTime * Speed);
            float NewYPosition = (NewVMovement * Time.fixedDeltaTime * Speed);

            Vector3 Movement = new Vector3(NewXPosition, NewYPosition, 0.0f);

            transform.Translate(Movement);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, VerticalOffset.x, VerticalOffset.y), Mathf.Clamp(transform.position.y, HorizontalOffset.x, HorizontalOffset.y), transform.position.z);
        }
    }

    public void ChangeMovement(bool bNewMove)
    {
        bMove = bNewMove;
    }

}
