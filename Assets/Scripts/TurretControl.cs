using UnityEngine;

public class TurretControl : MonoBehaviour
{
    //[SerializeField]
    private float maxRotationSpeedDegrees = 600.0f;
    public float MaxRotationSpeedDegrees {  get { return maxRotationSpeedDegrees; } }

    [SerializeField]
    private float angularVelocity = 0;

    /// <summary>
    /// Attempts to rotate with the given requested angular velocity about the Y axis. If the requested velocity is too large, it will rotate at the max speed instead
    /// </summary>
    /// <param name="targetAngularVelocity"></param>
    public void SetDesiredAngularVelocity(float targetAngularVelocity)
    {
        angularVelocity = Mathf.Clamp(targetAngularVelocity, -maxRotationSpeedDegrees, maxRotationSpeedDegrees);
    }

    public void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, angularVelocity * Time.fixedDeltaTime, 0));
    }
}
