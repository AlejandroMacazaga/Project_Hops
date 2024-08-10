using System;

namespace Utils.StateMachine
{
    public class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
        
        public Action TransitionalAction { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
            TransitionalAction = null;
        }

        public Transition(IState to, IPredicate condition, Action transitionalAction)
        {
            To = to;
            Condition = condition;
            TransitionalAction = transitionalAction;
        }
    }
}