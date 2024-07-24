using UnityEngine;

public class WorldspaceMarker : MonoBehaviour
{
    [SerializeField]
    public RadarMarkerUI marker;

    [SerializeField]
    public PlayerUIConfig playerUIConfig;

    [SerializeField]
    Canvas markerCanvas;

 //   [SerializeField]
    Camera mainCamera;

    public void Awake()
    {
       // if(!markerCanvas)
       // {
       //     markerCanvas = FindFirstObjectByType<Canvas>();
       // }

        marker = Instantiate(playerUIConfig.targetMarkerPrefab, markerCanvas.transform).GetComponent<RadarMarkerUI>();
        mainCamera = Camera.main;
    }

    public void OnDestroy()
    {
        if(marker != null)
        {
            Destroy(marker.gameObject);
        }
    }

    public void OnDisable()
    {
        if (marker)
        {
            marker.gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        if(marker)
        {
            marker.gameObject.SetActive(true);
        }
    }

    public void LateUpdate()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        if(screenPos.z < 1 )
        {
            //marker.transform.position = screenPos;
            //  marker.gameObject.SetActive(false);
            marker.transform.localScale = Vector3.zero;
        } else
        {
            marker.transform.localScale = Vector3.one;
            // marker.gameObject.SetActive(true);
            screenPos.z = 0;
            marker.transform.position = screenPos;
        }
    }
}
