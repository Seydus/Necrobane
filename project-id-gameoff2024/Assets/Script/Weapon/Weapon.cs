using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponSO weaponSO;

    public PlayerCombat PlayerCombat { get; set; }
    public abstract void SetAnimationLayer();
    public abstract void HandleAttack(Enemy enemy, float damage);
    public abstract void HandleFirstAttack();
    public abstract void HandleSecondaryAttack();
    public abstract void PerformFirstAttack();
    public abstract void PerformSecondaryAttack();
    public abstract void FinishAttack();
}
