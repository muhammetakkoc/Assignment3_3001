using UnityEngine;
using System.Collections;

public class csLightControl : MonoBehaviour
{

    [SerializeField]
    public Light _light;
    float _time = 0;

    float lightIntensity;
    public float FadeTime = 0.25f;

    void Start()
    {
        lightIntensity = _light.intensity;
        _time = FadeTime;
    }

    void Update()
    {
        _time -= Time.deltaTime;

        if (_light.intensity > 0)
            _light.intensity = Mathf.Lerp(0, lightIntensity, _time);

        if (_light.intensity <= 0)
            _light.intensity = 0;
    }
}
