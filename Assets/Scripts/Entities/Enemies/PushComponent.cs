using KBCore.Refs;
using UnityEngine;

namespace Entities.Enemies
{
    public class PushComponent : ValidatedMonoBehaviour, IVisitable
    {
        [SerializeField, HideInInspector, Self]
        private InterfaceRef<IPushed> toPush;

        public EntityTeam team;
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Push(Vector3 force)
        {
            if (toPush.Value.CanBePushed()) toPush.Value.Push(force);
        }
    }
}
