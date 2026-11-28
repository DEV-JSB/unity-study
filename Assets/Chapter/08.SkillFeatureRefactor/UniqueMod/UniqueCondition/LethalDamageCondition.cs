using UnityEngine;

namespace SkillFeatureStudy
{
    public class LethalDamageCondition : IUniqueCondition
    {
        public bool IsSatisfied(Hero hero, TriggerContext context)
        {
            DamageContext damageContext = (DamageContext)context;
            if (damageContext == null)
            {
                return false;
            }
            if (hero.CurrentHp - damageContext.Damage <= 0)
            {
                return true;
            }
            return false;
        }
    }

}
