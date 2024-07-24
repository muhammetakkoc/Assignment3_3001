using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(I_IFFChallengeable))]
public class UnitTeamColorer : MonoBehaviour
{
    [SerializeField]
    Unit me;

    [SerializeField]
    List<MeshRenderer> renderers;

    static int unitID = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(!me)
        {
            me = GetComponent<Unit>();
        }
        me.name = unitID.ToString();
        me.GetUnitName();
        unitID++;
        UpdateColor();
    }

    public void UpdateColor()
    {
        UpdateColor(renderers, me.Team);
    }

    public static void UpdateColor(List<MeshRenderer> renderers, Team team)
    {
        Color color = Color.black;
        switch (team)
        {
            case Team.Blue:
                {
                    color = Color.blue;
                    break;
                }
            case Team.Red:
                {
                    color = Color.red;
                    break;
                }
            default:
                {
                    break;
                }
        }

        foreach(MeshRenderer renderer in renderers)
        {
            renderer.material.SetColor("_Color", color);
        }
    }
}
