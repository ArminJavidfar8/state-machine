namespace YekGames.StateMachineSystem.Abstraction
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        void OnUpdate(float deltaTime);
        void AddState(IState state);
        public void AddTransition(IState sourceState, ITransition transition, IState targetState);
        void ChangeState(IState newState);
    }
}