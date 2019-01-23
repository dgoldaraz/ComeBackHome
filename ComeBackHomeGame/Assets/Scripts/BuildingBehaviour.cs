using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehaviour : MonoBehaviour {

    public GameObject EnemyPrefab;
    public int MaxNumberOfEnemies = 1;
    //Flag to set that this enemy has nbo building attached
    public bool SoloPlay = false;

    private float mSpeed = 1;
    List<GameObject> Enemies;

    public void Start()
    {
        if (SoloPlay)
        {
            Init(0.0f);
        }
    }

    // Use this for initialization
    public void Init (float Speed)
    {
        mSpeed = Speed;
        GroundMovement GrCmp = GetComponent<GroundMovement>();
        if (GrCmp)
        {
            GrCmp.Speed = mSpeed;
        }

        if (EnemyPrefab != null)
        {
            Enemies = new List<GameObject>();
            //Create a save Enemies 
            int nEnemies = Random.Range(1, MaxNumberOfEnemies);
            for (int i = 0; i < nEnemies; i++)
            {
                GameObject Enemy = Instantiate(EnemyPrefab, this.transform.position, Quaternion.identity);
                EnemyBehaviour EnBCmp = Enemy.GetComponent<EnemyBehaviour>();
                if(EnBCmp)
                {
                    EnBCmp.Init(this.gameObject, mSpeed);
                }

                Enemies.Add(Enemy);
            }
        }
        else
        {
            Debug.LogError("No Enemy Prefab set");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //If the player is in a radius of action, apply effect
}
