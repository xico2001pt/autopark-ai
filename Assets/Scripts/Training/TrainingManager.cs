using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    #region Fields
    [SerializeField] private ParkingSlotGenerator _parkingSlotGenerator;
    [SerializeField] private GameObject _targetSlotPrefab;
    [SerializeField] private VehicleController _vehicleController;
    
    private Transform _targetSlot;
    #endregion
    
    #region Unity Methods
    private void Start() {
        //GenerateEpisode();  // Debug
    }
    #endregion
    
    #region Public Methods
    public void GenerateEpisode() {
        // TODO: Reset car
        _parkingSlotGenerator.GenerateParkingSlots();
        Quaternion targetRotation = Quaternion.Euler(0f, _parkingSlotGenerator.GetTargetRotation(), 0f);
        _targetSlot = Instantiate(
            _targetSlotPrefab, 
            _parkingSlotGenerator.GetFreeParkingSlots()[0].position, 
            targetRotation,
            _parkingSlotGenerator.GetVehicleParent()).transform;
    }
    public float GetDistanceToTarget() {
        return Vector3.Distance(_vehicleController.transform.position, _targetSlot.position);
    }
    
    public float GetRotationAbsDotProduct() {
        return Mathf.Abs(Vector3.Dot(_vehicleController.transform.forward, _targetSlot.forward));
    }
    
    public Vector3 GetTargetSlotPosition() {
        return _targetSlot.position;
    }
    #endregion
}
