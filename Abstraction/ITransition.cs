namespace YekGames.StateMachineSystem.Abstraction
{
    public interface ITransition
    {
        IState TargetState { get; set; } // TODO make set internal
        void OnUpdate(float deltaTime);
        bool CheckTransition();
        void OnTransitionEntered();
        void OnTransitionExited();
    }
}