using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    #region Fields
    [SerializeField] private VehicleController _vehicleController;
    #endregion
    
    #region Unity Methods
    protected void Update() {
        _vehicleController.SetThrottle(Input.GetAxis("Vertical"));
        _vehicleController.SetSteering(Input.GetAxis("Horizontal"));
    }
    #endregion
}
