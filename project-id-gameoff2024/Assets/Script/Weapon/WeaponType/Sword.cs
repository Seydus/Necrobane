using UnityEngine;

public class Sword : Weapon
{
    public override void HandleFirstAttack()
    {
        if (PlayerCombat.IsAttacking)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            PlayerCombat.PlayerProfile.isDefending = false;
            PlayerCombat.PlayerAnimation.SetSwordDefendState(false);

            PlayerCombat.IsAttacking = true;
            PlayerCombat.PlayerAnimation.PeformBasicSwordAttackAnim();
            PlayerCombat.PlayerController.maxSpeed /= 2f;
        }
    }

    public override void HandleSecondaryAttack()
    {
        if (PlayerCombat.IsAttacking)
            return;

        if (Input.GetMouseButton(1))
        {
            if (PlayerCombat.PlayerProfile.playerStamina >= weaponData.WeaponStaminaCost)
            {
                PlayerCombat.PlayerProfile.isDefending = true;

                PlayerCombat.PlayerController.maxSpeed = 2.5f;
                PlayerCombat.PlayerAnimation.PerformDefendSwordAttackAnim();
                Debug.Log("Defending with Sword");
            }
            else
            {
                PlayerCombat.PlayerController.maxSpeed = PlayerCombat.oldMaxSpeed;
                PlayerCombat.PlayerAnimation.UnPerformDefendSwordAttackAnim();
                PlayerCombat.PlayerProfile.isDefending = false;
                Debug.Log("You don't have enough mana.");
            }
        }
        else
        {
            PlayerCombat.PlayerController.maxSpeed = PlayerCombat.oldMaxSpeed;
            PlayerCombat.PlayerAnimation.UnPerformDefendSwordAttackAnim();
            PlayerCombat.PlayerProfile.isDefending = false;
        }
    }

    public override void PerformFirstAttack()
    {
        if (PlayerCombat.WeaponCheckCastInfo())
        {
            for(int i = 0; i < PlayerCombat.GetAllColliderHit().Length; i++)
            {
                if (PlayerCombat.GetAllColliderHit()[i].transform.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.15f, 0.015f, Vector3.zero)));
                    PlayerCombat.InitHitVFX(PlayerCombat.GetAllColliderHit()[i].point);

                    HandleAttack(enemy, weaponData.WeaponBasicDamage);
                    PlayHitSFX(enemy);
                }
                
                if (PlayerCombat.GetAllColliderHit()[i].transform.TryGetComponent<BossController>(out BossController boss))
                {
                    PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.15f, 0.015f, Vector3.zero)));
                    PlayerCombat.InitHitVFX(PlayerCombat.GetAllColliderHit()[i].point);

                    boss.GetComponent<BossProfile>().DeductHealth(weaponData.WeaponBasicDamage);
                    AkSoundEngine.PostEvent("Play_Chops", PlayerCombat.gameObject);
                }
            }
        }
        else
        {
            if (PlayerCombat.WeaponCheckCastInfo())
            {
                PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);
            }
        }

        PlayerCombat.PlayerController.maxSpeed = PlayerCombat.oldMaxSpeed;
    }

    public override void PerformSecondaryAttack()
    {
        Debug.Log("Secondary");
        PlayerCombat.PlayerController.maxSpeed = PlayerCombat.oldMaxSpeed;
    }

    public override void HandleAttack(Enemy enemy, float damage)
    {
        enemy.DeductHealth(damage);
        PlayerCombat.ApplyKnockback(enemy.transform.position, enemy.navMeshAgent);
        PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);
    }

    public override void FinishAttack()
    {
        PlayerCombat.PlayerProfile.isDefending = false;
        PlayerCombat.IsAttacking = false;
    }

    public override void SetAnimationLayer()
    {
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(1, 0f);
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(2, 1f);
    }
}
