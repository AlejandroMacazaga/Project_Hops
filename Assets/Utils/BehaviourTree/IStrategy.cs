namespace Utils.BehaviourTree
{
    public interface IStrategy
    {
        Node.Status Process();

        void Reset()
        {
            // Noop
        }
    }

    public class ActionStrategy : IStrategy
    {
        private readonly System.Action _action;
        
        public ActionStrategy(System.Action action)
        {
            _action = action;
        }
        
        public Node.Status Process()
        {
            _action();
            return Node.Status.Success;
        }
    }
    
    public class Condition : IStrategy
    {
        private readonly System.Func<bool> _condition;
        
        public Condition(System.Func<bool> condition)
        {
            _condition = condition;
        }
        
        public Node.Status Process() => _condition() ? Node.Status.Success : Node.Status.Failure;
    }
    
    
    
}