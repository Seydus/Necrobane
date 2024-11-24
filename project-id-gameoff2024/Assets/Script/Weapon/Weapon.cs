using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponSO weaponSO;

    public PlayerCombat PlayerCombat { get; set; }
    public abstract void HandleAttack(Enemy enemy, float damage);
    public abstract void HandleBasicAttack();
    public abstract void HandleSuperAttack();
    public abstract void PerformBasicAttack();
    public abstract void PerformSuperAttack();
    public abstract void FinishAttack();
}
