using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 치명상 버티기. 피해 결과 판정 단계(DamageApplying)에서, 이미 줄어든 피해로 쓰러질지 본다.
    // 쓰러질 피해이고 이번 스테이지에 안 썼으면 체력 1이 남도록 피해를 고치고, 영웅의 사용 기록에 적는다.
    // 사용 기록은 이 객체가 아니라 Hero 에 있다: 장비를 뺐다 끼면 이 객체는 새로 만들어지기 때문 (빼고 끼기 어뷰징 방지).
    public class SurviveLethalMod : UniqueModBase
    {
        private static Dictionary<int, int> _activateHistory = new();


        public override void Detach()
        {
            base.Detach();
            Stage.OnStageStart.Unsubscribe(Reset);
        }
        
        private void Reset(Stage stage)
        {
            _activateHistory.Clear();
        }

        public override void Attach(Hero hero)
        {
            base.Attach(hero);
            Stage.OnStageStart.Subscribe(Reset);
        }
        
        

        protected override void UnsubscribeEvents(Hero hero)
        {
            hero.DamageApplying.Unsubscribe(OnDamageApplying);
        }

        protected override void SubscribeEvents(Hero hero)
        {
            hero.DamageApplying.Subscribe(OnDamageApplying);
        }

        private void OnDamageApplying(DamageContext context)
        {
            if (!_activateHistory.TryGetValue(_ownerHero.HeroKey, out int count) || count <= 0)
            {
                if (_ownerHero.CurrentHp - context.Damage <= 0 &&
                    _ownerHero.GetUniqueActivatedCount(EUniqueType.SurviveLethalOncePerStage) == 0)
                {
                    context.Damage = _ownerHero.CurrentHp - 1f;
                    _ownerHero.IncreaseUniqueActivateCount(EUniqueType.SurviveLethalOncePerStage, 1);
                    if (!_activateHistory.TryAdd(_ownerHero.HeroKey, 1))
                    {
                        _activateHistory[_ownerHero.HeroKey]++;
                    }
                }

            }
            
        }
    }
}
