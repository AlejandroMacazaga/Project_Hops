using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Utils.Extensions;

namespace Utils.BlackboardSystem
{
    public class Arbiter
    {
        private readonly List<IExpert> _experts = new();

        public void AddExpert(IExpert expert)
        {
            Preconditions.CheckNotNull(expert);
            _experts.Add(expert);
        }
        
        public void RemoveExpert(IExpert expert)
        {
            Preconditions.CheckNotNull(expert);
            _experts.Remove(expert);
        }

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            IExpert bestExpert = null;
            var bestExpertImportance = int.MinValue;
            
            foreach (var expert in _experts)
            {
                var importance = expert.GetImportance(blackboard);
                if (importance <= bestExpertImportance) continue;
                bestExpert = expert;
                bestExpertImportance = importance;
            }
            
            bestExpert?.Execute(blackboard);
            
            var actions = blackboard.PassedActions;
            blackboard.ClearActions();
            
            return actions;
        }
    }
}