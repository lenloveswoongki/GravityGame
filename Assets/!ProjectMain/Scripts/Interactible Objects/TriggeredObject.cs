using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredObject : MonoBehaviour
{
    [SerializeField] private TriggerObject[] _triggers;
    [SerializeField] private TriggerType triggerType;
    private byte _triggersAmount;
    private byte _triggersCounter = 0;
    public void Start()
    {
        if(triggerType == TriggerType.AllTriggersToExecute)
        {
            _triggersAmount = (byte)_triggers.Length;
        }
        
        foreach(TriggerObject trigger in _triggers)
        {
            trigger.Suscribe(Execute);
        }
    }

    private void Execute()
    {
        if (triggerType == TriggerType.AllTriggersToExecute)
        {
            _triggersCounter++;
            if (_triggersCounter == _triggersAmount) 
            { 
                Execute_Implementation();
            }
        }
        else if (triggerType != TriggerType.EachTriggerExecutes) 
        {            
            Execute_Implementation();
        }
    }

    protected virtual void Execute_Implementation()
    {

    }
}

