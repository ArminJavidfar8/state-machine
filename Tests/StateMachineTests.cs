using NUnit.Framework;
using YekGames.StateMachineSystem.Core;
using YekGames.StateMachineSystem.Abstraction;

namespace YekGames.StateMachineSystem.Tests
{
    public class StateMachineTests
    {
        #region Add State
        [Test]
        public void AddState_AddSingleState_MustFillCurrentState()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();

            stateMachine.AddState(state);

            Assert.AreEqual(stateMachine.CurrentState, state);
        }

        [Test]
        public void AddState_AddTheSameState_ThrowsException()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            stateMachine.AddState(state);

            StateMachineException stateMachineException = Assert.Catch<StateMachineException>(() => stateMachine.AddState(state));

            Assert.AreEqual(stateMachineException.ExceptionType, ExceptionType.StateAlreadyAdded);
        }

        [Test]
        public void AddState_AddMoreStates_CurrentStateWontChange()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state1 = new FakeState();
            FakeState state2 = new FakeState();

            stateMachine.AddState(state1);
            stateMachine.AddState(state2);

            Assert.AreEqual(stateMachine.CurrentState, state1);
        }

        #endregion

        #region Change State
        [Test]
        public void ChangeState_OnTheSameState_NothingHappens()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState state = new FakeState();
            stateMachine.AddState(state);
            state.OnStateEnteredTriggered = false;

            stateMachine.ChangeState(state);

            Assert.IsFalse(state.OnStateEnteredTriggered);
            Assert.IsFalse(state.OnStateExitedTriggered);
        }

        [Test]
        public void ChangeState_NewStateNotAdded_ThrowsException()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState oldState = new FakeState();
            FakeState newState = new FakeState();
            stateMachine.AddState(oldState);

            StateMachineException stateMachineException = Assert.Catch<StateMachineException>(() => stateMachine.ChangeState(newState));

            Assert.AreEqual(stateMachineException.ExceptionType, ExceptionType.StateIsNotAdded);
        }

        [Test]
        public void ChangeState_AllGood_ChangesState()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState oldState = new FakeState();
            FakeState newState = new FakeState();
            stateMachine.AddState(newState);
            stateMachine.AddState(oldState);

            stateMachine.ChangeState(newState);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(newState));
        }

        #endregion

        #region Add Transition
        [Test]
        public void AddTransition_SourceNotAdded_ThrowsException()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(targetState);

            StateMachineException stateMachineException = Assert.Catch<StateMachineException>(() => stateMachine.AddTransition(sourceState, transition, targetState));

            Assert.AreEqual(stateMachineException.ExceptionType, ExceptionType.StateIsNotAdded);
        }

        [Test]
        public void AddTransition_TargetNotAdded_ThrowsException()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);

            StateMachineException stateMachineException = Assert.Catch<StateMachineException>(() => stateMachine.AddTransition(sourceState, transition, targetState));

            Assert.AreEqual(stateMachineException.ExceptionType, ExceptionType.StateIsNotAdded);
        }

        [Test]
        public void AddTransition_TheSameSourceAndTarget_ThrowsException()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);

            stateMachine.AddTransition(sourceState, transition, targetState);
            StateMachineException stateMachineException = Assert.Catch<StateMachineException>(() => stateMachine.AddTransition(sourceState, transition, targetState));

            Assert.AreEqual(stateMachineException.ExceptionType, ExceptionType.SourceAndTargetStatesAlreadyHaveTransition);

        }
        #endregion

        #region OnUpdate
        [Test]
        public void OnUpdate_FalseTransitionCondition_StateDoesNotChangeToTarget()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();

            ITransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);
            stateMachine.AddTransition(sourceState, transition, targetState);

            stateMachine.OnUpdate(1f);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(sourceState));
        }

        [Test]
        public void OnUpdate_TrueTransitionCondition_StateChangesToTarget()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();

            ITransition transition = new FakeTransition(true);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);
            stateMachine.AddTransition(sourceState, transition, targetState);

            stateMachine.OnUpdate(1f);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(targetState));
        }

        #endregion
    }
}