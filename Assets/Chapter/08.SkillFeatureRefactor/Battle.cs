namespace SkillFeatureStudy
{
    // 리팩토링 "전" 전투 흐름. 평타 한 번을 계산하고 대상에게 피해를 준다.
    // TBH 의 BattleManager 자리. 스테이지 시작·끝은 Stage 가 맡는다.
    // 이 파일에는 유니크 분기가 없다. 유니크 B 는 DamageCalculator 안에, A·C 는 Hero.TakeDamage 안에 있다.
    public class Battle
    {
        // 공격자가 대상에게 평타를 한 번 친다
        public AttackResult BaseAttack(Hero attacker, Hero target)
        {
            AttackResult attackResult = DamageCalculator.CalculateBaseAttack(attacker);
            target.TakeDamage(attackResult.damage);
            return attackResult;
        }
    }
}
