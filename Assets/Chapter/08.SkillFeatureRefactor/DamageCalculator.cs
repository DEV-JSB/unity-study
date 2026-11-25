using UnityEngine;

namespace SkillFeatureStudy
{
    // 평타 한 번의 결과: 준 피해와 치명타였는지
    public class AttackResult
    {
        public readonly float damage;
        public readonly bool isCritical;

        public AttackResult(float damage, bool isCritical)
        {
            this.damage = damage;
            this.isCritical = isCritical;
        }
    }

    // 피해 계산 중재자. 평타 피해를 계산하고(CalculateBaseAttack), 계산한 피해를 대상에게 넣는다(ApplyAttackResult).
    // 피해를 넣을 때 대상의 사건을 순서대로 울린다: DamageModifying → 감소 비율 곱하기 → DamageApplying → HealthController.
    // 체력을 바꾸는 문(HealthController.ApplyDamage)은 여기서만 부른다. 다른 곳에서 바로 부르면 유니크를 건너뛴다.
    // 실습을 값으로 확인하려고 무작위 치명타는 없다. 치명타는 피해 2배.
    public static class DamageCalculator
    {
        public const float CriticalDamageMultiplier = 2f;

        
        public static AttackResult CalculateBaseAttack(Hero attacker, bool isCritical)
        {
            float damage = attacker.attackPower;

            if (isCritical)
            {
                damage *= CriticalDamageMultiplier;
            }
            return new AttackResult(damage, isCritical);
        }

        public static void ApplyAttackResult(Hero target, AttackResult damage)
        {
            if (target == null || damage == null)
            {
                return;
            }

            DamageContext damageContext = new DamageContext(damage.damage);
            target.DamageModifying.Notify(damageContext);

            damageContext.Damage *= (1f - damageContext.DamageDecreaseRatio);
            target.DamageApplying.Notify(damageContext);
            
            target.Health.ApplyDamage(damageContext);
        }
        
    }
}
