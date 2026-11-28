using UnityEngine;

namespace SkillFeatureStudy
{
    public class AddDamageDecreaseRatioEffect : IUniqueEffect
    {
        private readonly float _decreaseRatio;

        public AddDamageDecreaseRatioEffect(float decreaseRatio)
        {
            _decreaseRatio = decreaseRatio;
        }
        
        public void Execute(Hero ownerHero, TriggerContext context)
        {
            DamageContext damageContext = (DamageContext)context;
            if (damageContext == null)
            {
                return;
            }

            damageContext.DamageDecreaseRatio += _decreaseRatio;
        }
    }

}
