# AutoPark AI
An AI agent that learns to park a vehicle in a vertical parking spot using **Reinforcement Learning**.

## Table of Contents
- [Problem Formalization](#problem-formalization)
    - [State Space](#state-space)
    - [Action Space](#action-space)
    - [Reward Function](#reward-function)
- [Implementation Details](#implementation-details)
    - [Model Architecture](#model-architecture)
    - [Training Environment](#training-environment)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Future Work](#future-work)
- [License](#license)

## Problem Formalization

### State Space
The state space has a total size of **12** and is defined by the following variables:

| Variable | Description | Domain | Size |
| --- | --- | --- | --- |
| Relative Position | The relative position (X and Z) of the parking spot with respect to the vehicle. | [-∞, ∞] | 2 |
| Relative Orientation | The relative orientation (Y) of the parking spot with respect to the vehicle. | [-π, π] | 1 |
| Orientation Absolute Dot Product | The absolute value of the dot product between the orientation of the vehicle and the orientation of the parking spot. | [0, 1] | 1 |
| Sensors Distance | The distance between the sensors and any detected obstacle. | [0, 6] | 8 |

**Note:** The 3 most recent states space are fed to the model as input. This serves as a temporal context for the model to learn from, namely velocity and acceleration.

### Action Space
There are **2** possible actions that the agent can take:

| Variable | Description | Domain |
| --- | --- | --- |
| Throttle | The throttle value of the vehicle. | [-1, 1] |
| Steering | The steering angle of the vehicle. | [-1, 1] |

### Reward Function
The reward function is defined as follows:

| Task | Description | Reward |
| --- | --- | --- |
| Parking | The agent parks the vehicle successfully. | 4000 |
| Parking Rotation | This reward increases as the agent aligns the vehicle with the parking spot. | [0, 1500] |
| Parking Distance | This reward increases as the agent gets closer to the parking spot. | 50 per distance unit |
| Collision | The agent collides with an obstacle. | -600 |
| Time Failure | The agent takes too long to park the vehicle. | -650 per time unit |
| Duration Penalty | The agent is penalized for taking too long to park the vehicle. | -0.01 per step |

## Implementation Details

### Model Architecture
The model architecture is defined as follows:

| Layer | Description | Size |
| --- | --- | --- |
| Input | The input layer of the model. | 12 |
| Hidden 1 | The first hidden layer of the model. | 128 |
| Hidden 2 | The second hidden layer of the model. | 128 |
| Hidden 3 | The third hidden layer of the model. | 128 |
| Hidden 4 | The fourth hidden layer of the model. | 128 |
| Output | The output layer of the model. | 2 |

### Training Environment
The training environment is implemented in Unity using the ML-Agents Toolkit. The environment is a 3D simulation of a parking lot with a vehicle and parking spots with other vehicles as obstacles. The agent is trained to park the vehicle in a vertical parking spot. There are two spawn areas for the vehicle and parking spots, which allows the agent to learn parking in different scenarios.

## Installation
The prerequisites for this project are:

- Python 3.9
- pip
- Unity 2022.3

To install this project, first clone the repository:

```bash
git clone https://github.com/xico2001pt/autopark-ai
```

Then, install the dependencies:

```bash
pip install -r requirements.txt
```

## Usage
To train the model, run the following command and then hit Play in the Unity Editor:

```bash
mlagents-learn ./Configs/vertical_parking_agent.yaml --run-id=VerticalParking --torch-device=cuda
```

To test the model, select the test scene in the Unity Editor, associate the model with the agent, and hit Play.

## Project Structure
```
autopark-ai/
├── Assets/
│   ├── Materials/
│   ├── Models/
│   ├── Prefabs/
│   ├── Scenes/
│   │   ├── Test/
│   │   └── Train/
│   └── Scripts/
│       ├── Agents/
│       ├── Driving/
│       ├── Input/
│       └── Training/
├── Configs/
├── Packages/
└── ProjectSettings/
```

## Future Work
- Improve the model robustness by training on more complex scenarios and during longer periods.
- Implement a more sophisticated reward function, such as rewarding the agent for traveling smaller distances.
- Create a second model that learns parallel parking.
- Develop an algorithm that detects the parking type (vertical or parallel) and selects the appropriate model.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Please **ACKNOWLEDGE THE AUTHOR** if you use this repository by including a link to this repository.
