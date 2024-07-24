using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    Unit owner; // the unit that owns this launcher

    private Rigidbody rb;

    [Header("Shooting")]
    protected float timeBetweenShots = 2.0f;
    public float TimeBetweenShots { get { return timeBetweenShots; } private set { timeBetweenShots = value; } }
    public float ShotsPerSecond { get { return 1.0f / timeBetweenShots; } set { timeBetweenShots = 1.0f / value; } }

    private bool isTriggerDown = false;
    public bool IsTriggerDown { get { return isTriggerDown; } protected set { isTriggerDown = value; } }

    private float muzzleSpeed = 30;
    public float MuzzleSpeed {  get { return muzzleSpeed; } protected set { muzzleSpeed = value; } }

    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Audio")]
    [SerializeField]
    protected AudioSource firingAudioSource;

    private float unfiredTriggerTime = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void BeginTriggerPull()
    {
        if(!IsTriggerDown)
        {
            IsTriggerDown = true;
            if (firingAudioSource.loop)
            {
                firingAudioSource.Play();
            }
        }
    }

    public void CeaseTriggerPull()
    {
        if (IsTriggerDown)
        {
            IsTriggerDown = false;
            if (firingAudioSource.loop)
            {
                firingAudioSource.Stop();
            }
        }
    }

    protected void FixedUpdate()
    {
        if (IsTriggerDown)
        {
            unfiredTriggerTime += Time.fixedDeltaTime;

            if(unfiredTriggerTime > timeBetweenShots )
            {
                Shoot();
                unfiredTriggerTime -= timeBetweenShots;
            }
        } else
        {
            unfiredTriggerTime = Mathf.Min(unfiredTriggerTime + Time.fixedDeltaTime, timeBetweenShots);
        }
    }

    private void Shoot()
    {

        Vector3 initialVelocity = (transform.forward * muzzleSpeed) + owner.rigidbody.velocity;
        GameObject newObject = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(initialVelocity));
        Projectile projectile = newObject.GetComponent<Projectile>();
        projectile.SetOwner(owner);
        projectile.Rigidbody.velocity = initialVelocity;

        firingAudioSource.pitch = Random.Range(0.90f, 1.1f);
        firingAudioSource.Play();

        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), owner.collider, true); //Do not let projectile collide with owner!
    }
}
