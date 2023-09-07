using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //movement values
    Vector2 target;
    public float WalkSpeed = 5f;
    //public float RunSpeed = 8f;
    public float SpeedLimiter = 0.7f;
    float inputHorizontal;
    float inputVertical;

    //Establishes values used for detection
    public float viewRadius = 5f;
    [Range(1, 360)] public float viewAngle = 45f;
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask wallMask;
    public bool canSeePlayer;


    // Start is called before the first frame update
    void Start()
    {
        //Sets the reference to player and starts the FOVRoutine at the start
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    //Has the enemy run field of view every 0.2 seconds instead of every frame
    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }


    // Enemy Line of Sight
    private void FieldOfViewCheck()
    {
        //Creats an array for objects in the target layer that are within the viewable radius of the object
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        //If the array's length isn't 0, than an object in the target layer is in the radius
        if (rangeCheck.Length != 0)
        {
            //Checks the direction and angle target is in relation to object
            Transform target = rangeCheck[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            //Sees if the target is within viewable area, if not it doesn't see target
            if (Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                //Checks to see if there is an object on wall layer between it and target, if not it sees target
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, wallMask))
                {
                    canSeePlayer = true;
                    Debug.Log("Player spoted by enemy!");
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            Debug.Log("Enemy has lost sight of player");
        }
    }


    //Shows the objects viewable radius and the cone of sight for debug purposes
    private void OnDrawGizmos()
    {
        //Shows the radius as a white circle
        Gizmos.color = Color.white;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, viewRadius);

        //Shows the cone of view as two green lines
        Vector3 viewAngle1 = DirectionFromviewAngle(-transform.eulerAngles.z, -viewAngle * 0.5f);
        Vector3 viewAngle2 = DirectionFromviewAngle(-transform.eulerAngles.z, viewAngle * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + viewAngle1 * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngle2 * viewRadius);

        //If the object can see player, it draws a red line towards player
        if (canSeePlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerRef.transform.position);
        }

    }
    //Method called above to calculate the angles of sight, place here to make format easier to read
    private Vector2 DirectionFromviewAngle(float eulerY, float viewAngleDegrees)
    {
        viewAngleDegrees += eulerY;
        return new Vector2(Mathf.Sin(viewAngleDegrees * Mathf.Deg2Rad), Mathf.Cos(viewAngleDegrees * Mathf.Deg2Rad));
    }


    // Update is called once per frame
    void Update()
    {
        //If unable to see player, object rotates at a speed equal to walkSpeed
        if (canSeePlayer == false)
        {
            transform.Rotate(0, 0, WalkSpeed * Time.deltaTime);
        }
        //If able to see player, object will rotate to follow the player position
        else if (canSeePlayer)
        {
            Vector3 Look = transform.InverseTransformPoint(playerRef.transform.position);
            float targetAngle = Mathf.Atan2(Look.y, Look.x) * Mathf.Rad2Deg - 90;
            transform.Rotate(0, 0, targetAngle);
        }
    }
}