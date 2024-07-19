using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSlotGenerator : MonoBehaviour {
    #region Fields
    [SerializeField] private Transform[] _parkingSlots;
    [SerializeField] private GameObject[] _carPrefabs;
    [SerializeField] private Transform _vehiclesParent;
    [SerializeField] private float _targetRotation = 90f;
    [SerializeField] private float _rotationDeviation = 10f;
    [SerializeField] private int _freeSlots = 1;
    #endregion
    
    #region Public Methods
    public void GenerateParkingSlots() {
        DestroyVehicles();
        _parkingSlots = ShuffleArray(_parkingSlots);
        
        for (int i = _freeSlots; i < _parkingSlots.Length; i++) {
            int randomIndex = Random.Range(0, _carPrefabs.Length);
            GameObject car = Instantiate(_carPrefabs[randomIndex], _parkingSlots[i].position, Quaternion.identity, _vehiclesParent);
            car.transform.rotation = Quaternion.Euler(0f, _targetRotation + Random.Range(-_rotationDeviation, _rotationDeviation), 0f);
        }
    }
    
    public Transform[] GetFreeParkingSlots() {
        return _parkingSlots.Take(_freeSlots).ToArray();
    }
    
    public float GetTargetRotation() {
        return _targetRotation;
    }
    
    public Transform GetVehicleParent() {
        return _vehiclesParent;
    }
    #endregion
    
    #region Private Methods
    private static T[] ShuffleArray<T>(T[] array) {
        for (int i = 0; i < array.Length; i++) {
            T temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }
    
    private void DestroyVehicles() {
        foreach (Transform vehicle in _vehiclesParent) {
            Destroy(vehicle.gameObject);
        }
    }
    #endregion
}
