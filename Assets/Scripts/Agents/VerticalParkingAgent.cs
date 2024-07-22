using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VerticalParkingAgent : Agent {
    #region Fields
    [Header("References")]
    [SerializeField] private TrainingManager _trainingManager;
    [SerializeField] private SensorController[] _sensors;
    
    [Header("Rewards")]
    [SerializeField] private AgentRewards _rewards;
    
    [Header("Settings")]
    [SerializeField] private int _maxEpisodeSteps = 50000;
    [SerializeField] private float _finishDistance = 0.4f;
    [SerializeField] private float _finishRotationDotProduct = 0.995f;
    
    private VehicleController _vehicleController;
    private int _episodeSteps;
    private float _previousDistance;
    private float _currentDistance;

    private bool _isTraining;
    #endregion
    
    #region Unity Methods
    private void Awake() {
        _vehicleController = GetComponent<VehicleController>();
        _isTraining = false;
    }

    private void FixedUpdate() {
        if (!_isTraining) return;
        
        _episodeSteps++;
        
        // Time failure penalty
        if (_episodeSteps >= _maxEpisodeSteps) {
            AddReward(_rewards.TimeFailurePenalty);
            FinishEpisode();
        }
        
        // Time penalty
        AddReward(_rewards.DurationPenalty);
        
        // Distance to target reward
        _previousDistance = _currentDistance;
        _currentDistance = _trainingManager.GetDistanceToTarget();
        AddReward(_rewards.DistanceToTarget * (_previousDistance - _currentDistance));
        
        // Parking reward
        if (IsParkingFinished()) {
            AddReward(_rewards.ParkingReward);
            float rotationReward = _rewards.ParkingRotationReward * Mathf.Pow(_trainingManager.GetRotationAbsDotProduct(), 2);
            AddReward(rotationReward);
            Debug.Log("Parked!");
            FinishEpisode();
        }
    }

    private void OnCollisionEnter(Collision other) {
        AddReward(_rewards.CollisionPenalty);
        FinishEpisode();
    }
    #endregion
    
    #region Agent Methods
    public override void OnEpisodeBegin() {
        _trainingManager.GenerateEpisode();
        _episodeSteps = 0;
        _currentDistance = _trainingManager.GetDistanceToTarget();
        _isTraining = true;
    }
    
    public override void CollectObservations(VectorSensor sensor) {
        Vector2 relativePosition = new Vector2(
            _vehicleController.transform.localPosition.x - _trainingManager.GetTargetSlotPosition().x,
            _vehicleController.transform.localPosition.z - _trainingManager.GetTargetSlotPosition().z
        );
        
        float relativeRotation = _trainingManager.GetTargetSlotRotation().y - _vehicleController.transform.localEulerAngles.y;
        relativeRotation *= Mathf.Deg2Rad;
        
        sensor.AddObservation(relativePosition);  // Relative position
        sensor.AddObservation(relativeRotation);  // Relative rotation
        sensor.AddObservation(_trainingManager.GetRotationAbsDotProduct());  // Dot product of vehicle and target slot forward vectors
        foreach (SensorController sensorController in _sensors) {
            sensor.AddObservation(sensorController.GetRayDistance());  // Ray distance
        }
    }
    
    public override void OnActionReceived(ActionBuffers actions) {
        float throttle = actions.ContinuousActions[0];
        float steering = actions.ContinuousActions[1];
        
        _vehicleController.SetThrottle(throttle);
        _vehicleController.SetSteering(steering);
    }
    #endregion
    
    #region Private Methods
    private bool IsParkingFinished() {
        return _trainingManager.GetDistanceToTarget() < _finishDistance && _trainingManager.GetRotationAbsDotProduct() > _finishRotationDotProduct;
    }
    
    private void FinishEpisode() {
        _isTraining = false;
        _episodeSteps = 0;
        EndEpisode();
    }
    #endregion
}
