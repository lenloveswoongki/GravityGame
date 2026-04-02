using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractible
{
    public InteractionType interactionType { get; }
    public void Interact(Character triggerReference);
}

