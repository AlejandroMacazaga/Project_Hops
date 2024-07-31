using System;
using System.Collections.Generic;

namespace Utils.StateMachine
{
    public class StateMachine
    {
        private StateNode _current;
        private readonly Dictionary<Type, StateNode> _nodes = new();
        private readonly HashSet<ITransition> _anyTransitions = new();

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
                transition.TransitionalAction?.Invoke();
            }
            
            _current.State?.Update();
        }
        
        public void FixedUpdate()
        {
            _current.State?.FixedUpdate();
        }
        
        public void SetState(IState state)
        {
            _current = _nodes[state.GetType()];
            _current.State.OnEnter();
        }
        
        void ChangeState(IState state)
        {
            if (state == _current.State) return;
            
            var previousState = _current.State;
            var nextState = _nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState.OnEnter();
            
            _current = _nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in _anyTransitions)
                if (transition.To != _current.State && transition.Condition.Evaluate()) 
                    return transition;
            
            foreach (var transition in _current.Transitions)
                if (transition.Condition.Evaluate()) 
                    return transition;

            return null;
        }
        
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        
        public void AddTransition(IState from, IState to, Action transitionalAction, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition, transitionalAction);
        }
        
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }
        
        public void AddAnyTransition(IState to, IPredicate condition, Action transitionalAction)
        {
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition, transitionalAction));
        }

        private StateNode GetOrAddNode(IState state)
        {
            var node = _nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                _nodes.Add(state.GetType(), node);
            }

            return node;
        }
        
        private class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            
            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
            
            public void AddTransition(IState to, IPredicate condition, Action transitionalAction)
            {
                Transitions.Add(new Transition(to, condition, transitionalAction));
            }
        }
    }
}