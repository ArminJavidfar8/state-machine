using YekGames.StateMachineSystem.Abstraction;
using System.Collections.Generic;

namespace YekGames.StateMachineSystem.Core
{
    public class StateMachine : IStateMachine
    {
        private IList<IState> _States;
        private IState _CurrentState;
        private IDictionary<IState, IList<ITransition>> _Transitions;

        public IState CurrentState => _CurrentState;

        public StateMachine()
        {
            _States = new List<IState>();
            _Transitions = new Dictionary<IState, IList<ITransition>>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_CurrentState != null)
            {
                _CurrentState.OnUpdate(deltaTime);
                if (_Transitions.ContainsKey(_CurrentState))
                {
                    foreach (ITransition transition in _Transitions[_CurrentState])
                    {
                        transition.OnUpdate(deltaTime);
                        if (transition.CheckTransition())
                        {
                            ChangeState(transition.TargetState);
                        }
                    }
                }
            }
        }

        public void AddState(IState state)
        {
            if (_States.Contains(state))
            {
                throw new StateMachineException(ExceptionType.StateAlreadyAdded);
            }
            _States.Add(state);
            if (_CurrentState == null)
            {
                _CurrentState = state;
                _CurrentState.OnStateEntered();
            }
        }

        public void ChangeState(IState newState)
        {
            if (!_States.Contains(newState))
            {
                throw new StateMachineException(ExceptionType.StateIsNotAdded);
            }
            if (_CurrentState != newState)
            {
                _CurrentState.OnStateExited();
                foreach (KeyValuePair<IState, IList<ITransition>> item in _Transitions)
                {
                    if (item.Key == _CurrentState)
                    {
                        foreach (ITransition transition in item.Value)
                        {
                            transition.OnTransitionExited();
                        }
                    }
                }

                _CurrentState = newState;

                _CurrentState.OnStateEntered();
                foreach (KeyValuePair<IState, IList<ITransition>> item in _Transitions)
                {
                    if (item.Key == newState)
                    {
                        foreach(ITransition transition in item.Value)
                        {
                            transition.OnTransitionEntered();
                        }
                    }
                }
            }
        }

        public void AddTransition(IState sourceState, ITransition transition, IState targetState)
        {
            if (!_States.Contains(sourceState) || !_States.Contains(targetState))
            {
                throw new StateMachineException(ExceptionType.StateIsNotAdded);
            }
            if (!_Transitions.ContainsKey(sourceState))
            {
                _Transitions.Add(sourceState, new List<ITransition>());
            }
            foreach (ITransition transitionItem in _Transitions[sourceState])
            {
                if (transitionItem.TargetState == targetState)
                {
                    throw new StateMachineException(ExceptionType.SourceAndTargetStatesAlreadyHaveTransition);
                }
            }
            transition.TargetState = targetState;
            _Transitions[sourceState].Add(transition);
            if (CurrentState == sourceState)
            {
                transition.OnTransitionEntered();
            }
        }
    }
}