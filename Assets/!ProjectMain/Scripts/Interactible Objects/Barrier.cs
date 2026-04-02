using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private Collider2D _collision;
    private Renderer _renderer;

    private void Awake()
    {
        TryGetComponent<Collider2D>(out _collision);
        TryGetComponent<Renderer>(out _renderer);
    }

    #region Methods

    public void DeActivate()
    {
        StopAllCoroutines();
        StartCoroutine(DeActivate_Implementation());
    }

    public void Activate()
    {
        StopAllCoroutines();
        StartCoroutine(Activate_Implementation());
    }
    
    private IEnumerator DeActivate_Implementation()
    {
        yield return null;
        _collision.enabled = false;
        _renderer.enabled = false;
    }

    private IEnumerator Activate_Implementation()
    {
        yield return null;
        _collision.enabled = true;
        _renderer.enabled = true;
    }

    #endregion
}
