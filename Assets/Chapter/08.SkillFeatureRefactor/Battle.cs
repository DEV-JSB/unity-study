namespace SkillFeatureStudy
{
    // 전투 흐름. 평타 한 번을 계산하고 대상에게 피해를 준다.
    // TBH 의 BattleManager 자리. 스테이지 시작·끝은 Stage 가 맡는다.
    // 유니크 분기도, 피해 순서도 모른다. "누가 누구를 언제 치나"와 공격 횟수 세기만 맡고, 피해는 DamageCalculator.DealBaseAttack 에 맡긴다.
    // 공격 횟수는 BaseAttacking 을 알리기 전에 올려야 한다 (평타 횟수 조건이 그 값을 읽는다).
    public class Battle
    {
        // 공격자가 대상에게 평타를 한 번 친다
        public DamageContext BaseAttack(Hero attacker, Hero target)
        {
            attacker.IncreaseAttackCount();
            return DamageCalculator.DealBaseAttack(attacker, target);
        }
    }
}
