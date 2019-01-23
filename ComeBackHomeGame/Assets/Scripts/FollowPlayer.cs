using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    //Ref to Enemy class
    public EnemyBehaviour EnemyBehaviour;
    public float MinDistance = 2.0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Check if we collide with the player, if that's true, try to follow him
        if (collision.tag == "Player" && Vector3.Distance(transform.position, collision.transform.position) > MinDistance)
        {
            //Get Enemy Selector cmp and check if it can be follow
            EnemySelector EnmSlt = collision.gameObject.GetComponentInChildren<EnemySelector>();
            if(EnmSlt && EnmSlt.CanBeFollowed(this.gameObject))
            {
                EnemyBehaviour.AskFollow(collision.gameObject);
            }
        }
    }
}
