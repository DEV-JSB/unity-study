using SkillFeatureStudy;
using UnityEngine;

namespace SkillFeatureStudy
{
    
    public class ForceCriticalEffect : IUniqueEffect
    {
        public void Execute(Hero ownerHero, TriggerContext context)
        {
            DamageContext damageContext = (DamageContext)context;
            if (damageContext == null)
            {
                return;
            }
            damageContext.IsCritical = true;
        }
    }

    
}
