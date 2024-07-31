using UnityEngine;

namespace Entities
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor)
        {
            
        }
    }
}
