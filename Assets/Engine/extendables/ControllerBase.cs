using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllerBase
{
    public virtual void onUpdate(PawnBase obj)
    {

    }

    public virtual void onRemoveFromPawn(PawnBase obj)
    {

    }

    public virtual void onAddToPawn(PawnBase obj)
    {

    }

    public virtual void onFixedUpdate(PawnBase obj)
    {

    }

    public virtual void onDrawGizmos(PawnBase pawnBase)
    {
        throw new NotImplementedException();
    }
}
