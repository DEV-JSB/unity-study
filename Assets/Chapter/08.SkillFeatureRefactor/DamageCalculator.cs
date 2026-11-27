using UnityEngine;

namespace SkillFeatureStudy
{

    // 피해 계산 중재자. 평타 피해를 계산하고(CalculateBaseAttack), 계산한 피해를 대상에게 넣는다(ApplyAttackResult).
    // 피해를 넣을 때 대상의 사건을 순서대로 울린다: DamageModifying → 감소 비율 곱하기 → DamageApplying → HealthController.
    // 체력을 바꾸는 문(HealthController.ApplyDamage)은 여기서만 부른다. 다른 곳에서 바로 부르면 유니크를 건너뛴다.
    // 실습을 값으로 확인하려고 무작위 치명타는 없다. 치명타는 피해 2배.
    public static class DamageCalculator
    {
        public static DamageContext GetDamageContext(Hero hero, bool isCritical, bool isBaseAttack)
        {
            DamageContext baseAttackContext = new(hero.AttackPower, isCritical, isBaseAttack);

            if (isBaseAttack)
            {
                hero.BaseAttacking.Notify(baseAttackContext);
            }

            if (baseAttackContext.IsCritical)
            {
                baseAttackContext.Damage *= baseAttackContext.CriticalDamageMultiplier;
            }
            
            return baseAttackContext;
        }
        

        public static void ApplyDamageContext(Hero target, DamageContext damage)
        {
            if (target == null || damage == null)
            {
                return;
            }

            target.DamageModifying.Notify(damage);

            damage.Damage *= (1f - damage.DamageDecreaseRatio);
            target.DamageApplying.Notify(damage);
            
            target.Health.ApplyDamage(damage);
        }
        
    }
}
