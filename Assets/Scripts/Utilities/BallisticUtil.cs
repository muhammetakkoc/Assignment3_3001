using UnityEngine;

public class BallisticUtil
{

    /// <summary>
    /// Apply kinematic equation to calculate displacement given velocity, acceleration, time (v*t + 0.5*a*t^2)
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="acceleration"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static Vector3 CalculateKinematicDisplacement(Vector3 velocity, Vector3 acceleration, float time)
    {
        return (velocity * time) + (0.5f * acceleration * time * time);
    }


    public static float EstimateFlightTime(float range, float closingSpeed)
    {
        if (closingSpeed <= 0.00001)
        {
            return float.PositiveInfinity;
        }

        return range / closingSpeed;
    }

    /// <summary>
    /// Returns time taken for object moving at relativeVelocity to travel displacement, or +INF if not the correct direction
    /// </summary>
    /// <param name="displacementToTarget"></param>
    /// <param name="relativeVelocity"></param>
    /// <returns></returns>
    public static float EstimateFlightTime(Vector3 displacementToTarget, Vector3 relativeVelocity)
    {
        float range = displacementToTarget.magnitude;
        Vector3 directionToTarget = displacementToTarget / range;
        float closingVelocity = Vector3.Dot(directionToTarget, relativeVelocity);

        return EstimateFlightTime(range, closingVelocity);
    }

    /// <summary>
    /// Return intercept course as out parameters. If not able to catch, then returns false.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target"></param>
    /// <param name="targetPosition"></param>
    /// <param name="eta"></param>
    /// <param name="leadTimeScale"></param>
    /// <param name="leadTimeOffset"></param>
    /// <param name="etaTolerance"></param>
    /// <returns></returns>
    public static bool GetTargetLead(Rigidbody self, Rigidbody target, out Vector3 targetPosition, out float eta, float leadTimeScale = 1, float leadTimeOffset = 0, float etaTolerance = 100000)
    {
        bool success = false;
        Vector3 displacementToTarget = target.transform.position - self.transform.position;
        Vector3 newRelativeVelocity = self.velocity - target.velocity;

        float newTimeToTarget = EstimateFlightTime(displacementToTarget, newRelativeVelocity);
        if (newTimeToTarget > etaTolerance)
        {
            newTimeToTarget = etaTolerance;
        }
        else
        {
            success = true;
        }

        eta = newTimeToTarget;
        targetPosition = target.transform.position + (target.GetComponent<Rigidbody>().velocity * ((leadTimeScale * eta) + leadTimeOffset));
        return success;
    }

    /// <summary>
    /// Assumes gravity
    /// </summary>
    /// <param name="muzzleSpeed"></param>
    /// <param name="verticalDisplacement"></param>
    /// <param name="horizontalDisplacement"></param>
    /// <param name="elevationAngle1"></param>
    /// <param name="elevationAngle2"></param>
    /// <returns></returns>
    public static bool ComputeElevationAngle(float muzzleSpeed, float verticalDisplacement, float horizontalDisplacement, out float elevationAngle1, out float elevationAngle2)
    {
        float halfGravity = -Physics.gravity.y * 0.5f;
        float q = (halfGravity * horizontalDisplacement * horizontalDisplacement) / (muzzleSpeed * muzzleSpeed); //halfGravity * horizontalDisplacement / muzzleSpeed * muzzleSpeed;

        // float a = halfGravity * horizontalDisplacement * horizontalDisplacement / muzzleSpeed * muzzleSpeed;
        int solutions = Util.QuadraticFormula(q, horizontalDisplacement, (q - verticalDisplacement), out float solution1, out float solution2);

        if (solutions > 0)
        {
            elevationAngle1 = Mathf.Atan(solution1) * Mathf.Rad2Deg;
            elevationAngle2 = Mathf.Atan(solution2) * Mathf.Rad2Deg;
            return true;
        } else
        {
            elevationAngle1 = float.NaN;
            elevationAngle2 = float.NaN;
            return false;
        }    
    }
}
