using UnityEngine;

public class Sword : Weapon
{
    public override void HandleFirstAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attacking with sword.");
            //PlayerCombat.IsAttacking = true;

            //PlayerCombat.PlayerController.maxSpeed /= 2f;
            //PlayerCombat.PlayerAnimation.PeformBasicSwordAttackAnim();
        }
    }

    public override void HandleSecondaryAttack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerCombat.PlayerProfile.playerStamina >= weaponSO.WeaponStaminaCost)
            {
                PlayerCombat.PlayerProfile.isDefending = true;
                // PlayerCombat.IsAttacking = true;

                // PlayerCombat.PlayerController.maxSpeed /= 2;
                PlayerCombat.PlayerProfile.DeductStamina(weaponSO.WeaponStaminaCost);
                // PlayerCombat.PlayerAnimation.PerformDefendSwordAttackAnim();
                Debug.Log("Defending with Sword");
            }
            else
            {
                PlayerCombat.PlayerProfile.isDefending = false;
                Debug.Log("You don't have enough mana.");
            }
        }
        else
        {
            PlayerCombat.PlayerProfile.isDefending = false;
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

    public override void PerformSecondaryAttack()
    {

        Debug.Log("Secondary");
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
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(1, 0f);
        PlayerCombat.PlayerAnimation.anim.SetLayerWeight(2, 1f);
    }
}
