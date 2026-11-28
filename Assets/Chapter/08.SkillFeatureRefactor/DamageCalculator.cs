using UnityEngine;

namespace SkillFeatureStudy
{

    // 피해 계산 중재자. 피해 순서는 여기 한 곳에서만 정한다.
    // DealBaseAttack: 공격자 단계. 피해 재료를 만들고 → 공격자의 BaseAttacking 을 알리고(유니크가 IsCritical 을 켤 기회) → 치명타 배율을 한 번 곱한 뒤 → ApplyDamageContext 로 넘긴다.
    // ApplyDamageContext: 대상 단계. DamageModifying → 감소 비율 곱하기 → DamageApplying → HealthController. 공격자가 없는 피해도 이 입구로 들어온다.
    // 규칙: 사건을 먼저 알리고, 그 결과를 쓰는 계산은 알림 뒤에 한 번만 한다 (알리기 전에 계산하면 유니크가 바꾼 값이 반영되지 않는다).
    // 체력을 바꾸는 문(HealthController.ApplyDamage)은 여기서만 부른다. 다른 곳에서 바로 부르면 유니크를 건너뛴다.
    // 실습을 값으로 확인하려고 무작위 치명타는 없다. 치명타는 피해 2배.
    public static class DamageCalculator
    {

        public static DamageContext DealBaseAttack(Hero attacker,Hero target)
        {
            if (attacker == null || target == null)
            {
                return null;
            }

            DamageContext damageContext = new DamageContext(attacker.AttackPower, false, true);
            attacker.BaseAttacking.Notify(damageContext);

            if (damageContext.IsCritical)
            {
                damageContext.Damage *= damageContext.CriticalDamageMultiplier;
            }
            
            ApplyDamageContext(target,damageContext);
            return damageContext;
        }
        
        
        
        public static void ApplyDamageContext(Hero target, DamageContext damageContext)
        {
            if (target == null || damageContext == null)
            {
                return;
            }

            target.DamageModifying.Notify(damageContext);

            damageContext.Damage *= (1f - damageContext.DamageDecreaseRatio);
            
            target.DamageApplying.Notify(damageContext);
            target.Health.ApplyDamage(damageContext);
        }
        
    }
}
