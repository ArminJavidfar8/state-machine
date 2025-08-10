using System.Collections.Generic;

namespace YekGames.StateMachineSystem.Abstraction
{
    public interface IStateMachine
    {
        internal IList<IState> States { get; }
        internal IDictionary<IState, IList<ITransition>> Transitions { get; }
        internal ITransition LastTriggeredTransition { get; set; }

        IState CurrentState {  get; }
        void OnUpdate(float deltaTime);
        void AddState(IState state);
        public void AddTransition(IState sourceState, ITransition transition, IState targetState);
        void ChangeState(IState newState);
    }
}