namespace Weapons
{
    public interface IWeapon
    {
        void Action(WeaponAction action);
    }

    public enum WeaponAction
    {
        StartPrimaryAttack,
        StartSecondaryAttack,
        StartReload,
        HoldPrimaryAttack,
        HoldSecondaryAttack,
        HoldReload,
        ReleasePrimaryAttack,
        ReleaseSecondaryAttack,
        ReleaseReload,
    }
}
