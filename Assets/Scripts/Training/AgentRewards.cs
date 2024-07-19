[System.Serializable]
public struct AgentRewards {
    public float DurationPenalty;  // Reward penalty for each second
    public float TimeFailurePenalty;  // Reward penalty for failing to park in time
    public float DistanceToTarget;  // Reward for getting closer to the target
    public float CollisionPenalty;  // Reward penalty for colliding with obstacles
    public float ParkingReward;  // Reward for parking the vehicle
}
