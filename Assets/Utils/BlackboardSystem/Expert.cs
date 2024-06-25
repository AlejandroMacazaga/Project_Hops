namespace Utils.BlackboardSystem
{
    public interface IExpert
    {
        int GetImportance(Blackboard blackboard);
        void Execute(Blackboard blackboard);
    }
    
    public class ExpertExample : IExpert
    {
        BlackboardController _blackboardController;
        public int GetImportance(Blackboard blackboard) => 0;
        public void Execute(Blackboard blackboard) { }
    }
}