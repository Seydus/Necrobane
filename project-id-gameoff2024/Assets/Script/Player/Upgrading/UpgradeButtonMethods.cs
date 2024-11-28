using UnityEngine;

public class UpgradeButtonMethods : MonoBehaviour
{
    private UiUpgradingNeeds uun;
    private PlayerCombat playerCombat;

    private void Awake()
    {
        uun = GetComponent<UiUpgradingNeeds>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (playerCombat.WeaponHolder == null)
            return;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            IncreaseDammage("Metal shards", null, 1, 0, false, 20);
        }
    }

    public void IncreaseDammage(string NeededItem, string ExtraNeededItem, int NeededItemCost, int ExtraNeededItemCost, bool NeedExtra, int addingDammage)
    {
        for(int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if(uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if(uun.inv.Items[i].amount == NeededItemCost)
                        {
                            playerCombat.WeaponHolder.weapon.weaponSO.WeaponBasicDamage += addingDammage;
                            uun.inv.Items[i].amount -= NeededItemCost;
                        }
                    }
                }
            }

            if(NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    for(int j = 0; j < uun.inv.Items.Count; i++)
                    {
                        if(uun.inv.Items[j] != null)
                        {
                            if (uun.inv.Items[i].Objname == NeededItem && uun.inv.Items[j].Objname == ExtraNeededItem)
                            {
                                if (uun.inv.Items[i].amount == NeededItemCost && uun.inv.Items[j].amount == ExtraNeededItemCost)
                                {
                                    playerCombat.WeaponHolder.weapon.weaponSO.WeaponBasicDamage += addingDammage;
                                    uun.inv.Items[i].amount -= NeededItemCost;
                                    uun.inv.Items[j].amount -= ExtraNeededItemCost;
                                }
                            }
                        }
                    }

                }
            }
        }
    }


    public void DecreaseStaminaDrain(string NeededItem, string ExtraNeededItem, int NeededItemCost, int ExtraNeededItemCost, bool NeedExtra, int decreasingStamina)
    {
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount == NeededItemCost)
                        {
                            playerCombat.WeaponHolder.weapon.weaponSO.WeaponStaminaCost -= decreasingStamina;
                            uun.inv.Items[i].amount -= NeededItemCost;
                        }
                    }
                }
            }

            if (NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    for (int j = 0; j < uun.inv.Items.Count; i++)
                    {
                        if (uun.inv.Items[j] != null)
                        {
                            if (uun.inv.Items[i].Objname == NeededItem && uun.inv.Items[j].Objname == ExtraNeededItem)
                            {
                                if (uun.inv.Items[i].amount == NeededItemCost && uun.inv.Items[j].amount == ExtraNeededItemCost)
                                {
                                    playerCombat.WeaponHolder.weapon.weaponSO.WeaponStaminaCost -= decreasingStamina;
                                    uun.inv.Items[i].amount -= NeededItemCost;
                                    uun.inv.Items[j].amount -= ExtraNeededItemCost;
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public void IncreaseSuperAttack(string NeededItem, string ExtraNeededItem, int NeededItemCost, int ExtraNeededItemCost, bool NeedExtra, int increaseDammage)
    {
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount == NeededItemCost)
                        {
                            playerCombat.WeaponHolder.weapon.weaponSO.WeaponSuperAttackDamage += increaseDammage;
                            uun.inv.Items[i].amount -= NeededItemCost;
                        }
                    }
                }
            }

            if (NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    for (int j = 0; j < uun.inv.Items.Count; i++)
                    {
                        if (uun.inv.Items[j] != null)
                        {
                            if (uun.inv.Items[i].Objname == NeededItem && uun.inv.Items[j].Objname == ExtraNeededItem)
                            {
                                if (uun.inv.Items[i].amount == NeededItemCost && uun.inv.Items[j].amount == ExtraNeededItemCost)
                                {
                                    playerCombat.WeaponHolder.weapon.weaponSO.WeaponSuperAttackDamage += increaseDammage;
                                    uun.inv.Items[i].amount -= NeededItemCost;
                                    uun.inv.Items[j].amount -= ExtraNeededItemCost;
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
