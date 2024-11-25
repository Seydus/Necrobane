using UnityEngine;

public class Attacking : MonoBehaviour
{
    public PlayerAnimation plan;

    public int weapon;

    public GameObject[] Sword;
    public GameObject[] Gloves;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        plan.anim.SetInteger("Weapon", weapon);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon = 0;

            for (int i = 0; i < Sword.Length; i++)
            {
                Sword[i].SetActive(true);
            }

            for (int i = 0; i < Gloves.Length; i++)
            {
                Gloves[i].SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon = 1;

            for (int i = 0; i < Sword.Length; i++)
            {
                Sword[i].SetActive(false);
            }

            for (int i = 0; i < Gloves.Length; i++)
            {
                Gloves[i].SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (weapon == 0)
            {
                plan.PeformBasicSwordAttackAnim();
            }

            if(weapon == 1)
            {
                plan.PeformBasicPunchAttackAnim();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (weapon == 0)
            {
                plan.PerformDefendSwordAnim();
            }

            if (weapon == 1)
            {
                plan.PerformSuperPunchAttackAnim();
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            if (weapon == 0)
            {
                plan.UnPerformDefendSwordAnim();
            }
        }
    }
}
