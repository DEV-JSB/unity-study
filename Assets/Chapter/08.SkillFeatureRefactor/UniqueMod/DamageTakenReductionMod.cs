namespace SkillFeatureStudy
{
    // 받는 피해 감소. 피해 값 고치기 단계(DamageModifying)에서 자기 감소 비율을 DamageContext 에 더하기만 한다.
    // 장비 두 개면 객체도 두 개라 0.1 + 0.1 = 0.2 가 되고, Hero.TakeDamage 가 한 번에 적용한다 (곱하면 0.81 이 돼서 틀림).
    public class DamageTakenReductionMod : UniqueModBase
    {
        private readonly float _damageTakenReductionRatio;

        public DamageTakenReductionMod(float damageTakenReductionRatio)
        {
            _damageTakenReductionRatio = damageTakenReductionRatio;
        }

        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.DamageModifying.Unsubscribe(OnDamageModifying);
        }

        protected override void SubscribeEvents(Hero hero)
        {
            hero.DamageModifying.Subscribe(OnDamageModifying);
        }

        private void OnDamageModifying(DamageContext damageContext)
        {
            damageContext.DamageDecreaseRatio += _damageTakenReductionRatio;
        }
    }
}
