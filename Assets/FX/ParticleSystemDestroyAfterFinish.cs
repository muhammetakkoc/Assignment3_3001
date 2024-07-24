using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemDestroyAfterFinish : MonoBehaviour
{
    [SerializeField]
    float minimumTime;

    [SerializeField]
    ParticleSystem[] waitForFinish;

    IEnumerator KillSystem(float minimumTime)
    {
        yield return new WaitForSeconds(minimumTime);

        while(true)
        {
            bool canKill = true;
            foreach(ParticleSystem system in waitForFinish)
            {
                if(system.isPlaying)
                {
                    canKill = false;
                    break;
                }
            }

            if(canKill)
            {
                Destroy(transform.gameObject);
            }

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(KillSystem(minimumTime));
    }
}
