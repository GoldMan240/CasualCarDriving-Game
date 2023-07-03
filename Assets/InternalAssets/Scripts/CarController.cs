using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string DECELERATE_CAR = "DecelerateCar";

    [Space(20)]
    [Header("CAR SETUP")]
    [Space(10)]
    [Range(20, 190)]
    [SerializeField] private int maxSpeed = 90;
    [Range(10, 120)]
    [SerializeField] private int maxReverseSpeed = 45;
    [Range(1, 10)]
    [SerializeField] private int accelerationMultiplier = 2;
    [Space(10)]
    [Range(10, 45)]
    [SerializeField] private int maxSteeringAngle = 27;
    [Range(0.1f, 1f)]
    [SerializeField] private float steeringSpeed = 0.5f;
    [Space(10)]
    [Range(100, 650)]
    [SerializeField] private int brakeForce = 350;
    [Range(1, 10)]
    [SerializeField] private int decelerationMultiplier = 2;
    [Space(10)]
    [SerializeField] private Vector3 bodyMassCenter;

    [Space(20)]
    [Header("WHEELS")]
    [Space(10)]
    [SerializeField] private GameObject frontLeftMesh;
    [SerializeField] private WheelCollider frontLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject frontRightMesh;
    [SerializeField] private WheelCollider frontRightCollider;
    [Space(10)]
    [SerializeField] private GameObject rearLeftMesh;
    [SerializeField] private WheelCollider rearLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject rearRightMesh;
    [SerializeField] private WheelCollider rearRightCollider;

    [Space(20)]
    [Header("AUDIO")]
    [Space(10)]
    [SerializeField] private AudioSource carEngineSound;

    public bool DeceleratingCar { get; private set; }
    public float SteeringAxis { get; private set; }
    
    private Rigidbody carRigidbody;
    private float carSpeed;
    private float throttleAxis;
    private float localVelocityZ;
    private float initialCarEngineSoundPitch;

    private void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = bodyMassCenter;

        initialCarEngineSoundPitch = carEngineSound.pitch;

        InputManager.Instance.OnMoveForward += InputManager_OnMoveForward;
        InputManager.Instance.OnMoveReverse += InputManager_OnMoveReverse;
        InputManager.Instance.OnTurnLeft += InputManager_OnTurnLeft;
        InputManager.Instance.OnTurnRight += InputManager_OnTurnRight;
        InputManager.Instance.OnThrottleRelease += InputManager_OnThrottleOff;
        InputManager.Instance.OnDeceleratingCar += InputManager_OnDeceleratingCar;
        InputManager.Instance.OnSteeringRelease += InputManager_OnSteeringRelease;
        InputManager.Instance.AnimateWheels += InputManager_AnimateWheels;
    }

    // Œ¡–¿¡Œ“ ¿ —Œ¡€“»…

    private void InputManager_AnimateWheels()
    {
        AnimateWheelMeshes();
    }

    private void InputManager_OnSteeringRelease()
    {
        ResetSteeringAngle();
    }

    private void InputManager_OnDeceleratingCar()
    {
        InvokeRepeating(DECELERATE_CAR, 0f, 0.1f);
        DeceleratingCar = true;
    }

    private void InputManager_OnThrottleOff()
    {
        ThrottleOff();
    }

    private void InputManager_OnTurnRight()
    {
        TurnRight();
    }

    private void InputManager_OnTurnLeft()
    {
        TurnLeft();
    }

    private void InputManager_OnMoveReverse()
    {
        CancelInvoke(DECELERATE_CAR);
        DeceleratingCar = false;
        MoveReverse();
    }

    private void InputManager_OnMoveForward()
    {
        CancelInvoke(DECELERATE_CAR);
        DeceleratingCar = false;
        MoveForward();
    }

    // -------------------

    private void Update()
    {
        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;

        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
        carEngineSound.pitch = engineSoundPitch;
    }

    // ƒ¬»∆≈Õ»≈

    private void MoveForward()
    {
        bool isForward = true;
        BaseOfMove(isForward);
    }

    private void MoveReverse()
    {
        bool isForward = false;
        BaseOfMove(isForward);
    }

    private void BaseOfMove(bool isForward)
    {
        bool condition1;
        bool condition2;

        if (isForward)
        {
            throttleAxis = Mathf.Min(1, throttleAxis + (Time.deltaTime * 3f));

            condition1 = localVelocityZ < -1f;
            condition2 = Mathf.RoundToInt(carSpeed) < maxSpeed;
        }
        else
        {
            throttleAxis = Mathf.Max(-1, throttleAxis - (Time.deltaTime * 3f));

            condition1 = localVelocityZ > 1f;
            condition2 = Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed;
        }

        if (condition1)
        {
            Brakes();
        }
        else
        {
            if (condition2)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                ThrottleOff();
            }
        }
    }

    private void Brakes()
    {
        frontLeftCollider.brakeTorque = brakeForce;
        frontRightCollider.brakeTorque = brakeForce;
        rearLeftCollider.brakeTorque = brakeForce;
        rearRightCollider.brakeTorque = brakeForce;
    }

    private void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    private void DecelerateCar()
    {
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }

            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }

        carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));

        ThrottleOff();

        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke(DECELERATE_CAR);
        }
    }

    // œŒ¬Œ–Œ“

    private void TurnLeft()
    {
        bool isToRight = false;
        BaseOfTurn(isToRight);
    }

    private void TurnRight()
    {
        bool isToRight = true;
        BaseOfTurn(isToRight);
    }

    private void BaseOfTurn(bool isToRight)
    {
        float steeringSpeedNormolized = (Time.deltaTime * 10f * steeringSpeed);

        if (isToRight)
        {
            SteeringAxis = Mathf.Min(1f, SteeringAxis + steeringSpeedNormolized);
        }
        else
        {
            SteeringAxis = Mathf.Max(-1f, SteeringAxis - steeringSpeedNormolized);
        }

        float steeringAngle = SteeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    private void ResetSteeringAngle()
    {
        float steeringSpeedNormolized = (Time.deltaTime * 10f * steeringSpeed);

        if (SteeringAxis < 0f)
        {
            SteeringAxis += steeringSpeedNormolized;
        }
        else if (SteeringAxis > 0f)
        {
            SteeringAxis -= steeringSpeedNormolized;
        }

        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            SteeringAxis = 0f;
        }

        float steeringAngle = SteeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    // ¿Õ»Ã¿÷»ﬂ

    private void AnimateWheelMeshes()
    {
        try
        {
            AnimateWheelMesh(frontLeftMesh, frontLeftCollider);
            AnimateWheelMesh(frontRightMesh, frontRightCollider);
            AnimateWheelMesh(rearLeftMesh, rearLeftCollider);
            AnimateWheelMesh(rearRightMesh, rearRightCollider);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }

    private void AnimateWheelMesh(GameObject mesh, WheelCollider collider)
    {
        Quaternion wheelRotation;
        Vector3 wheelPosition;
        collider.GetWorldPose(out wheelPosition, out wheelRotation);
        mesh.transform.position = wheelPosition;
        mesh.transform.rotation = wheelRotation;
    }
}
