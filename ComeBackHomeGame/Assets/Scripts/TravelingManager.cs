using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelingManager : MonoBehaviour {


    /*
     * Class that have a list of Floor prefabs and will upate it properly
     */

    public GameObject FloorPrefab;

    public GameObject BuildingPrefab;
    public GameObject EnemyPrefab;


    public int FloorObjects = 2;
    public float Speed = 1.0f;
    private float EndWorldY = -10.0f;
    private float StartWorldY = 10.0f;
    private List<GameObject> mFloorObjects;

    private float CameraVertBound = 0;
    private float CameraHorzBound = 0;
    private bool bMove = true;


    // Use this for initialization
    void Start ()
    {
        //Get Camera Min/Max values
        CameraVertBound = Camera.main.orthographicSize;
        CameraHorzBound = CameraVertBound * Screen.width / Screen.height;

        if (FloorPrefab)
        {
            SpriteRenderer FloorSpr = FloorPrefab.GetComponent<SpriteRenderer>();
            if(FloorSpr)
            {
                mFloorObjects = new List<GameObject>();
                float SprtSizeY = FloorSpr.size.y;
                Vector3 Position = transform.position;
                for (int i = 0; i < FloorObjects; i++)
                {
                    GameObject Floor = Instantiate(FloorPrefab, Position, Quaternion.identity, this.transform);
                    CreateEnvironment(Floor);
                    mFloorObjects.Add(Floor);
                    Position.y = Position.y + SprtSizeY;
                }

                StartWorldY = SprtSizeY * (FloorObjects - 1);
                EndWorldY = SprtSizeY * -1f;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(bMove)
        {
            foreach (GameObject Go in mFloorObjects)
            {
                //Move based on the speed
                Go.transform.Translate(Vector3.down * Speed * Time.deltaTime);
                if (Go.transform.position.y < EndWorldY)
                {
                    Go.transform.position = new Vector3(Go.transform.position.x, StartWorldY, Go.transform.position.z);
                    //Create new Scenary
                    CreateEnvironment(Go);
                }
            }
        }
	}

    void CreateEnvironment(GameObject Floor)
    {
        //Calculate Boundaries
        SpriteRenderer FloorSpr = Floor.GetComponent<SpriteRenderer>();
        if (FloorSpr)
        {
            //Randomly select number of buildings (between 0 - 4)
            // The select a random position for them being upRight, DownRight, UpLeft, DownLeft
            int nBuildings = Random.Range(0, 5);
            if(nBuildings > 0)
            {
                float SprtSizeY = FloorSpr.size.y * 0.25f;
                float MinX = Floor.transform.position.x - CameraHorzBound;
                float MaxX = Floor.transform.position.x + CameraHorzBound;

                float MinY = Floor.transform.position.y - SprtSizeY;
                float MaxY = Floor.transform.position.y + SprtSizeY;

                List<Vector3> Positions = new List<Vector3>();
                Positions.Add(new Vector3(MinX, MinY, 0.0f));
                Positions.Add(new Vector3(MinX, MaxY, 0.0f));
                Positions.Add(new Vector3(MaxX, MinY, 0.0f));
                Positions.Add(new Vector3(MaxX, MaxY, 0.0f));
                for (int i = 0; i < nBuildings; i++)
                {
                    //Choose a random position
                    int Index = Random.Range(0, Positions.Count);
                    Vector3 NewPos = Positions[Index];
                    Positions.RemoveAt(Index);
                    GameObject NewObj = Instantiate(BuildingPrefab, NewPos, Quaternion.identity, this.transform);
                    BuildingBehaviour BuildingCmp = NewObj.GetComponent<BuildingBehaviour>();
                    if(BuildingCmp != null)
                    {
                        BuildingCmp.Init(Speed);
                    }
                }
            }
        }
    }

    public void Stop()
    {
        bMove = false;
    }

    public void ReStart()
    {
        bMove = true;
    }
}
