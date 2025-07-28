using System;

namespace YekGames.StateMachineSystem.Core
{
    public enum ExceptionType
    {
        None = 0,
        StateIsNotAdded,
        StateAlreadyAdded,
        SourceAndTargetStatesAlreadyHaveTransition,
    }

    public class StateMachineException : Exception
    {
        public ExceptionType ExceptionType { get; private set; }

        public StateMachineException(ExceptionType exceptionType)
        {
            ExceptionType = exceptionType;
        }
    }
}