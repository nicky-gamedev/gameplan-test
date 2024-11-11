using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputListener : Singleton<InputListener>
{
    public bool detectShakes = false;

    public Joystick joystick;
    public ButtonReferenceContainer buttons;
    public Text shakeDebug;

    public event Action<float> OnMovement = delegate {};
    public event Action OnJump = delegate {};
    public event Action OnShake = delegate {};

    [Header("Shake Settings")]
    public float accelerometerUpdateInterval = 1.0f / 60.0f;
    public float lowPassKernelWidthInSeconds = 1.0f;
    public float shakeDetectionThreshold = 2.0f;
    public float minTimeBetweenShakes = 1f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;
    float nextShakeTime = 0f;

    void Start()
    {
        joystick = GetComponentInChildren<Joystick>();
        buttons = GetComponentInChildren<ButtonReferenceContainer>();

        buttons.SubscribeToEvent("Jump", OnJumpButtonPressed);

        if(detectShakes) InitializeShakeDetection();
    }

    private void OnJumpButtonPressed()
    {
        OnJump.Invoke();
    }

    void Update()
    {
        OnMovement.Invoke(joystick.Horizontal);

        if(detectShakes) CheckForShake();
    }

    void InitializeShakeDetection()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    void CheckForShake()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold && nextShakeTime <= Time.unscaledTime)
        {
            shakeDebug.text = $"Shaking {deltaAcceleration.sqrMagnitude}, threshold {shakeDetectionThreshold}";
            OnShake.Invoke();
            nextShakeTime = Time.unscaledTime + minTimeBetweenShakes;
        }
        else
        {
            shakeDebug.text = $"Not Shaking {deltaAcceleration.sqrMagnitude}, threshold {shakeDetectionThreshold}";
        }
    }
}
