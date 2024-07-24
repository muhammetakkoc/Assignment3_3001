//Thanks to https://medium.com/p/4941d3fda905 for this bit of code!

using UnityEngine;

public static class DebugDrawing
{
    public static void DrawLineDotted(Vector3 start, Vector3 end, float segmentLength, float gapLength, Color color, float duration, bool depthTest = true)
    {
        Vector3 totalDisplacement = end - start;
        Vector3 direction = totalDisplacement / totalDisplacement.magnitude;

        Vector3 pointA = start;
        for (float distanceLeft = totalDisplacement.magnitude; distanceLeft > 0.001f;)
        {
            float length = Mathf.Min(segmentLength, distanceLeft);
            Vector3 pointB = pointA + direction * length;
            Debug.DrawLine(pointA, pointB, color, duration, depthTest);

            pointA = pointB + direction * gapLength;

            distanceLeft -= length;
            distanceLeft -= gapLength;
        }
    }

    public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, int segments, Color color, float duration, bool depthTest = true)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);
            lineStart.z = 0;

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));
            lineEnd.z = 0;

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            //Rotate it!
            lineStart = rotation * lineStart;
            lineEnd = rotation * lineEnd;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            Debug.DrawLine(lineStart, lineEnd, color, duration, depthTest);
        }
    }

    public static void DrawCircleDotted(Vector3 position, Quaternion rotation, float radius, int segments, float segmentLength, float gapLength, Color color, float duration, bool depthTest = true)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);
            lineStart.z = 0;

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));
            lineEnd.z = 0;

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            //Rotate it!
            lineStart = rotation * lineStart;
            lineEnd = rotation * lineEnd;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            DebugDrawing.DrawLineDotted(lineStart, lineEnd, segmentLength, gapLength, color, duration, depthTest);
        }
    }

    public static void DrawQuad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color color)
    {
        // Draw lines between the points
        Debug.DrawLine(pointA, pointB, color);
        Debug.DrawLine(pointB, pointC, color);
        Debug.DrawLine(pointC, pointD, color);
        Debug.DrawLine(pointD, pointA, color);
    }

    // Draw a rectangle defined by its position, orientation and extent
    public static void DrawRectangle(Vector3 position, Quaternion orientation, Vector2 extent, Color color)
    {
        Vector3 rightOffset = Vector3.right * extent.x * 0.5f;
        Vector3 upOffset = Vector3.up * extent.y * 0.5f;

        Vector3 offsetA = orientation * (rightOffset + upOffset);
        Vector3 offsetB = orientation * (-rightOffset + upOffset);
        Vector3 offsetC = orientation * (-rightOffset - upOffset);
        Vector3 offsetD = orientation * (rightOffset - upOffset);

        DrawQuad(position + offsetA,
                    position + offsetB,
                    position + offsetC,
                    position + offsetD,
                    color);
    }
}
