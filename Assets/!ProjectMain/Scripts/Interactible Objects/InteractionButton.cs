using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionButton : MonoBehaviour, IInteractible
{
    [Header("Button")] 
    [SerializeField] private UnityEvent[] onInteractionEvents;
    [SerializeField] private EventsExecutionType eventsExecutionType;
    [SerializeField] public InteractionType interactionType { get; }
    public void Interact(Character triggerReference)
    {
        switch (eventsExecutionType)
        {
            case EventsExecutionType.AllAtTheSameTime :
                foreach (UnityEvent index in onInteractionEvents)
                {
                    index?.Invoke();
                }

                break;
        }
    }
}
