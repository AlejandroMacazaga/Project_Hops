using KBCore.Refs;
using UnityEngine;

namespace Entities.Enemies
{
    public class StunComponent : ValidatedMonoBehaviour, IVisitable
    {
        [SerializeField, HideInInspector, Self]
        private InterfaceRef<IStunned> toStun;

        public EntityTeam team;
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Stun(float time)
        {
            if (toStun.Value.CanBeStunned()) toStun.Value.Stun(time);
        }
        
        
    }
}