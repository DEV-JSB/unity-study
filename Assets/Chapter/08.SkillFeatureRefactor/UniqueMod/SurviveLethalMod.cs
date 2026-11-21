using UnityEngine;



namespace SkillFeatureStudy
{
    public class SurviveLethalMod : UniqueModBase
    {
        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.DamageApplying.Unsubscribe(ModifyDamage);
        }


        private void ModifyDamage(DamageContext context)
        {
            if (_ownerHero.CurrentHp - context.Damage <= 0 &&
                _ownerHero.GetUniqueActivatedCount(EUniqueType.SurviveLethalOncePerStage) == 0)
            {
                context.Damage = _ownerHero.CurrentHp - 1f;
                _ownerHero.IncreaseUniqueActivateCount(EUniqueType.SurviveLethalOncePerStage, 1);
            }
        }
        
        protected override void SubscribeEvents(Hero hero)
        {
            hero.DamageApplying.Subscribe(ModifyDamage);
        }
        
        
        
    }

    
}

