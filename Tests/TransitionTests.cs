using NUnit.Framework;
using YekGames.StateMachineSystem.Abstraction;
using YekGames.StateMachineSystem.Core;

namespace YekGames.StateMachineSystem.Tests
{
    public class TransitionTests
    {
        #region OnUpdate
        [Test]
        public void OnUpdate_BeforeStateMachineUpdate_DoesNotGetInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);

            stateMachine.AddTransition(sourceState, transition, targetState);

            Assert.IsFalse(transition.OnUpdateTriggered);
        }
        
        [Test]
        public void OnUpdate_OnStateMachineUpdate_OnUpdateOfTransitionsGetInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            FakeTransition transition2 = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(targetState2);
            stateMachine.AddTransition(sourceState, transition, targetState1);
            stateMachine.AddTransition(sourceState, transition2, targetState2);

            stateMachine.OnUpdate(1f);

            Assert.IsTrue(transition.OnUpdateTriggered);
            Assert.IsTrue(transition2.OnUpdateTriggered);
        }
        #endregion

        #region CheckCondition
        [Test]
        public void CheckCondition_ConditionDoesNotMeet_CurrentStateDoesNotChange()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);
            stateMachine.AddTransition(sourceState, transition, targetState);

            stateMachine.OnUpdate(1);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(sourceState));
        }
        
        [Test]
        public void CheckCondition_ConditionMeets_CurrentStateChangesToTargetState()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState = new FakeState();
            FakeTransition transition = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState);
            stateMachine.AddTransition(sourceState, transition, targetState);

            transition.CheckResult = true;
            stateMachine.OnUpdate(1);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(targetState));
        }
        #endregion

        #region OnTransitionEntered
        [Test]
        public void OnTransitionEntered_WhenAddedToStateMachine_OnTransitionEnteredsGetInvoked()
        {
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);
            FakeTransition transition2 = new FakeTransition(false);
            stateMachine.AddState(sourceState);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(targetState2);

            stateMachine.AddTransition(sourceState, transition1, targetState1);
            stateMachine.AddTransition(sourceState, transition2, targetState2);

            Assert.IsTrue(transition1.OnTransitionEnteredTriggered);
            Assert.IsTrue(transition2.OnTransitionEnteredTriggered);
        }

        [Test]
        public void OnTransitionEntered_WhenTwoSetOfTransitionsAdded_OnlyCurrentStateTransitionEnteredGetsInvoked()
        {
            // Arrange
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState1 = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);
            
            FakeState sourceState2 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeTransition transition2 = new FakeTransition(false);

            stateMachine.AddState(sourceState1);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(sourceState2);
            stateMachine.AddState(targetState2);

            // Act
            stateMachine.AddTransition(sourceState1, transition1, targetState1);
            stateMachine.AddTransition(sourceState2, transition2, targetState2);

            // Assert
            Assert.IsTrue(transition1.OnTransitionEnteredTriggered);
            Assert.IsFalse(transition2.OnTransitionEnteredTriggered);
        }

        [Test]
        public void OnTransitionEntered_WhenStateChanges_OnTransitionEnteredsGetInvoked()
        {
            // Arrange
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState1 = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);
            
            FakeState sourceState2 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeState targetState3 = new FakeState();
            FakeTransition transition2 = new FakeTransition(false);
            FakeTransition transition3 = new FakeTransition(false);

            stateMachine.AddState(sourceState1);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(sourceState2);
            stateMachine.AddState(targetState2);
            stateMachine.AddState(targetState3);

            stateMachine.AddTransition(sourceState1, transition1, targetState1);
            stateMachine.AddTransition(sourceState2, transition2, targetState2);
            stateMachine.AddTransition(sourceState2, transition3, targetState3);

            // Act
            stateMachine.ChangeState(sourceState2);

            // Assert
            Assert.IsTrue(transition2.OnTransitionEnteredTriggered);
            Assert.IsTrue(transition3.OnTransitionEnteredTriggered);
        }

        [Test]
        public void OnTransitionEntered_WhenConditionMeets_OnTransitionEnteredsGetInvoked()
        {
            // Arrange
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState1 = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);

            FakeState targetState3 = new FakeState();
            FakeTransition transition2 = new FakeTransition(false);

            stateMachine.AddState(sourceState1);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(targetState3);

            stateMachine.AddTransition(sourceState1, transition1, targetState1);
            stateMachine.AddTransition(targetState1, transition2, targetState3);

            // Act
            transition1.CheckResult = true;
            stateMachine.OnUpdate(1);

            // Assert
            Assert.IsTrue(transition2.OnTransitionEnteredTriggered);
        }
        #endregion

        #region OnTransitionExited
        [Test]
        public void OnTransitionExited_WhenStateChanges_OnTransitionExitedsGetInvoked()
        {
            // Arrange
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState1 = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);
            FakeTransition transition2 = new FakeTransition(false);

            FakeState sourceState2 = new FakeState();
            FakeState targetState3 = new FakeState();
            FakeTransition transition3 = new FakeTransition(false);

            stateMachine.AddState(sourceState1);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(sourceState2);
            stateMachine.AddState(targetState2);
            stateMachine.AddState(targetState3);

            stateMachine.AddTransition(sourceState1, transition1, targetState1);
            stateMachine.AddTransition(sourceState1, transition2, targetState2);
            stateMachine.AddTransition(sourceState2, transition3, targetState3);

            // Act
            stateMachine.ChangeState(sourceState2);

            // Assert
            Assert.IsTrue(transition1.OnTransitionExitedTriggered);
            Assert.IsTrue(transition2.OnTransitionExitedTriggered);
        }

        [Test]
        public void OnTransitionExited_WhenConditionMeets_OnTransitionExitedsGetInvoked()
        {
            // Arrange
            IStateMachine stateMachine = new StateMachine();
            FakeState sourceState1 = new FakeState();
            FakeState targetState1 = new FakeState();
            FakeState targetState2 = new FakeState();
            FakeTransition transition1 = new FakeTransition(false);
            FakeTransition transition2 = new FakeTransition(false);

            FakeState sourceState2 = new FakeState();
            FakeState targetState3 = new FakeState();
            FakeTransition transition3 = new FakeTransition(false);

            stateMachine.AddState(sourceState1);
            stateMachine.AddState(targetState1);
            stateMachine.AddState(sourceState2);
            stateMachine.AddState(targetState2);
            stateMachine.AddState(targetState3);

            stateMachine.AddTransition(sourceState1, transition1, targetState1);
            stateMachine.AddTransition(sourceState1, transition2, targetState2);
            stateMachine.AddTransition(sourceState2, transition3, targetState3);

            // Act
            transition1.CheckResult = true;
            stateMachine.OnUpdate(1);

            // Assert
            Assert.IsTrue(transition1.OnTransitionExitedTriggered);
            Assert.IsTrue(transition2.OnTransitionExitedTriggered);
        }
        #endregion
    }
}