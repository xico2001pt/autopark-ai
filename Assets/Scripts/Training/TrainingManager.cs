using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    #region Fields
    [SerializeField] private ParkingSlotGenerator _parkingSlotGenerator;
    [SerializeField] private GameObject _targetSlotPrefab;
    [SerializeField] private VehicleController _vehicleController;
    [SerializeField] private SpawnArea[] _spawnAreas;
    
    private Transform _targetSlot;
    #endregion
    
    #region Public Methods
    public void GenerateEpisode() {
        ResetVehicle();
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
    
    #region Private Methods
    private void ResetVehicle() {
        SpawnArea spawnArea = GetRandomSpawnArea();
        
        float xPosition = Random.Range(spawnArea.renderer.bounds.min.x, spawnArea.renderer.bounds.max.x);
        float zPosition = Random.Range(spawnArea.renderer.bounds.min.z, spawnArea.renderer.bounds.max.z);
        Vector3 spawnPosition = new Vector3(xPosition, spawnArea.renderer.transform.position.y, zPosition);
        
        float yRotation = Random.Range(spawnArea.minRotation, spawnArea.maxRotation);
        Quaternion spawnRotation = Quaternion.Euler(0f, yRotation, 0f);
        
        _vehicleController.ResetVehicle();
        
        _vehicleController.transform.position = spawnPosition;
        _vehicleController.transform.rotation = spawnRotation;
    }
    
    private SpawnArea GetRandomSpawnArea() {
        float totalWeight = 0f;
        foreach (SpawnArea spawnArea in _spawnAreas) {
            totalWeight += spawnArea.weight;
        }
        float randomWeight = Random.Range(0f, totalWeight);
        SpawnArea selectedSpawnArea = _spawnAreas[0];
        foreach (SpawnArea spawnArea in _spawnAreas) {
            randomWeight -= spawnArea.weight;
            if (randomWeight <= 0f) {
                selectedSpawnArea = spawnArea;
                break;
            }
        }
        return selectedSpawnArea;
    }
    #endregion
}
