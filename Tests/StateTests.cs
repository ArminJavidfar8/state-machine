using NUnit.Framework;
using YekGames.StateMachineSystem.Abstraction;
using YekGames.StateMachineSystem.Core;

namespace YekGames.StateMachineSystem.Tests
{
    public class StateTests
    {
        #region OnUpdate
        [Test]
        public void OnUpdate_BeforeStateMachineUpdate_DoesNotGetInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            stateMachine.AddState(state);

            Assert.IsFalse(state.OnUpdateTriggered);
        }
        
        [Test]
        public void OnUpdate_WhenStateMachineUpdated_GetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            stateMachine.AddState(state);

            stateMachine.OnUpdate(1);

            Assert.IsTrue(state.OnUpdateTriggered);
        }

        [Test]
        public void OnUpdate_WhenStateMachineUpdated_OnlyCurrentStateUpdateGetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            FakeState state2 = new FakeState();
            stateMachine.AddState(state);
            stateMachine.AddState(state2);

            stateMachine.OnUpdate(1);

            Assert.IsTrue(state.OnUpdateTriggered);
            Assert.IsFalse(state2.OnUpdateTriggered);
        }
        #endregion

        #region OnStateEntered
        [Test]
        public void OnStateEntered_AddSingleState_OnStateEnteredGetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();

            stateMachine.AddState(state);

            Assert.IsTrue(state.OnStateEnteredTriggered);
        }
        
        [Test]
        public void OnStateEntered_AddMultipeStates_OnlyCurrentStateOnStateEnteredGetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            FakeState state2 = new FakeState();

            stateMachine.AddState(state);
            stateMachine.AddState(state2);

            Assert.IsTrue(state.OnStateEnteredTriggered);
            Assert.IsFalse(state2.OnStateEnteredTriggered);
        }

        [Test]
        public void OnStateEntered_WhenStateChanged_NewStateOnStateEnteredGetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            FakeState state2 = new FakeState();
            stateMachine.AddState(state);
            stateMachine.AddState(state2);

            stateMachine.ChangeState(state2);

            Assert.IsTrue(state2.OnStateEnteredTriggered);
        }
        #endregion

        #region OnStateExited
        [Test]
        public void OnStateExited_WhenStateChanges_CurrentStateOnStateExitedGetsInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState oldState = new FakeState();
            FakeState newState = new FakeState();
            stateMachine.AddState(oldState);
            stateMachine.AddState(newState);

            stateMachine.ChangeState(newState);

            Assert.IsTrue(oldState.OnStateExitedTriggered);
        }
        #endregion
    }
}