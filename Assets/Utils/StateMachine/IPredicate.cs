using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.StateMachine
{
    public interface IPredicate
    {
        bool Evaluate();
    }
    
    public class And : IPredicate {
        private readonly List<IPredicate> _rules = new ();
        public bool Evaluate() => _rules.All(r => r.Evaluate());
    }

    public class Or : IPredicate {
        private readonly List<IPredicate> _rules = new List<IPredicate>();

        public Or(List<IPredicate> rules)
        {
            _rules = rules;
        }
        public bool Evaluate() => _rules.Any(r => r.Evaluate());
    }

    public class Not : IPredicate {
        IPredicate _rule;
        public bool Evaluate() => !_rule.Evaluate();
    }
}