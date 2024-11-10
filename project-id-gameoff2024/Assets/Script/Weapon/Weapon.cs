using UnityEngine;

[CreateAssetMenu(fileName = "WeaponType", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
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
    public float WeaponDamage;
}
