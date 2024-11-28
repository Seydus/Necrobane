using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponSO weaponSO;
    public WeaponData weaponData { get; set; }
    public PlayerCombat PlayerCombat { get; set; }
    public abstract void SetAnimationLayer();
    public abstract void HandleAttack(Enemy enemy, float damage);
    public abstract void HandleFirstAttack();
    public abstract void HandleSecondaryAttack();
    public abstract void PerformFirstAttack();
    public abstract void PerformSecondaryAttack();
    public abstract void FinishAttack();

    private void Start()
    {
        weaponData = new WeaponData();
        weaponData.WeaponType = weaponSO.WeaponType;
        weaponData.WeaponName = weaponSO.WeaponName;
        weaponData.WeaponBasicDamage = weaponSO.WeaponBasicDamage;
        weaponData.WeaponStaminaCost = weaponSO.WeaponStaminaCost;
        weaponData.WeaponSuperAttackDamage = weaponSO.WeaponSuperAttackDamage;
        weaponData.WeaponStunDuration = weaponSO.WeaponStunDuration;
        weaponData.WeaponBlockDuration = weaponSO.WeaponBlockDuration;
    }
}
