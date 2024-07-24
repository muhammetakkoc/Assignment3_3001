using System;
using System.Collections.Generic;
using UnityEngine;

//struct TargetLock
//{
//    public Unit target;
//    public float strength;
//}

public class TargetingSystem : MonoBehaviour
{
    [SerializeField]
    public TargetAcquirer targetAcquirer;

    //[SerializeField]
    //protected HashSet<Unit> locks = new HashSet<Unit>();

    [SerializeField]
    protected List<Unit> currentTargets = null;
    //public Unit TargetUnit { get { return currentTarget; } private set { currentTarget = value; } }

    public Action<Unit> onTargetSelect;
    public Action<Unit> onTargetDeselect;

    public void Awake()
    {
        targetAcquirer.onTargetAcquiredEvent += AddTarget;
        targetAcquirer.OnTargetLostEvent += RemoveTarget;
    }

    public void OnDestroy()
    {
        targetAcquirer.onTargetAcquiredEvent -= AddTarget;
        targetAcquirer.OnTargetLostEvent -= RemoveTarget;
    }

    public void OnTargetKilledListener(Unit targetKilled)
    {
        RemoveTarget(targetKilled);
        //  if (targetKilled == currentTargets)
        {
            DeselectTarget(targetKilled);
        }
    }

    public Unit GetFirstSelectedTarget()
    {
        return currentTargets[0];
    }

    public List<Unit> GetSelectedTargets()
    {
        return currentTargets;
    }

    public bool SelectTargets(IEnumerable<Unit> targets)
    {
        DeselectTargets();
        foreach (Unit target in targets)
        {
            AddSelection(target);
        }
        return true;
    }

    public bool AddSelection(Unit target)
    {
        bool success;

        if (target != null)
        {
            if (onTargetSelect != null)
            {
                onTargetSelect.Invoke(target);
            }

            success = true;
            currentTargets.Add(target);
        }
        else
        {
            success = false;
        }

        return success;
    }

    public bool SelectTarget(Unit target)
    {
        bool success;

        if (target != null)
        {
            if(currentTargets.Count == 1 && target == currentTargets[0])
            {
                return false;
            }

            DeselectTargets();

            if (onTargetSelect != null)
            {
                onTargetSelect.Invoke(target);
            }

            success = true;
            currentTargets.Add(target);

        }
        else
        {
            success = false;
            DeselectTargets();
        }

        return success;
    }

    public bool SelectNearestInCone(float coneHalfAngle)
    {
        Ray headingRay = new Ray(targetAcquirer.transform.position, targetAcquirer.transform.forward);
        Unit newTarget = targetAcquirer.GetNearestToRayByAngle(headingRay, coneHalfAngle);
        return SelectTarget(newTarget);
    }

    public bool SelectNearest(IFF_Filter filter = null)
    {
        return SelectTarget(targetAcquirer.GetNearestToPosition(targetAcquirer.transform.position, filter));
    }

    public void DeselectTargets()
    {
        if (onTargetDeselect != null)
        {
            for (int i = currentTargets.Count - 1; i >= 0; i--)
            {
                onTargetDeselect.Invoke(currentTargets[i]);
            }
        }

        currentTargets.Clear();
    }

    public void DeselectTarget(Unit target)
    {
        if (currentTargets.Contains(target))
        {
            if (onTargetDeselect != null)
            {
                onTargetDeselect.Invoke(target);
            }
        }

        currentTargets.Remove(target);
    }

    public bool AddTarget(Unit newTarget)
    {
        newTarget.eventOnDeath.AddListener(OnTargetKilledListener);
        return true;
    }

    public bool RemoveTarget(Unit existingTarget)
    {
        existingTarget.eventOnDeath.RemoveListener(OnTargetKilledListener);
        DeselectTarget(existingTarget);

        return true;
    }
}
