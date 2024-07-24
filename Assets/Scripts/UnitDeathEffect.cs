using UnityEngine;

public class UnitDeathEffect : MonoBehaviour
{
    [SerializeField]
    GameObject deathPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Unit unit = GetComponent<Unit>();
        unit.eventOnDeath.AddListener(PlayEffect);
    }

    private void PlayEffect(Unit unit)
    {
        GameObject newObject = Instantiate(deathPrefab, unit.transform.position, unit.transform.rotation);
        newObject.GetComponent<Rigidbody>().AddExplosionForce(100000, unit.transform.position - Vector3.up + Random.insideUnitSphere, 20);
        Destroy(newObject, 10);
    }
}
