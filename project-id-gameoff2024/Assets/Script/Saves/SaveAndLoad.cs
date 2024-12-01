using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public save objInfo = new save();

    public PlayerCombat playerCombat;
    public PlayerProfile playerProfile;

    public GameObject player;
    public GameObject spawn1, spawn2, spawn3;
    public int currentLevel;

    public WeaponSO sword;
    public WeaponSO gloves;

    public UiUpgradingNeeds hpUp, stUp, sdUp, sbUp, gdUp, gsUp;

    private void Start()
    {
        Load();
    }
    public void Save()
    {
        Debug.LogError("saved");
        save_needs sn = new save_needs();

        sn.health = (int)playerProfile.profile.PlayerHealth;
        sn.stamina = (int)playerProfile.profile.PlayerStamina;

        sn.level = currentLevel;

        sn.swordDammage = (int)sword.WeaponBasicDamage;
        sn.swordBlock = (int)sword.WeaponStaminaCost;

        sn.glovesDammage = (int)gloves.WeaponBasicDamage;
        sn.glovesSupperDaammage = (int)gloves.WeaponSuperAttackDamage;

        sn.HpLvl = hpUp.lvl;
        sn.StLvl = stUp.lvl;
        sn.SwordDmLvl = sdUp.lvl;
        sn.SwordBlLvl = sbUp.lvl;
        sn.GlovesDmLvl = gdUp.lvl;
        sn.GlovesSpLvl = gsUp.lvl;

        objInfo.AllTheStuff = sn;
        string objData = JsonUtility.ToJson(objInfo);
        string FilePath = Application.persistentDataPath + "/Name.json";

        System.IO.File.WriteAllText(FilePath, objData);
        Load();
    }

    public void Load()
    {
        string FilePath = Application.persistentDataPath + "/Name.json";
        string objData = System.IO.File.ReadAllText(FilePath);

        objInfo = JsonUtility.FromJson<save>(objData);

        playerProfile.profile.PlayerHealth = objInfo.AllTheStuff.health;
        playerProfile.profile.PlayerStamina = objInfo.AllTheStuff.stamina;

        sword.WeaponBasicDamage = objInfo.AllTheStuff.swordDammage;
        sword.WeaponStaminaCost = objInfo.AllTheStuff.swordBlock;

        gloves.WeaponBasicDamage = objInfo.AllTheStuff.glovesDammage;
        gloves.WeaponSuperAttackDamage = objInfo.AllTheStuff.glovesSupperDaammage;

        hpUp.lvl = objInfo.AllTheStuff.HpLvl;
        stUp.lvl = objInfo.AllTheStuff.StLvl;

        sdUp.lvl = objInfo.AllTheStuff.SwordDmLvl;
        sbUp.lvl = objInfo.AllTheStuff.SwordBlLvl;

        gdUp.lvl = objInfo.AllTheStuff.GlovesDmLvl;
        gsUp.lvl = objInfo.AllTheStuff.GlovesSpLvl;

        currentLevel = objInfo.AllTheStuff.level;

        if (currentLevel == 1)
        {
            player.transform.position = spawn1.transform.position;
        }

        if (currentLevel == 2)
        {
            player.transform.position = spawn2.transform.position;
        }

        if (currentLevel == 3)
        {
            player.transform.position = spawn3.transform.position;
        }
    }

    [System.Serializable]
    public class save
    {
        public save_needs AllTheStuff = new save_needs();
    }

    [System.Serializable]
    public class save_needs
    {
        public int level = 1, metalAmount = 0, bonesAmaount = 0, health = 100, stamina = 100, swordDammage = 40, swordBlock = 20, glovesDammage = 30, glovesSupperDaammage = 200;
        public int HpLvl = 1, StLvl = 1, SwordDmLvl = 1, SwordBlLvl = 1, GlovesDmLvl = 1, GlovesSpLvl = 1;
    }
}
