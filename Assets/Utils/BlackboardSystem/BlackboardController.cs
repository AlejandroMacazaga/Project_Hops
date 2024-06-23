using UnityEngine;

namespace Utils.BlackboardSystem
{
    public class BlackboardController : MonoBehaviour
    {
        [SerializeField] private BlackboardData blackboardData;
        private readonly Blackboard _blackboard = new();
        private readonly Arbiter _arbiter = new();

        private void Awake()
        {
            blackboardData.SetValuesOnBlackboard(_blackboard);
            _blackboard.Debug();
        }

        private void Update()
        {
            foreach (var action in _arbiter.BlackboardIteration(_blackboard))
            {
                action();
            }
        }
        
        public Blackboard GetBlackboard() => _blackboard;
        
        public void AddExpert(IExpert expert) => _arbiter.AddExpert(expert);
        
        public void RemoveExpert(IExpert expert) => _arbiter.RemoveExpert(expert);
    }
}