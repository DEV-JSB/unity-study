using System.Collections.Generic;


namespace SkillFeatureStudy
{
    // 유니크 모드 관리자 (static). 두 가지를 맡는다.
    // 1. 장비 → 유니크 모드 객체: 장비를 끼면 유니크 종류로 객체를 만들어 붙이고(Attach), 빼면 뗀다(Detach)
    // 2. 사용 기록: 영웅 키 → 유니크 종류 → 이번 스테이지 사용 횟수. 스테이지가 시작되면 비운다
    // 장비가 하나도 없어도 살아 있어서, 장비를 다 뺀 사이에 스테이지가 바뀌어도 기록 비우기를 놓치지 않는다.
    // Initialize 는 게임 시작 때 한 번 부른다 (실습에서는 검사기가 그 자리를 대신한다).
    public static class UniqueModHandler
    {
        private static Dictionary<int, Dictionary<ulong, UniqueMod>> _heroEquippedUniqueMod = new();
        
        // 영웅 키 → (유니크 종류 → 이번 스테이지 사용 횟수)
        private static Dictionary<int, Dictionary<EUniqueType, int>> _uniqueActivatedHistory = new();


        public static void Initialize()
        {
            Stage.OnStageStart.Subscribe(ResetActivateHistory);
        }

        private static void ResetActivateHistory(Stage stage)
        {
            _uniqueActivatedHistory.Clear();
        }

        
        public static int GetActivateHistory(int heroKey, EUniqueType uniqueType)
        {
            _uniqueActivatedHistory.TryGetValue(heroKey, out Dictionary<EUniqueType, int> uniqueMods);
            if (uniqueMods == null)
            {
                return -1;
            }

            return uniqueMods.GetValueOrDefault(uniqueType, -1);
        }

        public static void RegisterActivateHistory(int heroKey, EUniqueType uniqueType, int count)
        {
            _uniqueActivatedHistory.TryAdd(heroKey, new Dictionary<EUniqueType, int>());
            if (!_uniqueActivatedHistory[heroKey].TryAdd(uniqueType, count))
            {
                _uniqueActivatedHistory[heroKey][uniqueType] += count;
            }
        }


        public static void UnEquipUniqueMod(int heroKey, ulong itemUniqueId)
        {
            if (_heroEquippedUniqueMod.TryGetValue(heroKey, out Dictionary<ulong, UniqueMod> uniqueMods))
            {
                if (uniqueMods.TryGetValue(itemUniqueId, out UniqueMod uniqueMod))
                {
                    uniqueMod.Detach();
                    uniqueMods.Remove(itemUniqueId);
                }
            }
        }

        public static void EquipUniqueMod(Hero hero, ulong itemUniqueId, EUniqueType uniqueType)
        {
            UniqueMod uniqueMod = null;
            
            switch (uniqueType)
            {
                case EUniqueType.SurviveLethalOncePerStage:
                    uniqueMod = new UniqueMod(hero, ETriggerType.DamageApplying,
                        new IUniqueCondition[]
                        {
                            new LethalDamageCondition(),
                            new UniqueActivationCountBelowCondition(1, EUniqueType.SurviveLethalOncePerStage)
                        }, new[] { new LeaveHpEffect(1) }, uniqueType);
                    break;
                case EUniqueType.DamageTakenReduction10:
                    uniqueMod = new UniqueMod(hero, ETriggerType.DamageModifying, null,
                        new[] { new AddDamageDecreaseRatioEffect(0.1f) }, uniqueType);
                    break;
                case EUniqueType.EveryFifthBaseAttackCritical:
                    uniqueMod = new UniqueMod(hero, ETriggerType.BaseAttacking,
                        new[] { new EveryNthBaseAttackCondition(5) }, new[] { new ForceCriticalEffect() }, uniqueType);
                    break;
            }

            if (uniqueMod == null)
            {
                return;
            }

            uniqueMod.Attach();
            _heroEquippedUniqueMod.TryAdd(hero.HeroKey, new());
            _heroEquippedUniqueMod[hero.HeroKey].TryAdd(itemUniqueId, uniqueMod);
        }
        
        
        /*public static void EquipUniqueMod(Hero hero, ulong itemUniqueId, EUniqueType uniqueType)
        {
            UniqueModBase uniqueMod = null;

            switch (uniqueType)
            {
                case EUniqueType.SurviveLethalOncePerStage:
                    uniqueMod = new SurviveLethalMod();
                    break;
                case EUniqueType.DamageTakenReduction10:
                    uniqueMod = new DamageTakenReductionMod(0.1f);
                    break;
                case EUniqueType.EveryFifthBaseAttackCritical:
                    uniqueMod = new EveryNthBaseAttackCriticalMod(5);
                    break;
            }

            if (uniqueMod == null)
            {
                return;
            }

            uniqueMod.Attach(hero);
            _heroEquippedUniqueMod.TryAdd(hero.HeroKey, new());
            _heroEquippedUniqueMod[hero.HeroKey].TryAdd(itemUniqueId, uniqueMod);
        }*/
    }
}

