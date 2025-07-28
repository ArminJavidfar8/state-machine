using YekGames.StateMachineSystem.Abstraction;

namespace YekGames.StateMachineSystem.Tests
{
    public class FakeState : IState
    {
        public bool OnUpdateTriggered {  get; set; }
        public bool OnStateEnteredTriggered {  get; set; }
        public bool OnStateExitedTriggered { get; set; }

        public void OnUpdate(float deltaTime)
        {
            OnUpdateTriggered = true;
        }

        public void OnStateEntered()
        {
            OnStateEnteredTriggered = true;
        }

        public void OnStateExited()
        {
            OnStateExitedTriggered = true;
        }
    }
}