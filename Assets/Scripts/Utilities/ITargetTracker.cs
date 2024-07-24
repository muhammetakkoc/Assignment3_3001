using System;

public interface ITargetTracker
{
    public bool AddTarget(Unit newTarget);
    public bool RemoveTarget(Unit existingTarget);
}
