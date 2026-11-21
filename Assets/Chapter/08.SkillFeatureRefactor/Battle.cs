namespace SkillFeatureStudy
{
    // 리팩토링 "전" 전투 흐름. 평타 한 번을 계산하고 대상에게 피해를 준다.
    // TBH 의 BattleManager 자리. 스테이지 시작·끝은 Stage 가 맡는다.
    // 유니크 B(다섯 번째 평타 치명타) 분기가 BaseAttack 안에 있다. A·C 는 Hero.TakeDamage 안에 있다.
    public class Battle
    {
        // 공격자가 대상에게 평타를 한 번 친다
        public AttackResult BaseAttack(Hero attacker, Hero target)
        {
            bool criticalAttack = false;
            attacker.IncreaseAttackCount();
            BaseAttackContext context = new(false);
            attacker.BaseAttacking.Notify(context);
            AttackResult attackResult = DamageCalculator.CalculateBaseAttack(attacker, context.IsCritical);
            target.TakeDamage(attackResult.damage);
            return attackResult;
        }
    }
}
