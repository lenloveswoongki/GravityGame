using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour, IInteractible
{
    [SerializeField] private bool bCanBeTriggeredMultipleTimes = false;
    private bool _bHasBeenTriggered = false;
    private Action OnTrigger;
    public InteractionType interactionType { get; }

    public void Suscribe(Action action)
    {
        OnTrigger += action;
    }
    public void DeSuscribe(Action action)
    {
        OnTrigger -= action;
    }

    protected void Trigger()
    {
        if (bCanBeTriggeredMultipleTimes)
        {
            OnTrigger?.Invoke();
            _bHasBeenTriggered = true;
        }
        else
        {
            if(!_bHasBeenTriggered)
            {
                OnTrigger?.Invoke();
                _bHasBeenTriggered = true;
            }
        }
    }
    public virtual void Interact(Character triggerReference)
    {
        Trigger();
    }
}
