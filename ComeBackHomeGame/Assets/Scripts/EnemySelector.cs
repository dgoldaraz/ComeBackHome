using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to select which enamy should continue following the player
public class EnemySelector : MonoBehaviour
{
    GameObject EnemyFollowing = null;

    public bool CanBeFollowed(GameObject Enemy)
    {
        return EnemyFollowing == Enemy || EnemyFollowing == null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Get Enemy following
        if (collision.gameObject != EnemyFollowing && collision.tag == "Enemy")
        {
            EnemyBehaviour EnemyBh = collision.gameObject.GetComponent<EnemyBehaviour>();
            if (EnemyFollowing == null)
            {
                EnemyFollowing = collision.gameObject;
                EnemyBh.ChangeState += EnemyStateChanged;
            }
            else
            {
                EnemyBh.StopFollowing();
            }
        }
    }
    //Create listener function to know when the enmy stops following

    void EnemyStateChanged(EnemyBehaviour.State NewState)
    {
        if(NewState == EnemyBehaviour.State.Walking || NewState == EnemyBehaviour.State.Idle)
        {
            //Stops listening to enamy and clean enemySelector
            EnemyFollowing.GetComponent<EnemyBehaviour>().ChangeState -= EnemyStateChanged;
            EnemyFollowing = null;
        }
    }
}
