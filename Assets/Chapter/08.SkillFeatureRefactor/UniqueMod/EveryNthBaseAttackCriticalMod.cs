namespace SkillFeatureStudy
{
    // N번째 평타마다 치명타. 평타 피해를 계산하기 전(BaseAttacking)에 자기 영웅의 평타 횟수를 보고 치명타를 켠다.
    // N은 생성자로 받는다: 다섯 번째마다면 5, 세 번째마다면 3. 같은 class 로 여러 유니크를 만든다.
    public class EveryNthBaseAttackCriticalMod : UniqueModBase
    {
        // 몇 번째 평타마다 치명타인가
        private readonly int _criticalAttackInterval;

        public EveryNthBaseAttackCriticalMod(int criticalAttackInterval)
        {
            _criticalAttackInterval = criticalAttackInterval;
        }

        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.BaseAttacking.Unsubscribe(OnBaseAttacking);
        }

        protected override void SubscribeEvents(Hero hero)
        {
            hero.BaseAttacking.Subscribe(OnBaseAttacking);
        }

        private void OnBaseAttacking(BaseAttackContext context)
        {
            if (_ownerHero.AttackCount % _criticalAttackInterval == 0)
            {
                context.IsCritical = true;
            }
        }
    }
}
