using System;

namespace YekGames.StateMachineSystem.Abstraction
{
    public interface IState
    {
        void OnUpdate(float deltaTime);
        void OnStateEntered();
        void OnStateExited();
    }
}