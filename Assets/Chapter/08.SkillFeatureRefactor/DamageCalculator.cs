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

    // 리팩토링 "전" 피해 계산기. 지금은 공격력 그대로 피해를 낸다.
    // 실습을 값으로 확인하려고 무작위 치명타는 없다. 치명타는 피해 2배.
    public static class DamageCalculator
    {
        public const float CriticalDamageMultiplier = 2f;

        public static AttackResult CalculateBaseAttack(Hero attacker)
        {
            bool isCritical = false;

            float damage = attacker.attackPower;
            if (isCritical)
            {
                damage *= CriticalDamageMultiplier;
            }
            return new AttackResult(damage, isCritical);
        }
    }
}
