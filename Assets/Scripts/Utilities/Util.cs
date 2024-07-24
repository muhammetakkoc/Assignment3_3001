using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    /// <summary>
    /// A more reasonable epsilon for checking before normalizing small vectors etc.
    /// </summary>
    public const float Epsilon = 0.0001f;

    /// <summary>
    /// Clamp each component of a vector to a given size
    /// </summary>
    /// <param name="inVec"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 ClampComponents(Vector3 inVec, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(inVec.x, min, max),
            Mathf.Clamp(inVec.y, min, max),
            Mathf.Clamp(inVec.z, min, max)
            );
    }

    /// <summary>
    /// Calculates radius of a cone with distance height and angle of halfAngleDegrees
    /// </summary>
    /// <param name="halfAngleDegrees"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float CalculateConeRadiusAtDistance(float halfAngleDegrees, float distance)
    {
        if (distance <= Epsilon)
        {
            return 0;
        }
        else
        {
            float tan = Mathf.Tan(halfAngleDegrees * Mathf.Deg2Rad) * distance;

            return tan;
        }
    }
    /// <summary>
    /// returns x if given variables in from an expression of 0 = ax^2 + bx + c, and number of solutions. If there is one solution, both x1 and x2 are the same
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="x1"></param>
    /// <param name="x2"></param>
    /// <returns></returns>
    public static int QuadraticFormula(float a, float b, float c, out float x1, out float x2)
    {
        float determinant = (b * b) - (4 * a * c);
        if (determinant < 0)
        {
            x1 = float.NaN;
            x2 = float.NaN;
            return 0; // no solutions
        }
        float root = Mathf.Sqrt(determinant);
        if (determinant > 0) // two solutions
        {

            x1 = (-b - root) / (2 * a);
            x2 = (-b + root) / (2 * a);
            return 2;
        }
        else
        {
            x1 = (-b + root) / (2 * a); // one solution
            x2 = x1;
            return 1;
        }
    }
    /// <summary>
    /// Returns the largest of the components of the vector
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static float Max(Vector3 vector)
    {
        return Mathf.Max(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], clamped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    //public static T Lmap<T>(T val, T fromStart, T fromEnd, T toStart, T toEnd)
    //{
    //    float tVal = (val - fromStart) / (fromEnd - fromStart);
    //    tVal = Mathf.Clamp01(tVal);
    //    return ((1.0f - tVal) * toStart) + (toEnd * tVal);
    //}

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], clamped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static float Lmap(float val, float fromStart, float fromEnd, float toStart, float toEnd)
    {
        var tVal = (val - fromStart) / (fromEnd - fromStart);
        tVal = Mathf.Clamp01(tVal);
        return ((1.0f - tVal) * toStart) + (toEnd * tVal);
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], unclamped
    /// Range is unclamped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static float LmapUnclamped(float val, float fromStart, float fromEnd, float toStart, float toEnd)
    {
        var tVal = (val - fromStart) / (fromEnd - fromStart);
        return ((1.0f - tVal) * toStart) + (toEnd * tVal);
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], clamped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static double Lmap(double val, double fromStart, double fromEnd, double toStart, double toEnd)
    {
        var tVal = (val - fromStart) / (fromEnd - fromStart);
        tVal = (tVal > 1) ? 1 : (tVal < 0) ? 0 : tVal;
        return ((1.0 - tVal) * toStart) + (toEnd * tVal);
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], unclamped
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static double LmapUnclamped(double val, double fromStart, double fromEnd, double toStart, double toEnd)
    {
        var tVal = (val - fromStart) / (fromEnd - fromStart);
        return ((1.0 - tVal) * toStart) + (toEnd * tVal);
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], clamped. Rounds ints down
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static int Lmap(int val, int fromStart, int fromEnd, int toStart, int toEnd)
    {
        float tVal = (float)(val - fromStart) / (float)(fromEnd - fromStart);
        tVal = Mathf.Clamp01(tVal);
        return (int)(((1.0f - tVal) * (float)toStart) + ((float)toEnd * tVal));
    }

    /// <summary>
    /// Linearly map val from the range [fromStart, fromEnd] to the range [toStart, toEnd], unclamped. Rounds ints down
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    /// <param name="fromStart"></param>
    /// <param name="fromEnd"></param>
    /// <param name="toStart"></param>
    /// <param name="toEnd"></param>
    /// <returns></returns>
    public static int LmapUnclamped(int val, int fromStart, int fromEnd, int toStart, int toEnd)
    {
        float tVal = (float)(val - fromStart) / (float)(fromEnd - fromStart);
        return (int)(((1.0f - tVal) * (float)toStart) + ((float)toEnd * tVal));
    }

    /// <summary>
    /// Easing function with a sinusoidal smooth shape
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="tValue"></param>
    /// <returns></returns>
    public static float EaseSinusoidal(float a, float b, float tValue)
    {
        float equation = (Mathf.Sin(tValue * 1.57079f));
        float ret = a + ((b - a) * equation);
        return ret;
    }

    /// Computes a spring force of form (-scale*(d^degree))  where d = distance between the attractor and attractee. at degree = 1 it will be a linear spring
    /// </summary>
    /// <param name="attractor"></param>
    /// <param name="attractee"></param>
    /// <param name="stiffness"></param>
    /// <param name="degree"></param>
    /// <returns></returns>
    public static Vector3 CalculateSpringExponential(Vector3 attractor, Vector3 attractee, float stiffness, float degree = 1) // 
    {
        Vector3 displacement = attractor - attractee;
        float distance = displacement.magnitude;
        return displacement * stiffness * Mathf.Pow(distance, degree);
    }

    /// <summary>
    /// Computes falloff of form (scale/d^degree) where d = distance between the attractor and attractee. at degree = 2 this will follow law of r^2. If distance is less than epsilon, result will be a zero vector
    /// </summary>
    /// <param name="attractor"></param>
    /// <param name="attractee"></param>
    /// <param name="scale"></param>
    /// <param name="degree"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public static Vector3 CalculateMagnetismExponential(Vector3 attractor, Vector3 attractee, float scale, float degree = 2, float epsilon = Util.Epsilon)
    {
        Vector3 displacement = attractor - attractee;
        float distance = displacement.magnitude;
        if (distance < epsilon)
        {
            return Vector3.zero;
        }
        else
        {
            return (displacement * scale) / (1.0f + (Mathf.Pow(distance, degree)));
        }
    }
    
    /// <summary>
    /// Gets a layermask that collides with the provided layer ID
    /// </summary>
    /// <param name="againstLayer"></param>
    /// <returns></returns>
    public static LayerMask GetMask(int againstLayer)
    {
        LayerMask result = new LayerMask();
        for (int i = 0; i < 32; i++)
        {
            result = result ^ ((Physics.GetIgnoreLayerCollision(i, againstLayer) ? 0 : 1) << i);
        }
        return result;
    }
}
