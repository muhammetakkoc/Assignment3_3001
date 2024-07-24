using System.Collections.Generic;
using UnityEngine;
public interface IMoveCommandable
{
    /// <summary>
    /// Get or set the move target
    /// </summary>
    public abstract Vector3 MoveTarget { get; set; }

    /// <summary>
    /// Get or set the current waypoint set. Waypoints are removed from the queue and automatically set as MoveTarget
    /// </summary>
    public abstract Queue<Vector3> Waypoints { get; set; }

    /// <summary>
    /// Should return true if command is successful, false otherwise
    /// </summary>
    /// <param name="moveTargetPosition"></param>
    /// <returns></returns>
    public bool RequestMoveTo(Vector3 moveTargetPosition);

    /// <summary>
    /// Respond to command
    /// </summary>
    public void AcknowledgeMoveCommand();

    /// <summary>
    /// Returns true if the Unit is done moving
    /// </summary>
    /// <returns></returns>
    public bool IsStoppedMoving();

    /// <summary>
    /// Returns true if the Unit is at its TargetPosition
    /// </summary>
    /// <returns></returns>
    public bool IsAtMoveTarget();

    /// <summary>
    /// Returns the distance tolerance to be satisfied that the Unit is "at the Move Target"
    /// </summary>
    /// <returns></returns>
    public float GetPositionTolerance();

    /// <summary>
    /// Command to stop where it is
    /// </summary>
    public void Stop();

    /// <summary>
    /// Returns the expected goal position after completing all waypoints -- either the MoveTarget if the queue is empty, or the last waypoint in the Queue
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFinalTargetLocation();

    /// <summary>
    /// Add a waypoint to the queue
    /// </summary>
    /// <param name="waypoint"></param>
    /// <returns></returns>
    public bool RequestAddWaypoint(Vector3 waypoint);

    /// <summary>
    /// Add a list of waypoints to the current path. Returns number of waypoints added
    /// </summary>
    /// <param name="waypointPath"></param>
    /// <returns></returns>
    public int RequestAddWaypoints(IEnumerable<Vector3> waypointPath);

    /// <summary>
    /// Set a list of waypoints as the current path
    /// </summary>
    /// <param name="waypointPath"></param>
    /// <returns></returns>
    public bool RequestSetWaypoints(IEnumerable<Vector3> waypointPath);

    /// <summary>
    /// set MoveTarget to the next waypoint
    /// </summary>
    public void NextWaypoint();

}