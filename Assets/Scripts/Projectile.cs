using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The one who shot this projectile
    /// </summary>
    Unit owner = null;

    private static float selfDestructFrequencyDelay = 0.3f;
    private float distanceTravelled = 0.0f;
    private float timeAlive = 0;

    [SerializeField]
    private Rigidbody body;
    public Rigidbody Rigidbody {  get { return body; } set { body = value; } }

    [SerializeField]
    private Collider myCollider;

    [SerializeField]
    private GameObject detonationPrefab;

    [SerializeField]
    private GameObject selfDestructPrefab;

    [SerializeField]
    private float maxRange;
    public float MaxRange { get { return maxRange; } private set {  maxRange = value; } }

    [SerializeField]
    private float maxLifetime;
    public float MaxLifeTime { get { return maxLifetime; } private set { maxLifetime = value; } }

    [SerializeField]
    private int directHitDamage;
    public int DirectHitDamage { get { return directHitDamage; } private set { directHitDamage = value; } }

    /// <summary>
    /// Set who owns this projectile
    /// </summary>
    /// <param name="myOwner"></param>
    public void SetOwner(Unit myOwner)
    {
        owner = myOwner;
    }

    void Start()
    {
        InvokeRepeating("SelfDestructCheck", selfDestructFrequencyDelay, selfDestructFrequencyDelay);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Unit killable = (Unit)collision.gameObject.GetComponent<I_Killable>();

        if (killable != null)
        {
            if (killable == owner) return; // do not hit self

            killable.DealDamage(directHitDamage); // hit valid target
            Detonate();
        }
        else
        {
            SelfDestruct(); // did not hit valid target
        }
    }


    void SelfDestructCheck()
    {
        distanceTravelled += selfDestructFrequencyDelay * body.velocity.magnitude;
        timeAlive += selfDestructFrequencyDelay;

        if (distanceTravelled > maxRange || timeAlive > maxLifetime)
        {
            {
                SelfDestruct();
            }
        }

    }

    public void Detonate()
    {
        if (detonationPrefab != null)
        {
            GameObject newObject = Instantiate(detonationPrefab, transform.position, transform.rotation);//Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up));
            Destroy(newObject, 30);
            Destroy(gameObject);
        }
    }

    public void SelfDestruct()
    {
        if(selfDestructPrefab != null)
        {
            GameObject newObject = Instantiate(selfDestructPrefab, transform.position, Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up));
            Destroy(newObject, 10);
        }
        Destroy(gameObject);
    }
}
