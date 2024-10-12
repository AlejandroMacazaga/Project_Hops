using UnityEngine;

namespace Entities.Enemies
{
    public interface IPushed
    {
        void Push(Vector3 force);
        bool CanBePushed();
    }
}