using UnityEngine;


namespace SkillFeatureStudy
{
    public class EveryNthBaseAttackCriticalMod : UniqueModBase
    {
        private readonly int _n;

        public EveryNthBaseAttackCriticalMod(int n)
        {
            _n = n;
        }
        
        
        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.BaseAttacking.Unsubscribe(SetBaseAttackContext);
        }

        private void SetBaseAttackContext(BaseAttackContext context)
        {
            if (_ownerHero.AttackCount % _n == 0)
            {
                context.IsCritical = true;
            }
        }

        protected override void SubscribeEvents(Hero hero)
        {
            hero.BaseAttacking.Subscribe(SetBaseAttackContext);
        }
    }
}

