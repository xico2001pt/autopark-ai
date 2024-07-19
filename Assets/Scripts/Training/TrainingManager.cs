using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    #region Fields
    [SerializeField] private ParkingSlotGenerator _parkingSlotGenerator;
    [SerializeField] private GameObject _targetSlotPrefab;
    [SerializeField] private Transform _targetSlotParent;
    [SerializeField] private Transform _carTransform;
    
    private Transform _targetSlot;
    #endregion
    
    #region Unity Methods
    private void Start() {
        GenerateEpisode();  // Debug
    }
    #endregion
    
    #region Public Methods
    public void GenerateEpisode() {
        _parkingSlotGenerator.GenerateParkingSlots();
        _targetSlot.position = _parkingSlotGenerator.GetFreeParkingSlots()[0].position;
        Quaternion targetRotation = Quaternion.Euler(0f, _parkingSlotGenerator.GetTargetRotation(), 0f);
        _targetSlot.rotation = targetRotation;
    }
    public float GetDistanceToTarget() {
        return Vector3.Distance(_carTransform.position, _targetSlot.position);
    }
    
    public float GetRotationDotProduct() {
        return Vector3.Dot(_carTransform.forward, _targetSlot.forward);
    }
    #endregion
}
