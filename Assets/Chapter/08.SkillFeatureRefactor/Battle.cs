namespace SkillFeatureStudy
{
    // 전투 흐름. 평타 한 번을 계산하고 대상에게 피해를 준다.
    // TBH 의 BattleManager 자리. 스테이지 시작·끝은 Stage 가 맡는다.
    // 유니크 분기는 없다. 평타 계산 전에 공격자의 BaseAttacking 을 알리고, 구독한 유니크 모드가 치명타 여부를 고친다.
    public class Battle
    {
        // 공격자가 대상에게 평타를 한 번 친다
        public AttackResult BaseAttack(Hero attacker, Hero target)
        {
            attacker.IncreaseAttackCount();
            BaseAttackContext baseAttackContext = new(false);
            attacker.BaseAttacking.Notify(baseAttackContext);
            AttackResult attackResult = DamageCalculator.CalculateBaseAttack(attacker, baseAttackContext.IsCritical);
            target.TakeDamage(attackResult.damage);
            return attackResult;
        }
    }
}
