using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputController", menuName = "Input/PlayerInputController", order = 1)]
public class PlayerInputController : ScriptableObject
{
    [SerializeField] private TouchInputComponent _touchInputComponent;
    [SerializeField] private AccelerometerComponent _accelerometerComponent;
    public void Init()
    {
        _touchInputComponent.Init();
        _accelerometerComponent.Init();
    }
    public void BindAction(TapType inputType, Action<Touch> outMethod)
    {
        _touchInputComponent.Suscribe(inputType, outMethod);
    }        
    public void BindAxis(InputType inInputType, Action<float, float> inMethod)
    {
        switch(inInputType)
        {
            case InputType.Accelerometer:
                _accelerometerComponent.Suscribe(inMethod); break;
        }
    }

    public void Update()
    {
        _touchInputComponent.Update();      
    }
    public void FixedUpdate()
    {
        _accelerometerComponent.FixedUpdate();
    }
}

[System.Serializable]
public class TouchInputComponent
{
    private Action<Touch> singleTap;
    private Action<Touch> doubleTap;
    private Action<Touch> holdTap;

    [SerializeField] private float _touchThreeshold;
    private bool _bIsCheckingForDoubleTouch = false;
    private byte _touchCount = 0;
    private Touch _firstTap;

    public void Init()
    {
        singleTap = null;
        doubleTap = null;
        holdTap = null;
    }
    public void Update()
    {
        byte touchCount = (byte)Input.touchCount;
        
        switch(touchCount)
        {
            case 0:
                return;
            case 1:
                SingleTouch();
                break;
            case > 1:
                CheckForMultiTouch();
                break;
        }
    }

    public void Suscribe(TapType inputType, Action<Touch> outMethod)
    {
        switch (inputType)
        {
            case TapType.SingleTap:
                singleTap += outMethod;
                break;
            case TapType.DoubleTap:
                doubleTap += outMethod;
                break;
            case TapType.TapAndHold:
                holdTap += outMethod;
                break;
        }
    }
    public void Unscribe(TapType inputType, Action<Touch> outMethod)
    {
        switch (inputType)
        {
            case TapType.SingleTap:
                singleTap -= outMethod;
                break;
            case TapType.DoubleTap:
                doubleTap -= outMethod;
                break;
            case TapType.TapAndHold:
                holdTap -= outMethod;
                break;
        }
    }
    public void SingleTouch()
    {
        Touch t = Input.GetTouch(0);
        if(t.phase == TouchPhase.Began) 
        {
            _touchCount++;
            if (!_bIsCheckingForDoubleTouch)
            {
                _touchCount = 0;
                _firstTap = t;
                _bIsCheckingForDoubleTouch = true;
                GameManager.Instance.SetTimer(_touchThreeshold, CheckForDoubleTap);
            }
        }       
    }

    public void CheckForDoubleTap()
    {
        if (_touchCount == 1)
        {
            singleTap?.Invoke(_firstTap);
        }
        else if (_touchCount > 1) {
            doubleTap?.Invoke(_firstTap);
        }
        _touchCount = 0;
        _bIsCheckingForDoubleTouch = false;
    }
    public void CheckForMultiTouch()
    {

    }
}


[System.Serializable]
public class AccelerometerComponent
{
    private Action<float, float> accelerometerDelegate;

    public void Init()
    {
        Input.gyro.enabled = false;
        Input.compensateSensors = true;
        accelerometerDelegate = null;
    }

    public void Suscribe(Action<float, float> inMethod)
    {
        accelerometerDelegate += inMethod;
    }

    public void UnSuscribe(Action<float, float> inMethod)
    {
        accelerometerDelegate -= inMethod;
    }
    public void FixedUpdate()
    {
        #if UNITY_EDITOR
        accelerometerDelegate?.Invoke(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Debug.Log($"ACCEL: {Input.GetAxis("Horizontal")} {Input.GetAxis("Vertical")}");
        return;
        #endif
        
        Vector3 accelerometerInput = Input.acceleration;
        accelerometerDelegate?.Invoke(accelerometerInput.x, accelerometerInput.y);
  
    }
}