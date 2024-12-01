using UnityEngine;

public class UiUpgradingController : MonoBehaviour
{
    public GameObject Item1, Item2, Item3;
    public GameObject ItemCell1, ItemCell2, ItemCell3;
    public PlayerCombat playerCombat;

    private void Update()
    {
        if (playerCombat.WeaponHolder == null)
        {
            ItemCell2.SetActive(false);
            ItemCell3.SetActive(false);
        }
        else if (playerCombat.WeaponHolder.weapon.weaponData.WeaponType == WeaponSO.Weapons.Sword)
        {
            ItemCell2.SetActive(true);
            ItemCell3.SetActive(false);
        }
        else if (playerCombat.WeaponHolder.weapon.weaponData.WeaponType == WeaponSO.Weapons.PowerGlove)
        {
            ItemCell2.SetActive(false);
            ItemCell3.SetActive(true);
        }
        
    }
    public void TurnOff(int number)
    {
        if(number == 0)
        {
            Item1.SetActive(true);
            Item2.SetActive(false);
            Item3.SetActive(false);
        }

        if (number == 1)
        {
            Item1.SetActive(false);
            Item2.SetActive(true);
            Item3.SetActive(false);
        }

        if (number == 2)
        {
            Item1.SetActive(false);
            Item2.SetActive(false);
            Item3.SetActive(true);
        }

    }
}
