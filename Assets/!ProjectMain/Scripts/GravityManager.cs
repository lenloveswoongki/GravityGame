using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GravityManager
{
    [Header("Variables")]
    [SerializeField] private float _gravityForce;
    [SerializeField] private bool _bGravityIsInverted = false;

    public void Init()
    {
        GameManager.Instance.GetPlayerInputController.BindAxis(InputType.Accelerometer, UpdateGravity);
    }
    public void UpdateGravity(float horizontal, float vertical)
    {
        Physics2D.gravity = new Vector2(horizontal, vertical) * _gravityForce;
        if (_bGravityIsInverted)
        {
            Physics2D.gravity = -Physics2D.gravity;
        }
        Debug.Log($"GRAVITY: {Physics2D.gravity.x} {Physics2D.gravity.y}");

    }
}
