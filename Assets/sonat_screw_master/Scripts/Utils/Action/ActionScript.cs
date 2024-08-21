using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionScript : MonoBehaviour
{
    protected virtual void DoAction()
    {

    }

    [ContextMenu("DoAction")]
    public virtual void StartAction()
    {
        DoAction();
    }

    public virtual void StartAction(int i)
    {

    }

    public virtual void StopAction()
    {

    }
}