using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Unit : MonoBehaviour, I_Killable, I_IFFChallengeable
{
    [Header("IFF")]

    [SerializeField]
    new public Rigidbody rigidbody = null;

    [SerializeField]
    new public Collider collider = null;

    [SerializeField]
    string unitName = "UNKNOWN";

    [SerializeField]
    private Domain_Tag domain = Domain_Tag.None;
    public Domain_Tag Domain { get { return domain; } set { domain = value; } }
    
    [SerializeField]
    private Team team = Team.None;
    public Team Team { get { return team; } set { team = value;} }

    [Header("Killable")]
    public bool IsAlive = true;

    [SerializeField]
    private int hitPointsMax = 10;
    public int HitPointsMax { get { return hitPointsMax; } set { hitPointsMax = value; } }

    //[SerializeField]
    private int hitPoints = 10;
    public int HitPoints { get { return hitPoints; } private set { hitPoints = value; } }

    [Header("Events")]
    //Triggered once when killed
    [SerializeField]
    public UnityEvent<Unit> eventOnDeath;

    //Triggered every time when damaged but not killed
    [SerializeField]
    public UnityEvent<Unit> eventOnHit;

    void Awake()
    {
        hitPoints = hitPointsMax;
        IsAlive = true;
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody>();  
        }
        if(collider == null)
        {
            collider = GetComponent<Collider>();    
        }
    }

    public float GetHealthPercentage()
    {
        return hitPoints / (float)hitPointsMax;
    }

    public float GetHealth()
    {
        return hitPoints;
    }

    public void DealDamage(int damage)
    {
        hitPoints -= damage;

        if (IsAlive)
        {
            if (hitPoints <= 0)
            {
                Kill();
            }
            else
            {
                eventOnHit.Invoke(this);
            }
        }
    }

    public void Kill()
    {
        HitPoints = 0;
        IsAlive = false;
        eventOnDeath.Invoke(this);
        Destroy(gameObject);
    }

    public IFF_Tag IFF_GetResponse(Team whoIsAsking)
    {
        return I_IFFChallengeable.GetDefaultAlignment(Team)[(int)whoIsAsking];
    }

    public string GetUnitName()
    {
        return unitName;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Team = team;
    }
#endif
}
