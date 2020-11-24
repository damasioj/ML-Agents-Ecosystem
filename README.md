# Master's Thesis: Ecosystem Simulation

This project is being developed for a Master's thesis that studies the implementation of behaviours in subjects using Machine Learning. This scenario is a combination of all other scenarios related to the thesis (animal, collector and hauler) and attempts to integrate all of the agents into a single environment without retraining them.

This project also focuses on introducing an abstract and dynamic code architecture that can be used for a variety of scenarios and agents.  

# Unity ML-Agents SDK

The project uses Unity ML-Agents to train agents.

# Goals

The following are the goals of this project:

- [x] Integrate agents into environment without requiring retraining
	- [x] Resource Collector Agent
	- [x] Animal Agent
	- [ ] ~~Hauler Agent~~
- [x] Adding new objects to the environments (i.e. targets) and having agents interact with them as required

# Requirements

To run this environment, you will need:
- Python 3.6.1 or greater
	- mlagents version 0.16.1
- Unity 2019.2.0f1 or greater

# Limitations and Observations

## Data Assocation

Machine learning in general faces an issue with associating input data. All observations of the environment are typically bundled together into a single array and then passed through the model. The model must create an association on its own through training which leads to increased training times. It would have made the agent’s learning process a lot faster if it were possible to associate data to their virtual objects. For instance, in the first scenario (AnimalSimulation), it would have been beneficial to associate the enemy’s velocity and position to the enemy itself. This way when processing those values, the model would understand that those values have no relation to any other object aside from the enemy, decreasing its training time. This, of course, is a limitation in machine learning and is beyond the scope of this project and study.

## Abstraction with Unity

While developing the various scenarios within Unity it was found that the framework has some limitations regarding abstraction. These limitations affected the time it took to develop scenarios and a proper architecture for implementing the agents. One of the main limitations is the lack of support for interfaces with Unity’s game objects. When referencing a game object, only the concrete or abstract class is returned. It is necessary to manually convert and verify if a game object implements an interface before you can reference that interface directly. The main issue with this fact is that most of Unity’s supported functions use game objects, so it is more cost effective to instead use abstract classes. Another limitation is that all game objects, including agents, must inherit from the Monobehaviour class. The Monobehaviour class is responsible for instantiating and updating game objects. This makes it impossible to use dependency inversion. While this limitation has not greatly affected the development of the environments, it may be a factor when implementing agents into a real game environment.

## Input simplification

*From ResourceCollector Scenario*

When providing inputs to an agent, it is more efficient to divert the responsibility of quantitative requirements to another class. This class would send updated information of the targets to the agent in real-time and on-demand; at this point, one can add basic logic in the agent class for deciding what is the next target. Using this approach, the agent would only need a single target and does not need any information related to the task’s requirements.

This simplification creates more flexibility as the number of targets and materials are no longer input parameters for the model, thus changing them does not affect the algorithm in any way. The result of this is that we can freely change the target and its behaviours without requiring any retraining whatsoever; the agent focuses solely on completing a task rather than understanding its requirements.

## Use of Finite-State Machines (FSMs)

*From ResourceCollector Scenario*

The scenario uses a basic implementation of finite-states to manage actions and animations from the agent. These states are triggered depending on the agent’s chosen action and environment conditions, rather than set by rules or an algorithm. For example, the agent enters the gathering state when it approaches the defined target. The gathering state automatically locks the agent in position and executes the action and necessary animations. 

For the Ecosystem environment, all agents implement FSMs to manage actions and animations. 

## Object dimension information

*From Hauler Scenario*

Ideally, it would be best to have the possibility to provide the agent with an object's dimensions, but this seems to be more complicated than initially thought. The dimensions of a box, for instance, can be defined with 8 given vectors (1 vector for each point), but if we change the target object to a ball the number of vectors are virtually infinite. The most that can be provided is the size of an object - but again, this information has limited usefulness if one does not know its shape.

The solution we used for this problem is creating an enum that will represent the different object shapes. The enum would obviously be limiting, and adding a new shape would require retraining, but it would at least provide a few options to work with.

*Note: The enum solution has been kept within the project but is not used. It is merely for demonstration purposes.*
