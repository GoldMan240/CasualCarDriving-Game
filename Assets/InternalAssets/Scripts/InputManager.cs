using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private CarController carController;
    [Space(20)]
    [SerializeField] private bool useTouchControls = false;
    [Space(10)]
    [SerializeField] private TouchButton moveForwardButton;
    [SerializeField] private TouchButton moveReverseButton;
    [Space(10)]
    [SerializeField] private TouchButton turnLeftButton;
    [SerializeField] private TouchButton turnRightButton;

    public event Action OnMoveForward;
    public event Action OnMoveReverse;
    public event Action OnTurnLeft;
    public event Action OnTurnRight;
    public event Action OnThrottleRelease;
    public event Action OnDeceleratingCar;
    public event Action OnSteeringRelease;
    public event Action AnimateWheels;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (useTouchControls)
        {
            if (moveForwardButton.IsPressed)
            {
                OnMoveForward?.Invoke();
            }

            if (moveReverseButton.IsPressed)
            {
                OnMoveReverse?.Invoke();
            }

            if (turnLeftButton.IsPressed)
            {
                OnTurnLeft?.Invoke();
            }

            if (turnRightButton.IsPressed)
            {
                OnTurnRight?.Invoke();
            }

            if (!moveForwardButton.IsPressed && !moveReverseButton.IsPressed)
            {
                OnThrottleRelease?.Invoke();
            }

            if (!moveForwardButton.IsPressed && !moveReverseButton.IsPressed && !carController.DeceleratingCar)
            {
                OnDeceleratingCar?.Invoke();
            }

            if (!turnLeftButton.IsPressed && !turnRightButton.IsPressed && carController.SteeringAxis != 0f)
            {
                OnSteeringRelease?.Invoke();
            }
        }
        else
        {
            DisableTouchButton(moveForwardButton);
            DisableTouchButton(moveReverseButton);
            DisableTouchButton(turnLeftButton);
            DisableTouchButton(turnRightButton);

            if (Input.GetKey(KeyCode.W))
            {
                OnMoveForward?.Invoke();
            }

            if (Input.GetKey(KeyCode.S))
            {
                OnMoveReverse?.Invoke();
            }

            if (Input.GetKey(KeyCode.A))
            {
                OnTurnLeft?.Invoke();
            }

            if (Input.GetKey(KeyCode.D))
            {
                OnTurnRight?.Invoke();
            }

            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
            {
                OnThrottleRelease?.Invoke();
            }

            if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !carController.DeceleratingCar)
            {
                OnDeceleratingCar?.Invoke();
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && carController.SteeringAxis != 0f)
            {
                OnSteeringRelease?.Invoke();
            }
        }
        
        AnimateWheels?.Invoke();
    }

    private void DisableTouchButton(TouchButton touchButton)
    {
        touchButton.gameObject.SetActive(false);
    }
}
