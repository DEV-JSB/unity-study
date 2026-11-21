using UnityEngine;

namespace SkillFeatureStudy
{
    public class DamageTakenReductionMod : UniqueModBase
    {

        private readonly float _damageTakenReductionRatio;
        public DamageTakenReductionMod(float damageReductionRatioRatio)
        {
            _damageTakenReductionRatio = damageReductionRatioRatio;
        }
        
        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.DamageModifying.Unsubscribe(ReductionDamageContext);
        }

        protected override void SubscribeEvents(Hero hero)
        {
            hero.DamageModifying.Subscribe(ReductionDamageContext);
        }

        private void ReductionDamageContext(DamageContext damageContext)
        {
            damageContext.DamageDecreaseRatio += _damageTakenReductionRatio;
        }
    }
}
