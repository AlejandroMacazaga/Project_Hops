namespace Weapons
{
    public interface IWeapon
    {
        void Action(WeaponAction action);
    }

    public enum WeaponAction
    {
        TapPrimaryAttack,
        TapSecondaryAttack,
        TapReload,
        HoldPrimaryAttack,
        HoldSecondaryAttack,
        HoldReload,
        ReleasePrimaryAttack,
        ReleaseSecondaryAttack,
        ReleaseReload,
    }
}
