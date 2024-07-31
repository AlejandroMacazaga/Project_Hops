using System;

namespace Utils.StateMachine
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
        
        Action TransitionalAction { get;  }
    }
}