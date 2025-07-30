# State Machine System

A lightweight and extensible **C# State Machine System** designed for managing complex object behaviors, game states, or any system requiring defined states and transitions.  

This library provides a clean, interface-driven approach, making your state logic **modular**, **testable**, and **easy to understand**.


## Features

- **Clean Abstraction** – Built with clear interfaces (`IState`, `ITransition`, `IStateMachine`) to separate concerns and promote a modular design.
- **State Lifecycle Management** – States come with built-in lifecycle methods (`OnStateEntered`, `OnUpdate`, `OnStateExited`) for executing logic specific to each state.
- **Transition-Driven Flow** – Define explicit transitions between states, including custom conditions and lifecycle callbacks (`OnTransitionEntered`, `OnTransitionExited`) for seamless state changes.
- **Centralized Control** – The `StateMachine` class acts as the orchestrator, managing state additions, transitions, and the overall update loop.
- **Extensible and Testable** – The interface-based design allows for easy creation of custom states and transitions, making the system highly adaptable to various needs and facilitating unit testing.

## How It Works

The **State Machine System** operates on three core concepts:

### States (`IState`)
Represents a specific condition or behavior of an object.  
You implement this interface to define what happens when a state is **entered**, **updated**, or **exited**.

### Transitions (`ITransition`)
Defines a potential move from a `SourceState` to a `TargetState`.  
Each transition has:
- `CheckTransition()` – Determines if the conditions for changing state are met.
- `OnTransitionEntered()` / `OnTransitionExited()` – Lifecycle methods for logic during transition entry and exit.

### State Machine (`IStateMachine`)
The central component that:
- Manages all registered states and their associated transitions  
- Processes updates and evaluates transition conditions  
- Handles state changes, invoking appropriate lifecycle methods on states and transitions  

---

When `OnUpdate(deltaTime)` is called on the `StateMachine`:
1. It first updates the `CurrentState`.
2. Then, it checks all transitions originating from the `CurrentState`.
3. If a transition’s `CheckTransition()` returns `true`, the state machine triggers a state change, gracefully exiting the old state and its transitions, and entering the new state and its associated transitions.

---

##  Direct State Changes (Without Transitions)

While `ITransition` objects provide a powerful way to define conditions for state changes, they are **not required** for every state switch.  
The `IStateMachine` interface exposes a `ChangeState(IState newState)` method that allows you to **force a state change immediately**, bypassing transition evaluation.

This is useful for:
- **Event-driven changes** – e.g., a Character pressing "Pause", level load complete, or game-over condition  
- **Unconditional state changes** – where the new state is determined by external logic  
- **Debugging/testing** – to jump directly to a specific state  

---

### When `ChangeState()` is called directly:
- `OnStateExited()` of the current state is invoked.  
- `OnTransitionExited()` is invoked for all transitions originating from the previous state.  
- `CurrentState` is updated to `newState`.  
- `OnStateEntered()` of the `newState` is invoked.  
- `OnTransitionEntered()` is invoked for all transitions originating from the `newState`.  

## 💻 Example
```csharp
// Assuming you have an instance of your state machine and states
IStateMachine CharacterStateMachine = new StateMachine();
CharacterIdleState idleState = new CharacterIdleState();
CharacterDeadState deadState = new CharacterDeadState(); // Assume this state is defined

CharacterStateMachine.AddState(idleState);
CharacterStateMachine.AddState(deadState);

// ... later, when the Character's health drops to zero
public void OnCharacterDied()
{
    Console.WriteLine("Character health reached zero. Forcing state change to DeadState.");
    CharacterStateMachine.ChangeState(deadState);
}
```

 ## Getting Started
Here's a quick example of how to use the State Machine System:

First, define your custom states and transitions by implementing the respective interfaces:

```csharp
public class CharacterIdleState : IState
{
    public void OnStateEntered() => Debug.Log("Character entered Idle State. Play idle animation.");
    public void OnUpdate(float deltaTime) { }
    public void OnStateExited() => Debug.Log("Character exited Idle State.");
}

public class CharacterWalkState : IState
{
    public void OnStateEntered() => Debug.Log("Character entered Walk State. Play walk animation.");
    public void OnUpdate(float deltaTime) { }
    public void OnStateExited() => Debug.Log("Character exited Walk State.");
}

public class CharacterIdleToWalkTransition : ITransition
{
    public IState TargetState { get; set; }
    private readonly IInputHandler _inputHandler;
    private const float HORIZONTAL_INPUT_THRESHOLD = 0.01f;

    public CharacterIdleToWalkTransition(IState targetState, IInputHandler inputHandler)
    {
        TargetState = targetState;
        _inputHandler = inputHandler;
    }

    public void OnTransitionEntered() => Debug.Log("Evaluating IdleToWalk transition...");
    public void OnUpdate(float deltaTime) { }
    public void OnTransitionExited() => Debug.Log("IdleToWalk transition finished evaluation.");

    public bool CheckTransition()
    {
        return Mathf.Abs(_inputHandler.GetHorizontalInput()) > HORIZONTAL_INPUT_THRESHOLD;
    }
}
```
Now, set up your state machine:

```csharp
public class CharacterController : MonoBehaviour
{
    private IStateMachine _characterStateMachine;
    private CharacterIdleState _idleState; 
    private CharacterWalkState _walkState;
    private CharacterIdleToWalkTransition _idleToWalkTransition;

    // Initializes the CharacterController with a state machine and an input handler instance injected.
    public void Init(IStateMachine stateMachine, IInputHandler inputHandler)
    {
        _characterStateMachine = stateMachine;

        _idleState = new CharacterIdleState();
        _walkState = new CharacterWalkState(); // Instantiated CharacterWalkState

        _characterStateMachine.AddState(_idleState);
        _characterStateMachine.AddState(_walkState);

        _idleToWalkTransition = new CharacterIdleToWalkTransition(_walkState, inputHandler);
        
        _characterStateMachine.AddTransition(_idleState, _idleToWalkTransition, _walkState); 
    }

    void Update()
    {
        _characterStateMachine.OnUpdate(Time.deltaTime);
    }
```


## Unit Tested
This State Machine System is rigorously tested to ensure reliability and correctness. It comes with a comprehensive suite of NUnit unit tests covering all core functionalities, including:

-  **State Management**: Adding states, changing states, and verifying state lifecycle callbacks.

- **Transition Logic**: Adding transitions, evaluating transition conditions, and confirming transition lifecycle callbacks.

- **Error Handling**: Testing custom exceptions for invalid operations (e.g., adding duplicate states, invalid transitions).
