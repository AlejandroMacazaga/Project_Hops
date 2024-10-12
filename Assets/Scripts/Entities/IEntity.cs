namespace Entities
{
    public interface IEntity
    {
        EntityTeam GetTeam();
    }
    
    public enum EntityTeam
    {
        Player,
        Ally,
        Enemy,
        Environment
    }
}