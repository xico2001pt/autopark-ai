using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {
    #region Fields
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private VehicleWheel[] _wheels;
    [SerializeField] private AnimationCurve _torqueCurve;
    
    [Header("Settings")]
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _maxMotorTorque = 100f;
    [SerializeField] private float _maxBrakeTorque = 100f;
    [SerializeField] private float _maxSteeringAngle = 35f;
    
    [Header("Input")]
    [SerializeField, Range(-1f, 1f)] private float _throttle;
    [SerializeField, Range(-1f, 1f)] private float _steering;
    #endregion
    
    #region Unity Methods
    protected void FixedUpdate() {
        ApplyMotor();
        ApplyBrake();
        ApplySteering();
        UpdateWheelPoses();
        
        if (_rigidbody.velocity.magnitude > _maxSpeed) {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;
        }
    }
    #endregion
    
    #region Public Methods
    public void SetThrottle(float throttle) {
        _throttle = Mathf.Clamp(throttle, -1f, 1f);
    }
    
    public void SetSteering(float steering) {
        _steering = Mathf.Clamp(steering, -1f, 1f);
    }
    #endregion
    
    #region Private Methods
    private void ApplyMotor() {
        float torque = GetTorque();
        foreach (VehicleWheel wheel in _wheels) {
            if (wheel.isTraction) {
                wheel.collider.motorTorque = _throttle * _maxMotorTorque * torque;
            }
        }
    }
    
    private float GetTorque() {
        return _torqueCurve.Evaluate(_rigidbody.velocity.magnitude / _maxSpeed);
    }
    
    private void ApplyBrake() {
        float brake = 0f;
        float movingDirection = Vector3.Dot(transform.forward, _rigidbody.velocity);
        if (movingDirection < -0.5f && _throttle > 0f) {
            brake = Mathf.Abs(_throttle);
            _throttle = 0f;
        } else if (movingDirection > 0.5f && _throttle < 0f) {
            brake = Mathf.Abs(_throttle);
            _throttle = 0f;
        }
        
        foreach (VehicleWheel wheel in _wheels) {
            if (wheel.isBraking) {
                wheel.collider.brakeTorque = brake * _maxBrakeTorque;
            }
        }
    }
    
    private void ApplySteering() {
        foreach (VehicleWheel wheel in _wheels) {
            if (wheel.isSteering) {
                wheel.collider.steerAngle = _steering * _maxSteeringAngle;
            }
        }
    }
    
    private void UpdateWheelPoses() {
        foreach (VehicleWheel wheel in _wheels) {
            Vector3 position;
            Quaternion rotation;
            wheel.collider.GetWorldPose(out position, out rotation);
            wheel.mesh.transform.position = position;
            wheel.mesh.transform.rotation = rotation;
        }
    }
    #endregion
}

[System.Serializable]
public struct VehicleWheel {
    public WheelCollider collider;
    public MeshRenderer mesh;
    public bool isSteering;
    public bool isTraction;
    public bool isBraking;
}