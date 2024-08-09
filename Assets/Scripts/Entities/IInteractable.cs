namespace Entities
{
    public interface IInteractable
    {
        void IsHovered();
        void IsNotHovered();
        void Interact();
    }
}