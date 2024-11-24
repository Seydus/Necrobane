using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/WeaponType")]
public class WeaponSO : ScriptableObject
{
    public enum Weapons
    {
        PowerGlove,
        Sword,
        Dagger,
        Axe
    };

    public Weapons WeaponType = Weapons.PowerGlove;

    [Space]

    public string WeaponName;
    public float WeaponBasicDamage;
    public float WeaponStaminaCost;
    public float WeaponSuperAttackDamage;
    public float WeaponStunDuration;
    public float WeaponBlockDuration;
}
