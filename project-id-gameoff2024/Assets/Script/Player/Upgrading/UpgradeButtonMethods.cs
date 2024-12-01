using UnityEngine;

public class UpgradeButtonMethods : MonoBehaviour
{
    private UiUpgradingNeeds uun;
    public PlayerCombat playerCombat;
    public PlayerProfile playerProfile;

    public string NeededItem, ExtraNeededItem;
    private int NeededItemCost, ExtraNeededItemCost;

    private void Awake()
    {
        uun = gameObject.GetComponent<UiUpgradingNeeds>();
    }

    private void Update()
    {
        NeededItemCost = uun.cost;
        ExtraNeededItemCost = uun.Extracost;

    }

    public void IncreaseDammage(int addingDammage)
    {
        bool NeedExtra = uun.IsExtra;

        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if(uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if(uun.inv.Items[i].amount >= NeededItemCost)
                        {
                            uun.lvl += 1;
                            playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponBasicDamage += addingDammage;
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
                                if (uun.inv.Items[i].amount >= NeededItemCost && uun.inv.Items[j].amount >= ExtraNeededItemCost)
                                {
                                    uun.lvl += 1;
                                    playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponBasicDamage += addingDammage;
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

    public void DecreaseStaminaDrain(int decreasingStamina)
    {
        bool NeedExtra = uun.IsExtra;
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount >= NeededItemCost)
                        {
                            uun.lvl += 1;
                            playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponStaminaCost -= decreasingStamina;
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
                                if (uun.inv.Items[i].amount >= NeededItemCost && uun.inv.Items[j].amount >= ExtraNeededItemCost)
                                {
                                    uun.lvl += 1;
                                    playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponStaminaCost -= decreasingStamina;
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

    public void IncreaseSuperAttack(int increaseDammage)
    {
        bool NeedExtra = uun.IsExtra;
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount >= NeededItemCost)
                        {
                            uun.lvl += 1;
                            playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponSuperAttackDamage += increaseDammage;
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
                                if (uun.inv.Items[i].amount >= NeededItemCost && uun.inv.Items[j].amount >= ExtraNeededItemCost)
                                {
                                    uun.lvl += 1;
                                    playerCombat.CurrentWeaponHolder.weapon.weaponData.WeaponSuperAttackDamage += increaseDammage;
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

    public void IncreaseHealth(int increaseHealth)
    {
        bool NeedExtra = uun.IsExtra;
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount >= NeededItemCost)
                        {
                            uun.lvl += 1;
                            playerProfile.profile.PlayerHealth += increaseHealth;
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
                                if (uun.inv.Items[i].amount >= NeededItemCost && uun.inv.Items[j].amount >= ExtraNeededItemCost)
                                {
                                    uun.lvl += 1;
                                    playerProfile.profile.PlayerHealth += increaseHealth;
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

    public void IncreaseStamina(int increaseStamina)
    {
        bool NeedExtra = uun.IsExtra;
        for (int i = 0; i < uun.inv.Items.Count; i++)
        {
            if (!NeedExtra)
            {
                if (uun.inv.Items[i] != null)
                {
                    if (uun.inv.Items[i].Objname == NeededItem)
                    {
                        if (uun.inv.Items[i].amount >= NeededItemCost)
                        {
                            uun.lvl += 1;
                            playerProfile.profile.PlayerStamina += increaseStamina;
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
                                if (uun.inv.Items[i].amount >= NeededItemCost && uun.inv.Items[j].amount >= ExtraNeededItemCost)
                                {
                                    uun.lvl += 1;
                                    playerProfile.profile.PlayerStamina += increaseStamina;
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
