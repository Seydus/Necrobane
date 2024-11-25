using UnityEngine;

public class Gloves : Weapon
{
    public override void HandleBasicAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerCombat.IsAttacking = true;

            PlayerCombat.PlayerController.maxSpeed /= 2f;
            PlayerCombat.PlayerAnimation.PeformBasicPunchAttackAnim();
        }
    }

    public override void HandleSuperAttack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerCombat.PlayerProfile.playerStamina >= weaponSO.WeaponStaminaCost)
            {
                PlayerCombat.IsAttacking = true;

                PlayerCombat.PlayerController.maxSpeed /= 2;
                PlayerCombat.PlayerProfile.DeductStamina(weaponSO.WeaponStaminaCost);
                PlayerCombat.PlayerAnimation.PerformSuperPunchAttackAnim();
            }
            else
            {
                Debug.Log("You don't have enough mana.");
            }
        }
    }

    public override void PerformBasicAttack()
    {
        if (PlayerCombat.WeaponCheckCastInfo())
        {
            if (PlayerCombat.sphereCastHit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.15f, 0.015f, Vector3.zero)));
                PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);

                HandleAttack(enemy, weaponSO.WeaponBasicDamage);
                
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

    public override void PerformSuperAttack()
    {
        if (PlayerCombat.WeaponCheckCastInfo())
        {
            if (PlayerCombat.sphereCastHit.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                PlayerCombat.StartCoroutine(PlayerCombat.PlayerCombatCamera.CameraShake(new CameraCombatInfo(0.20f, 0.020f, Vector3.zero)));
                PlayerCombat.InitHitVFX(PlayerCombat.sphereCastHit.point);

                HandleAttack(enemy, weaponSO.WeaponSuperAttackDamage);

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
}
