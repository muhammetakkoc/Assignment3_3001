using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New IFF Filter", menuName = "IFF")]
public class IFF_Filter : ScriptableObject
{
    [Header("If this array is empty, it means ANY")]
    public Team[] allowedTeams;

    [Header("If this array is empty, it means ANY")]
    public IFF_Tag[] allowedIFFResponses;

    [Header("If this array is empty, it means ANY")]
    public Domain_Tag[] allowedDomains;

    public static bool IsHostile(I_IFFChallengeable contact, Team myTeam)
    {
        return contact.IFF_GetResponse(myTeam) == IFF_Tag.Enemy;
    }

    /// <summary>
    /// Returns None if filtered out, returns the response otherwise
    /// </summary>
    /// <param name="contact"></param>
    /// <param name="myTeam"></param>
    /// <returns></returns>
    public IFF_Tag GetFilteredIFFResponse(I_IFFChallengeable contact, Team myTeam)
    {
        if (allowedTeams.Length > 0 && !allowedTeams.Contains(contact.Team))
        {
            return IFF_Tag.None;
        }

        IFF_Tag response = contact.IFF_GetResponse(myTeam);

        if (allowedIFFResponses.Length > 0 && !allowedIFFResponses.Contains(response))
        {
            return IFF_Tag.None;
        }

        return response;
    }

    public bool DoesContactPassFilter(I_IFFChallengeable contact, Team myTeam)
    {
        if (allowedTeams.Length > 0 && !allowedTeams.Contains(contact.Team))
        {
            return false;
        }

        if (allowedIFFResponses.Length > 0 && !allowedIFFResponses.Contains(contact.IFF_GetResponse(myTeam)))
        {
            return false;
        }

        if (allowedDomains.Length > 0 && !allowedDomains.Contains(contact.Domain))
        {
            return false;
        }

        return true;
    }

    public HashSet<Unit> GetFilteredUnitList(HashSet<Unit> UnitCollectionIn, Team myTeam)
    {
        HashSet<Unit> UnitCollection = new HashSet<Unit>();
        foreach (Unit unit in UnitCollectionIn)
        {
            if (DoesContactPassFilter(unit, myTeam))
            {
                UnitCollection.Add(unit);
            }
        }
        return UnitCollection;
    }

    public bool GetFilteredUnitListByTeam(ref HashSet<Unit> unitCollectionOut)
    {
        foreach (Unit unit in unitCollectionOut)
        {
            if (allowedTeams.Length > 0 && !allowedTeams.Contains(unit.Team))
            {
                unitCollectionOut.Remove(unit);
            }
        }
        return unitCollectionOut.Count > 0;
    }

    public bool GetFilteredUnitListByIFFResponse(ref HashSet<Unit> unitCollectionOut, Team myTeam)
    {
        foreach (Unit unit in unitCollectionOut)
        {
            if (allowedIFFResponses.Length > 0 && !allowedIFFResponses.Contains(unit.IFF_GetResponse(myTeam)))
            {
                unitCollectionOut.Remove(unit);
            }
        }
        return unitCollectionOut.Count > 0;
    }
}
