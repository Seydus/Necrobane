using UnityEngine;

public class Gloves : Weapon
{

    public override void HandleFirstAttack()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    PlayerCombat.IsAttacking = true;

        //    PlayerCombat.PlayerController.maxSpeed /= 2f;
        //    PlayerCombat.PlayerAnimation.PeformBasicPunchAttackAnim();
        //    Debug.Log("FIRST ATTACK");
        //}
    }

    public override void HandleSecondaryAttack()
    {
        if (PlayerCombat.IsAttacking)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerCombat.PlayerProfile.playerStamina >= weaponData.WeaponStaminaCost)
            {
                PlayerCombat.IsAttacking = true;

                PlayerCombat.PlayerController.maxSpeed /= 2;
                PlayerCombat.PlayerProfile.DeductStamina(weaponData.WeaponStaminaCost);
                PlayerCombat.PlayerAnimation.PerformSuperPunchAttackAnim();
            }
            else
            {
                Debug.Log("You don't have enough mana.");
            }
        }
    }

    public override void PerformFirstAttack()
    {
        if (PlayerCombat.WeaponCheckCastInfo())
        {
            if (PlayerCombat.sphereCastHit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.15f, 0.015f, Vector3.zero)));
                PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);

                HandleAttack(enemy, weaponData.WeaponBasicDamage);
                
                AkSoundEngine.PostEvent("Play_HitBones", PlayerCombat.gameObject);
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
        if (PlayerCombat.WeaponCheckCastInfo())
        {
            if (PlayerCombat.sphereCastHit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.20f, 0.020f, Vector3.zero)));
                PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);

                HandleAttack(enemy, weaponData.WeaponSuperAttackDamage);

                AkSoundEngine.PostEvent("Play_HitBones", PlayerCombat.gameObject);
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

    public override void HandleAttack(Enemy enemy, float damage)
    {
        enemy.DeductHealth(damage);
        PlayerCombat.ApplyKnockback(enemy.transform.position, enemy.navMeshAgent);
        PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);
    }

    public override void FinishAttack()
    {
        PlayerCombat.IsAttacking = false;
    }

    public override void SetAnimationLayer()
    {
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(1, 1f);
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(2, 0f);
    }
}
