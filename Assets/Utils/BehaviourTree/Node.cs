using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace Utils.BehaviourTree
{
    public class RandomSelector : PrioritySelector
    {
        protected override List<Node> SortChildren() => Children.Shuffle().ToList();
        
        public RandomSelector(string name) : base(name) { }
    }
    public class PrioritySelector : Selector
    {
        List<Node> _sortedChildren;
        List<Node> SortedChildren => _sortedChildren ??= SortChildren();
        
        protected virtual List<Node> SortChildren() => Children.OrderByDescending(child => child.Priority).ToList();
        
        public PrioritySelector(string name) : base(name) { }

        public override void Reset()
        {
            base.Reset();
            _sortedChildren = null;
        }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    case Status.Failure:
                    default:
                        continue;
                }
            }

            return Status.Failure;
        }
    }
    
    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }
        
        public override Status Process()
        {
            if (CurrentChildIndex < Children.Count)
            {
                switch (Children[CurrentChildIndex].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    case Status.Failure:
                    default:
                        CurrentChildIndex++;
                        return Status.Running;
                }
            }
            Reset();
            return Status.Failure;
        }
    }
    
    public class Sequence : Node
    {
        public Sequence(string name, int priority) : base(name, priority) { }
        
        public override Status Process()
        {
            if (CurrentChildIndex < Children.Count)
            {
                switch (Children[CurrentChildIndex].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    case Status.Success:
                    default:
                        CurrentChildIndex++;
                        return CurrentChildIndex == Children.Count ? Status.Success : Status.Running;
                }
            }
            Reset();
            return Status.Success;
        }
    }
    
    public class Leaf : Node
    {
        private readonly IStrategy _strategy;

        public Leaf(string name, IStrategy strategy, int priority) : base(name, priority)
        {
            // 
            _strategy = strategy;
        }
        
        public override Status Process() => _strategy.Process();
        public override void Reset() => _strategy.Reset();
    }

    public class Node 
    {
        public enum Status { Success, Failure, Running }

        public readonly string Name;
        public readonly int Priority;
        
        public readonly List<Node> Children = new();
        protected int CurrentChildIndex = 0;
        
        public Node(string name, int priority = 0)
        {
            Name = name;
            Priority = priority;
        }
        
        public void AddChild(Node child) => Children.Add(child);

        public virtual Status Process() => Children[CurrentChildIndex].Process();
        
        public virtual void Reset()
        {
            CurrentChildIndex = 0;
            foreach (var child in Children)
            {
                child.Reset();
            }
        }
    }
    
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override Status Process()
        {
            while (CurrentChildIndex < Children.Count)
            {
                var status = Children[CurrentChildIndex].Process();
                if (status != Status.Success) return status;
                CurrentChildIndex++;
            }
            return Status.Success;
        }
    }
}
