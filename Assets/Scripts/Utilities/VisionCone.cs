
using System;
using UnityEngine;

[Serializable]
public class VisionCone
{
    public float halfAngle = 30.0f;
    public float range = 10000.0f;

    public bool IsTargetWithinCone(Ray rayWorldspace, Vector3 targetPosWorldspace)
    {
        Vector3 displacement = targetPosWorldspace - rayWorldspace.origin;

        float distance = displacement.magnitude;
        float angle = Vector3.Angle(rayWorldspace.direction, displacement / distance);

        return angle <= halfAngle && distance <= range;
    }
}