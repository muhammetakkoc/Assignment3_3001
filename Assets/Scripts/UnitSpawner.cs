using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnPeriod = 10;

    [SerializeField]
    GameObject prefab = null;

    [SerializeField]
    Team team;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(spawnPeriod >= 0)
        {
            yield return new WaitForSeconds(spawnPeriod);
            Collider[] collider = Physics.OverlapSphere(transform.position, 8, LayerMask.GetMask("Unit"));
            if (collider.Length == 0)
            {
                GameObject unitObj = Instantiate(prefab, transform.position, transform.rotation);
                unitObj.GetComponent<Unit>().Team = team;
            }
        }
    }
}
