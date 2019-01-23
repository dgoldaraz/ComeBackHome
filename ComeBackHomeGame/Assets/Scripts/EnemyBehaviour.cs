using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*\
 Basic Enemy Behaviour class.
 The basic enemy will wander around until he finds the player, then he attach to the player and try to move to home
 If after some time it's not possible, he start wandering around again
*/
public class EnemyBehaviour : MonoBehaviour
{
    //Enumus
    public enum State { Idle, Walking, Reaching, Talking, Dragging }

    //Delegates
    public delegate void ChangeStateDelegate(State NewState);
    public ChangeStateDelegate ChangeState;

    // Public Vars
    public float MaxTimePerState = 2.0f;
    public float MaxTimeReachingPlayer = 2.0f;
    public float WalkSpeed = 1.0f;
    public float ReachSpeed = 5.0f;
    public float WalkingRotationSpeed = 1.0f;
    public float ReachingRotationSpeed = 10.0f;

    //Time to stop following player for this enemy
    public float DeltaTimeNotReaching = 1.0f;

    [Range(0, 1)]
    public float IdleProbability = 0.25f;
    public float MinAngleRotation = 3.0f;
    //Flag to set that this enemy has nbo building attached
    public bool SoloPlay = false;
    public float RangeFarToTarget = 2.0f;

    //Private Vars
    private State mState = State.Idle;
    private GameObject mBuilding;
    private float TimeOnSameState = 0.0f;

    private Vector3 TargetPosition;
    private GroundMovement GrMvment = null;

    private bool bRotate = true;
    private int RangeToTarget = 1;

    private GameObject ActivePlayer;
    private bool bNotFollow = false;
    private GroundMovement GrCmp = null;

    private bool bBlocked = false;
    private bool bAllowToTalk = true;

    public void Start()
    {
        if (SoloPlay)
        {
            Init(null, 1.0f);
        }
    }

    // Use this for initialization
    public void Init(GameObject Building, float Speed)
    {
        mBuilding = Building;
        GrCmp = GetComponent<GroundMovement>();
        if (GrCmp)
        {
            GrMvment = GrCmp;
            GrCmp.Speed = Speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!bBlocked)
        {
            TimeOnSameState += Time.deltaTime;
            switch (mState)
            {
                case State.Idle:
                    {
                        //Stay on same place 
                        break;
                    }
                case State.Walking:
                    {
                        ReachPosition(GetTarget());
                        break;
                    }
                case State.Reaching:
                    {
                        ReachPosition(GetTarget());
                        break;
                    }
                case State.Dragging:
                    {
                        ReachPosition(GetHomePosition(),true);
                        break;
                    }
                default:
                    break;
            }

            UpdateState();
        }
        else
        {
            if(mState == State.Walking)
            {
                ReachPosition(GetTarget());
            }
        }
     
    }

    private void UpdateState()
    {
        if (NeedsToChangeState())
        {
            ChooseNewState();
        }
    }

    //TODO: Review who is calling this to avoid duplications
    private Vector3 GetTarget()
    {
        switch (mState)
        {
            case State.Reaching:
                {
                    return ActivePlayer.transform.position;
                }
            default:
                {
                    return TargetPosition;
                }
        }
    }

    private Vector3 GetHomePosition()
    {
        if(mBuilding)
        {
            return mBuilding.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private bool NeedsToChangeState()
    {
        switch (mState)
        {
            case State.Walking:
                {
                    return Vector3.Distance(transform.position, GetTarget()) < RangeToTarget;
                }
            case State.Reaching:
                {
                    bool DistanceCheck = Vector3.Distance(transform.position, GetTarget()) < RangeToTarget;
                    bool DistanceFarCheck = Vector3.Distance(transform.position, GetTarget()) > RangeFarToTarget;
                    bool TimeCheck = TimeOnSameState > MaxTimeReachingPlayer;
                    return DistanceCheck || DistanceFarCheck || TimeCheck;
                }
            case State.Talking:
                {
                    return !bAllowToTalk;
                }
            case State.Dragging:
                {
                    //Check if we are close to home
                    //TODO: Move this into Player Code
                    if (Vector3.Distance(transform.position, GetHomePosition()) < 1.0f)
                    {
                        ActivePlayer.GetComponent<PlayerMovement>().ChangeMovement(true);
                        ActivePlayer.transform.rotation = Quaternion.identity;
                        return true;
                    }
                    return false;
                }
            default:
                {
                    return TimeOnSameState > MaxTimePerState;
                }
        }
    }

    void ChooseNewState()
    {
        bool DistanceCheck = Vector3.Distance(transform.position, GetTarget()) < RangeToTarget;
        if (DistanceCheck && bAllowToTalk)
        {
            
            //Search for a QuestManager
            QuestionaryEvent QuestMngr = GameObject.FindObjectOfType<QuestionaryEvent>();
            if(QuestMngr)
            {
                //Mini Game starts
                mState = State.Talking;
                QuestMngr.InitNewQuestionary(ActivePlayer, this.gameObject);
            }
        }
        else
        {
            //Select a new State
            if (Random.value > 0.25f)
            {
                //Move
                if (GrMvment)
                {
                    GrMvment.Stop();
                }

                mState = State.Walking;
                //Get a new Position
                float CameraVertBound = Camera.main.orthographicSize;
                float CameraHorzBound = CameraVertBound * Screen.width / Screen.height;

                TargetPosition = new Vector3(Random.Range(-CameraHorzBound, CameraHorzBound), Random.Range(-5, 15), 0);
                RangeToTarget = Random.Range(1, 3);
                bRotate = true;
            }
            else
            {
                //Idle
                mState = State.Idle;
                if (GrMvment)
                {
                    GrMvment.ReStart();
                }
            }
        }


        TimeOnSameState = 0.0f;
        if (ChangeState != null)
        {
            ChangeState(mState);
        }

    }

    void ReachPosition(Vector3 DesiredPosition, bool bDragPlayer = false)
    {
        //Move towards the Destiantion position
        Vector3 Direction = DesiredPosition - transform.position;
        Direction.Normalize();

        float Rot_z = GetRotationToTarget(DesiredPosition);
        bRotate = Mathf.Abs(Rot_z - transform.rotation.eulerAngles.z) > MinAngleRotation;

        switch (mState)
        {
            case State.Dragging:
            case State.Walking:
                {
                    WalkToPosition(this.gameObject, Direction, Rot_z);

                    //TODO: Move this into Player Code
                    if(bDragPlayer)
                    {
                        WalkToPosition(ActivePlayer, Direction, Rot_z);
                    }
                    break;
                }
            case State.Reaching:
                {
                    FollowToPosition(Direction, Rot_z);
                    break;
                }
            default:
                {
                    //Bug if we enter here
                    Debug.LogError("Reach Position in a invalid State on the Enemy");
                    break;
                }
        }
    }

    //Walk to posiution rotating first and moving later with lerp
    private void WalkToPosition(GameObject GO, Vector3 Direction, float RotZ)
    {
        if(!GO)
        {
            Debug.LogError("No GameObject passed to WalkToPosition");
            return;
        }

        //Check Rotation
        if (bRotate)
        {
            Quaternion NewQuaternion = Quaternion.Euler(0f, 0f, RotZ);
            //Upate Rotation if necessary
            GO.transform.rotation = Quaternion.Lerp(transform.rotation, NewQuaternion, Time.deltaTime * WalkingRotationSpeed);
        }
        else
        {
            bRotate = false;
            GO.transform.position = transform.position + (Direction * Time.deltaTime * WalkSpeed);
        }
    }

    //Much faster movement, avoid lerping and just try to catch the player
    private void FollowToPosition(Vector3 Direction, float RotZ)
    {
        //TODO: First time do a lerp on the rotation
        Quaternion NewQuaternion = Quaternion.Euler(0f, 0f, RotZ);
        //Upate Rotation if necessary
        transform.rotation = NewQuaternion;
        transform.position = transform.position + (Direction * Time.deltaTime * ReachSpeed);
    }

    public void AskFollow(GameObject PlayerF)
    {
        if (!bNotFollow || mState != State.Talking || TimeOnSameState > DeltaTimeNotReaching)
        {
            bNotFollow = false;
            mState = State.Reaching;
            ActivePlayer = PlayerF;
            RangeToTarget = 1;
            if (NeedsToRotate(ActivePlayer.transform.position))
            {
                bRotate = true;
            }
        }

    }

    public void StopFollowing()
    {
        if (mState == State.Reaching)
        {
            bNotFollow = true;
            ChooseNewState();
        }
    }

    public void BlockEnemy()
    {
        bBlocked = true;
        if (mState != State.Walking)
        {
            if(GrMvment)
            {
                GrMvment.Stop();
            }
        }
    }

    public void UnBlockEnemy()
    {
        bBlocked = false;
        if (mState == State.Idle)
        {
            if (GrMvment)
            {
                GrMvment.ReStart();
            }
        }
    }

    //Called when the player answer correc
    public void StopTalking()
    {
        bAllowToTalk = false;
    }

    public void MovePlayerToHome( GameObject PlayerToDrag )
    {
        //Method called when the Enemy needs to drag the player to home
        mState = State.Dragging;
        ActivePlayer = PlayerToDrag;
    }

    private float GetRotationToTarget(Vector3 DesiredPosition)
    {
        Vector3 Direction = DesiredPosition - transform.position;
        Direction.Normalize();
        float Rot_z = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        return Rot_z + 90;
    }

    private bool NeedsToRotate(Vector3 DesiredPosition)
    {
        float Rot_z = GetRotationToTarget(DesiredPosition);
        return Mathf.Abs(Rot_z - transform.rotation.eulerAngles.z) > MinAngleRotation;
    }

    void OnDrawGizmosSelected()
    {

        //Draw 2D "Fwd" vector
        Gizmos.color = Color.blue;
        Vector3 Fwd = this.transform.up * -1.0f;
        Gizmos.DrawLine(this.transform.position, this.transform.position + Fwd);

        // Draw a yellow sphere at the transform's position
        if (mState == State.Walking)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetTarget(), 0.5f);

            Gizmos.color = Color.red;
            Vector3 Direction = GetTarget() - transform.position;
            Direction.Normalize();
            Gizmos.DrawLine(this.transform.position, this.transform.position + Direction);

        }
        else if (mState == State.Reaching)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(GetTarget(), 0.5f);

            Gizmos.color = Color.red;
            Vector3 Direction = GetTarget() - transform.position;
            Direction.Normalize();
            Gizmos.DrawLine(this.transform.position, this.transform.position + Direction);
        }
    }
}
