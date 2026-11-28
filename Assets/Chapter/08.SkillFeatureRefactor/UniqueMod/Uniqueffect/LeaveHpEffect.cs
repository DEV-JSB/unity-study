using UnityEngine;

namespace SkillFeatureStudy
{
    public class LeaveHpEffect : IUniqueEffect
    {
        private readonly float _leaveHp;


        public LeaveHpEffect(float leaveHp)
        {
            _leaveHp = leaveHp;
        }
        
        public void Execute(Hero ownerHero, TriggerContext context)
        {
            DamageContext damageContext = (DamageContext)context;
            if (damageContext == null)
            {
                return;
            }
            damageContext.Damage = ownerHero.CurrentHp - _leaveHp;
        }
    }

}

