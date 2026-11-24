using System.Collections.Generic;


namespace SkillFeatureStudy
{
    public static class UniqueModHandler
    {
        private static Dictionary<int, Dictionary<ulong, UniqueModBase>> _heroEquippedUniqueMod = new();
        
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
            if (_heroEquippedUniqueMod.TryGetValue(heroKey, out Dictionary<ulong, UniqueModBase> uniqueMods))
            {
                if (uniqueMods.TryGetValue(itemUniqueId, out UniqueModBase uniqueMod))
                {
                    uniqueMod.Detach();
                    uniqueMods.Remove(itemUniqueId);
                }
            }
        }

        public static void EquipUniqueMod(Hero hero, ulong itemUniqueId, EUniqueType uniqueType)
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
        }
    }
}

