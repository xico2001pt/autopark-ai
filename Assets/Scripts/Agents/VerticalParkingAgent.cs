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
    [SerializeField] private float _maxEpisodeTime = 20f;
    [SerializeField] private float _finishDistance = 0.4f;
    [SerializeField] private float _finishRotationDotProduct = 0.995f;
    
    private VehicleController _vehicleController;
    private float _episodeTime;
    private float _previousDistance;
    private float _currentDistance;
    #endregion
    
    #region Unity Methods
    private void Awake() {
        _vehicleController = GetComponent<VehicleController>();
    }

    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
        _episodeTime += deltaTime;
        
        // Time failure penalty
        if (_episodeTime >= _maxEpisodeTime) {
            AddReward(_rewards.TimeFailurePenalty);
            EndEpisode();
        }
        
        // Time penalty
        AddReward(_rewards.DurationPenalty * deltaTime);
        
        // Distance to target reward
        _previousDistance = _currentDistance;
        _currentDistance = _trainingManager.GetDistanceToTarget();
        AddReward(_rewards.DistanceToTarget * (_previousDistance - _currentDistance));
        
        // Parking reward
        if (IsParkingFinished()) {
            AddReward(_rewards.ParkingReward);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision other) {
        AddReward(_rewards.CollisionPenalty);
        EndEpisode();
    }
    #endregion
    
    #region Agent Methods
    public override void OnEpisodeBegin() {
        _trainingManager.GenerateEpisode();
        _episodeTime = 0f;
        _currentDistance = _trainingManager.GetDistanceToTarget();
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.position);  // Vehicle position
        sensor.AddObservation(_trainingManager.GetTargetSlotPosition());  // Target slot position
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
    #endregion
}
