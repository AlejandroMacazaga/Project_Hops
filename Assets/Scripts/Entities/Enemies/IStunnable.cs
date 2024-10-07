namespace Entities.Enemies
{
    public interface IStunned
    {
        void Stun(float time);
        bool CanBeStunned();
        bool IsStunned();
    }
}