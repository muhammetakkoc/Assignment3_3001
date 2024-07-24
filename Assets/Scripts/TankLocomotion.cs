using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankLocomotion : MonoBehaviour, IMoveCommandable
{
    private float speedMax = 25.0f;
    public float SpeedMax { get { return speedMax; } }
    public float Speed { get { return Rigidbody.velocity.magnitude; } }

    private float acceleration = 70;
    public float Acceleration { get { return acceleration; } } 
    private float rotationSpeed = 500;
    public float RotationSpeed {  get { return rotationSpeed; } }
    private float brakingAcceleration = 100.0f;
    public float BrakingAcceleration { get { return brakingAcceleration; } }

    private Vector3 positionError;
    private float angleError;

    public Vector3 MoveTarget { get { return (waypoints.Count > 0)? waypoints.Peek() : transform.position; } set { RequestMoveTo(value); } }


    private Vector3 lastWaypoint;
    private Queue<Vector3> waypoints = new Queue<Vector3>();
    public Queue<Vector3> Waypoints { get { return waypoints; } set { RequestSetWaypoints(value); } }

    private float positionErrorTolerance = 10.0f;
    private float angleErrorTolerance = 20.0f;

    Rigidbody Rigidbody;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public bool RequestMoveTo(Vector3 targetPositionIn)
    {
        waypoints.Clear();
        return RequestAddWaypoint(targetPositionIn);
    }

    public void AcknowledgeMoveCommand()
    {
       // Debug.Log(name + " moving to: " + MoveTarget);
    }

    public void DebugDrawWaypoints()
    {
        //Debug.DrawLine(transform.position, moveTarget, Color.green, Time.fixedDeltaTime);

        Debug.DrawLine(transform.position, MoveTarget, Color.green);

        Vector3 pointA = MoveTarget;
        foreach (Vector3 pointB in waypoints)
        {
            Debug.DrawLine(pointA, pointB, Color.green);
            pointA = pointB;
        }
    }

    public void RotateToFace(Vector3 targetDirection)
    {
        Vector3 newForward = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 9999);
        Vector2 targetElevationAzimuth = NavigationUtil.DirectionToElevationAzimuthDegrees(newForward);

        //rotate  Y directly
        Rigidbody.rotation = Quaternion.Euler(0, targetElevationAzimuth.y, 0);
    }

    public void MoveInDirection(Vector3 targetDirection)
    {
        Rigidbody.AddForce(targetDirection * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
        Rigidbody.velocity = Vector3.ClampMagnitude(Rigidbody.velocity, speedMax);
    }

    public void Brake()
    {
        Vector3 velocity = Rigidbody.velocity;
        Vector3 brakeVelocityChange = Vector3.ClampMagnitude(-(velocity.normalized) * brakingAcceleration * Time.fixedDeltaTime, velocity.magnitude);
        Rigidbody.AddForce(brakeVelocityChange, ForceMode.VelocityChange);
    }

    void FixedUpdate()
    {
        DebugDrawWaypoints();

        AttemptMoveToTarget();
    }

    private void AttemptMoveToTarget()
    {
        positionError = MoveTarget - transform.position;
        angleError = Vector3.SignedAngle(transform.forward, positionError.normalized, Vector3.up);

        if(IsAtMoveTarget() && waypoints.Count > 0)
        {
            NextWaypoint();
        }

        if (!IsAtMoveTarget())
        {
            RotateToFace(positionError.normalized);

            //turn to face target direction
            if (Mathf.Abs(angleError) > angleErrorTolerance)
            {
                Brake();

            }
            else // Move with acceleration
            {
                MoveInDirection(transform.forward);
            }
        }
        else
        {
           // if (positionError.sqrMagnitude > 0.001f)
           // {
           //     RotateToFace(positionError.normalized);
           // }

            Brake();
        }

        //Project onto X-Z plane
        Rigidbody.position = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        //Dissallow rotation on X or Z axis
        Rigidbody.rotation = Quaternion.Euler(0, Rigidbody.rotation.eulerAngles.y, 0);
    }

    public bool IsStoppedMoving()
    {
        return (Rigidbody.velocity.sqrMagnitude < 1.0f);
    }

    public bool IsAtMoveTarget()
    {
        return (positionError.sqrMagnitude < positionErrorTolerance * positionErrorTolerance);
    }

    public bool RequestAddWaypoint(Vector3 waypoint)
    {
        waypoints.Enqueue(waypoint);
        lastWaypoint = waypoint;
        return true;
    }

    public int RequestAddWaypoints(IEnumerable<Vector3> waypointPath)
    {
        int numAdded = 0;
       foreach(Vector3 point in waypointPath)
        {
            if(!RequestAddWaypoint(point))
            {
                return numAdded;
            } else
            {
                numAdded++;
            }
        }
       return numAdded;
    }

    public bool RequestSetWaypoints(IEnumerable<Vector3> waypointPath)
    {
        waypoints.Clear();
        bool success = true;
        foreach(Vector3 waypoint in waypointPath)
        {
            success &= RequestAddWaypoint(waypoint);
        }
        return success;
    }

    public float GetPositionTolerance()
    {
        return positionErrorTolerance;
    }

    public void NextWaypoint()
    {
        waypoints.Dequeue();
    }

    public Vector3 GetFinalTargetLocation()
    {
        if(waypoints.Count > 1)
        {
            return lastWaypoint;
        } else
        {
            return MoveTarget;
        }
    }

    public void Stop()
    {
        waypoints.Clear();
    }
}
