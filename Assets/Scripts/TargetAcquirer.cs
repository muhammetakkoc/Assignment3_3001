using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetAcquirer : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Used for IFF signals, if null, will target everything")]
    public Unit myUnitIFFSource = null;

    public IFF_Filter IFF_Filter = null;

    HashSet<Unit> contacts = new HashSet<Unit>();

    public IReadOnlyCollection<Unit> Contacts { get { return contacts;} private set { contacts = (HashSet<Unit>) value; } }

    public delegate bool OnTargetAcquiredDelegate(Unit target);
    public event OnTargetAcquiredDelegate onTargetAcquiredEvent;

    public delegate bool OnTargetLostDelegate(Unit target);
    public event OnTargetLostDelegate OnTargetLostEvent;

    [SerializeField]
    Collider myCollider;
    // public delegate bool OnContactAcquiredDelegate(Unit target);
    // public event OnTargetAcquiredDelegate onTargetAcquiredEvent;

    private bool TrackingEnabled = true;

    public void Start()
    {
        myCollider = GetComponent<Collider>();
    }

    public bool GetTrackingEnabled()
    {
        return TrackingEnabled;
    }

    public void SetTrackingEnabled(bool enabled)
    {
        TrackingEnabled = enabled;
        if (TrackingEnabled)
        {
            myCollider.enabled = true;
        } else
        {
            DeregisterAllContacts();
            myCollider.enabled = false;
        }
    }

    public void EnableTracking()
    {
        TrackingEnabled = true;
        myCollider.enabled = true;
    }

    public void DisableTracking()
    {
        DeregisterAllContacts();
        TrackingEnabled = false;
        myCollider.enabled = false;
    }

    public HashSet<Unit> GetCopyOfContactsList()
    {
        return new HashSet<Unit>(contacts);
    }

    public void RegisterContact(Unit contact)
    {
        if (contact == null)
        {
            return;
        }
        contacts.Add(contact);

        if (myUnitIFFSource != null)
        {
            if(IFF_Filter != null)
            {
                if(!IFF_Filter.DoesContactPassFilter(contact, myUnitIFFSource.Team))
                {
                    return;
                }
            }
        }

        if(onTargetAcquiredEvent != null)
        {
            onTargetAcquiredEvent.Invoke(contact);
        }
    }

    public void DeregisterAllContacts()
    {
        foreach (Unit contact in Contacts)
        {
            DeregisterContact(contact);
        }
    }

    public void DeregisterContact(Unit contact)
    {
        if (contact == null)
        {
            return;
        }

        if(contacts.Contains(contact))
        {
            if (OnTargetLostEvent != null)
                OnTargetLostEvent.Invoke(contact);

            contacts.Remove(contact);
        }
    }


    /// <summary>
    /// Gets the known target closest by linear distance to the ray which is in front of the ray, and within distanceTolerance units from it.
    /// Null if nothing was found
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="distanceTolerance"></param>
    /// <returns></returns>
    public Unit GetNearestToRayByDistance(Ray ray, float distanceTolerance = float.PositiveInfinity, IFF_Filter filter = null)
    {
        Unit nearest = null;
        float nearestDistanceSq = float.PositiveInfinity;

        ICollection<Unit> list;
        if (filter == null)
        {
            list = contacts;
        }
        else
        {
            list = filter.GetFilteredUnitList(contacts, myUnitIFFSource.Team);
        }

        foreach (Unit contact in list)
        {
            if (contact == null )
            {
                continue;
            }

            Vector3 contactPos = contact.transform.position;
            Vector3 contactDisp = contactPos - ray.origin;
            float dot = Vector3.Dot(contactDisp, ray.direction);

            if (dot < 0)
            {
                //Target is behind ray
                continue;
            }

            Vector3 projected = contactDisp - dot * ray.direction;
            float distance = projected.sqrMagnitude;
            if(distance < nearestDistanceSq && distance < distanceTolerance)
            {
                nearest = contact;
                nearestDistanceSq = distance;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Returns whether or not this TargetAcquirer knows about and can see the given contact
    /// </summary>
    /// <param name="contact"></param>
    /// <param name="fov"></param>
    /// <param name="visibleMask"></param>
    /// <returns></returns>
    public bool CanSee(Unit contact, VisionCone fov, LayerMask visibleMask = new LayerMask(), IFF_Filter filter = null)
    {
        if (filter != null)
        {
            if(!filter.DoesContactPassFilter(contact, myUnitIFFSource.Team))
            {
                return false;
            }
        }

        if (!Contacts.Contains(contact))
        {
            return false;
        }

        Ray boreSightRay = new Ray(transform.position, transform.forward);
        Vector3 contactPos = contact.transform.position;

        return fov.IsTargetWithinCone(boreSightRay, contactPos) && HasLOS(contact, fov, visibleMask);
    }

    /// <summary>
    /// Returns whether or not there is a visible line from this TargetAcquirer to the contact, regardless of the vision cone or known contact list
    /// </summary>
    /// <param name="contact"></param>
    /// <param name="ray"></param>
    /// <param name="fov"></param>
    /// <param name="visibleMask"></param>
    /// <returns></returns>
    public bool HasLOS(Unit contact, VisionCone fov, LayerMask visibleMask)
    {
        Ray ray = new Ray(transform.position, contact.transform.position - transform.position);
        RaycastHit hitInfo;
        bool didHit = Physics.Raycast(ray, out hitInfo, fov.range, visibleMask);
        if (didHit)
        {
            if (hitInfo.rigidbody == contact.rigidbody)
            {
                //Direct LOS to target!
                return true;
            }
            else
            {
                //Hit something else in the way!
                return false;
            }
        }
        //If we did not hit anything, it might mean the contact is invisible or the VisibleMask may not be set correctly.
        return false;
    }

    /// <summary>
    /// Find a target given visibility
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="coneHalfAngle"></param>
    /// <param name="maxRange"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public Unit GetNearestVisibleByAngle(VisionCone fov, LayerMask visibleMask, IFF_Filter filter = null)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Unit nearest = null;
        float nearestAngle = float.PositiveInfinity;

        ICollection<Unit> list;
        if (filter == null)
        {
            list = contacts;
        }
        else
        {
            list = filter.GetFilteredUnitList(contacts, myUnitIFFSource.Team);
        }

        foreach (Unit contact in list)
        {
            if (contact == null) continue;

            Vector3 contactPos = contact.transform.position;
            Vector3 contactDisp = contactPos - ray.origin;

            float distance = contactDisp.magnitude;
            if (distance > fov.range)
            {
                //Target is outside our concern
                continue;
            }

            Vector3 contactDir = contactDisp / distance;
            float angle = Vector3.Angle(ray.direction, contactDir);

            if (angle > fov.halfAngle)
            {
                //Target is outside our concern
                continue;
            }

            if(!HasLOS(contact, fov, visibleMask))
            {
                // No Line of sight
                continue;
            }

            if (angle < nearestAngle)
            {
                nearest = contact;
                nearestAngle = angle;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Returns known Unit with the smallest angle that is within coneHalfAngle in front of the ray. 
    /// Null if none found
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="coneHalfAngle"></param>
    /// <returns></returns>
    public Unit GetNearestToRayByAngle(Ray ray, float coneHalfAngle = 180.0f, float maxRange = float.PositiveInfinity, IFF_Filter filter = null)
    {
        Unit nearest = null;
        float nearestAngle = float.PositiveInfinity;

        ICollection<Unit> list;
        if(filter == null )
        { 
            list = contacts;
        } else
        {
            list = filter.GetFilteredUnitList(contacts, myUnitIFFSource.Team);
        }

        foreach (Unit contact in list)
        {
            if (contact == null) continue;

            Vector3 contactPos = contact.transform.position;
            Vector3 contactDisp = contactPos - ray.origin;

            float distance = contactDisp.magnitude;
            if (distance > maxRange)
            {
                //Target is outside our concern
                continue;
            }

            Vector3 contactDir = contactDisp / distance;
            float angle = Vector3.Angle(ray.direction, contactDir);

            if(angle > coneHalfAngle)
            {
                //Target is outside our concern
                continue;
            }

            if (angle < nearestAngle)
            {
                nearest = contact;
                nearestAngle = angle;
            }
        }

        return nearest;
    }

    /// <summary>
    /// Returns known Unit closes to the given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Unit GetNearestToPosition(Vector3 position, IFF_Filter filter = null)
    {
        ICollection<Unit> list;
        if (filter == null)
        {
            list = contacts;
        }
        else
        {
            list = IFF_Filter.GetFilteredUnitList(contacts, myUnitIFFSource.Team);
        }

        Unit newTarget = null;
        float closestDistanceSquared = float.PositiveInfinity;
        foreach (Unit killable in list)
        {
            Vector3 displacement = killable.transform.position - position;
            if (displacement.sqrMagnitude < closestDistanceSquared)
            {
                //new closest
                newTarget = killable;
                closestDistanceSquared = displacement.sqrMagnitude;
            }
        }

        return newTarget;
    }


    private void OnTriggerEnter(Collider other)
    {
        Unit contact = other.GetComponent<Unit>();
        if (contact != null && contact.IsAlive && TrackingEnabled)
        {
            contact.eventOnDeath.AddListener(DeregisterContact);
            RegisterContact(contact);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit contact = other.GetComponent<Unit>();
        if (contact != null)
        {
            contact.eventOnDeath.RemoveListener(DeregisterContact);
            DeregisterContact(contact);
        }
    }
}
