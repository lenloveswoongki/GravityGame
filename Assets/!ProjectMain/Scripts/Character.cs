using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IKillable
{
    [SerializeField] private bool bIsPlayer;
    public bool IsPlayer() { return bIsPlayer; }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        IInteractible targetObject = collision.gameObject.GetComponent<IInteractible>();
        if (targetObject != null)
        {
            targetObject.Interact(this);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractible targetObject = collision.gameObject.GetComponent<IInteractible>();
        if (targetObject != null)
        {
            targetObject.Interact(this);
        }
    }
    public virtual void Kill()
    {
        this.gameObject.SetActive(false);
    }
}
