using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableCat : Character
{
    public override void Kill()
    {
        GameManager.Instance.RestartLevel();
        base.Kill();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<IInteractible>() != null)
        {
            other.gameObject.GetComponent<IInteractible>().Interact(this);
        }
    }
}

