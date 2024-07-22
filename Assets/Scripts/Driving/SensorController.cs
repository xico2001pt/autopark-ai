using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorController : MonoBehaviour {
    #region Fields
    [SerializeField] private Vector3 _directionOffset = new Vector3(0f, 0f, 0f);  // In degrees
    [SerializeField] private float _maxRayDistance = 6f;
    [SerializeField] private bool _drawDebugRay = true;
    [SerializeField] private Gradient _rayColor;
    
    private Vector3 _direction;
    private float _rayDistance;
    #endregion
    
    #region Unity Methods
    private void FixedUpdate() {
        _direction = Quaternion.Euler(_directionOffset) * transform.up;
        _direction.Normalize();
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _direction, out hit, _maxRayDistance)) {
            _rayDistance = hit.distance;
        } else {
            _rayDistance = _maxRayDistance;
        }
    }
    
    private void Update() {
        if (_drawDebugRay) {
            DrawDebugRay();
        }
    }
    #endregion
    
    #region Public Methods
    public float GetRayDistance() {
        return _rayDistance;
    }
    #endregion
    
    #region Private Methods
    private void DrawDebugRay() {
        Debug.DrawRay(transform.position, _direction * _rayDistance, _rayColor.Evaluate(_rayDistance / _maxRayDistance));
    }
    #endregion
}
