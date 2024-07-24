using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class NavigationUtil
{
    /// <summary>
    /// Get size of bounds in pixels
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Vector2 GetScreenSpaceSize(Bounds bounds, Camera camera)
    {
        Vector4 ndc = GetClipSpaceSize(bounds, camera);
        return ndc / ndc.w;
    }

    /// <summary>
    /// Get size of bounds in normalized device coordinates
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Vector4 GetClipSpaceSize(Bounds bounds, Camera camera)
    {
        Vector4 size = new Vector4(bounds.size.x, bounds.size.y, bounds.size.z, 1);
        return camera.worldToCameraMatrix * size;
    }

   // public float ShortestRotationFromTo(float fromEulerDegrees, float toEulerDegrees)
   // {
   //     //Place both in terms of from
   //     float delta = toEulerDegrees - fromEulerDegrees;
   //     return 
   // }

    /// <summary>
    /// Returns Atan2 result of direction. Assumes directionNormal is normalized and in space relative to the parent (y of directionNormal should be parent's y)
    /// </summary>
    /// <param name="directionNormal"></param>
    /// <returns></returns>
    public static Vector2 DirectionToElevationAzimuth(Vector3 directionNormal)
    {
        float pitch = Mathf.Atan2(directionNormal.y, directionNormal.z);
        float yaw = Mathf.Atan2(directionNormal.x, directionNormal.z);
        return new Vector2(pitch, yaw);
    }


    /// <summary>
    /// Returns Atan2 result of direction but in degrees. Assumes directionNormal is normalized and in space relative to the parent (y of directionNormal should be parent's y)
    /// </summary>
    /// <param name="directionNormal"></param>
    /// <returns></returns>
    public static Vector2 DirectionToElevationAzimuthDegrees(Vector3 directionNormal)
    {
        float pitch = Mathf.Atan2(directionNormal.y, directionNormal.z);
        float yaw = Mathf.Atan2(directionNormal.x, directionNormal.z);
        return new Vector2(pitch * Mathf.Rad2Deg, yaw * Mathf.Rad2Deg);
    }


    /// <summary>
    /// Returns normalized direction vector from Elevation(x) and Azimuth(y) in degrees
    /// </summary>
    /// <param name="eulerAnglesDegrees"></param>
    /// <returns></returns>
    public static Vector3 ElevationAzimuthToDirection(Vector2 eulerAnglesDegrees)
    {
        Quaternion TargetOrientation = Quaternion.Euler(eulerAnglesDegrees.x, eulerAnglesDegrees.y, 0);
        return TargetOrientation * Vector3.forward;
    }

    public static float Wrap360(float inEulerDegrees)
    {
        return (inEulerDegrees + 10800) % 360.0f;
    }

    /// <summary>
    /// For elevation/pitch angles. Converts angle range of (0,360) with 0/360 being forward and 90 being down to a 180-degree system of (-90,90), where 0 is forward, -90 is down, and 90 is up.
    /// </summary>
    /// <returns></returns>
    public static float EulerToElevationAngleDegrees(float inEulerXDegrees)
    {
        inEulerXDegrees = (Wrap360(inEulerXDegrees));
        if(inEulerXDegrees < 180.0f)
        {
            return Util.LmapUnclamped(inEulerXDegrees, 0, 180, 0, -180);
        } else
        {
            return Util.LmapUnclamped(inEulerXDegrees, 180, 360, 180, 0);
        }
    }

    /// <summary>
    /// Returns how much roll is needed to align up vector, in radians. 
    /// Assumes directionNormal is normalized and in space relative to the parent (y of directionNormal should be parent's y). Vector3.up is 0 roll.
    /// If directionNormal XY component is less than errorTolerance, will return zero. This can help reduce how much it flips around if directionNormal's X and Y components are close to zero.
    /// </summary>
    /// <param name="directionNormal"></param>
    /// <param name="errorTolerance"></param>
    /// <returns></returns>
    public static float GetRollErrorToTargetRad(Vector3 directionNormal, float errorTolerance = 0.01f)
    {
        Quaternion rotate90 = Quaternion.Euler(0, 0, -90);
        Vector3 TargetHeadingLocalSpaceNormalizedRollAxis = rotate90 * directionNormal;

        Vector2 TargetXYPlane = new Vector2(TargetHeadingLocalSpaceNormalizedRollAxis.x, TargetHeadingLocalSpaceNormalizedRollAxis.y);
        if(TargetXYPlane.sqrMagnitude < errorTolerance * errorTolerance)
        {
            return 0;
        }

        return Mathf.Atan2(TargetXYPlane.y, TargetXYPlane.x);
    }
}
