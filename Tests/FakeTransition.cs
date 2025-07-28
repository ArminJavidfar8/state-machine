using YekGames.StateMachineSystem.Abstraction;

namespace YekGames.StateMachineSystem.Tests
{
    public class FakeTransition : ITransition
    {
        private bool _CheckResult;

        public bool OnUpdateTriggered { get; set; }
        public bool OnTransitionEnteredTriggered { get; set; }
        public bool OnTransitionExitedTriggered { get; set; }

        public IState TargetState { get; set; }
        public bool CheckResult { get => _CheckResult; set => _CheckResult = value; }

        public FakeTransition(bool checkResult)
        {
            _CheckResult = checkResult;
        }
        public void OnUpdate(float deltaTime)
        {
            OnUpdateTriggered = true;
        }

        public void OnTransitionEntered()
        {
            OnTransitionEnteredTriggered = true;
        }

        public void OnTransitionExited()
        {
            OnTransitionExitedTriggered = true;
        }

        public bool CheckTransition()
        {
            return _CheckResult;
        }
    }
}