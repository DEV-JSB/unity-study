using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 영웅. 낀 장비, 체력과 피해 받기, 평타 횟수.
    // 유니크 동작은 Hero 에 없다. 장비를 끼면 장비에 달린 유니크 모드 객체(UniqueMod 폴더)가 만들어져
    // Hero 의 사건(DamageModifying, DamageApplying, BaseAttacking)을 구독하고, Hero 는 알리기만 한다.
    // 유니크 사용 기록은 Hero 가 유니크 종류별로 들고 있고, 스테이지가 시작되면(StartSetting) 지운다
    // (규칙은 EUniqueType.cs, 정답은 검사기).
    public class Hero
    {
        public readonly string heroName;
        public readonly float attackPower;
        private int _attackCount = 0;
        public int AttackCount => _attackCount;
        
        
        // 장비 고유 번호 → 낀 장비. 장비 하나에 유니크 모드가 하나 달려 있다
        private readonly Dictionary<ulong, Gear> _equippedGearsByUniqueId = new ();

        private readonly Dictionary<ulong, UniqueModBase> _equippedUniqueMod = new ();
        private Dictionary<EUniqueType, int> _uniqueActivateCount = new ();

        public readonly Observer<DamageContext> DamageModifying = new ();
        public readonly Observer<DamageContext> DamageApplying = new ();
        public readonly Observer<BaseAttackContext> BaseAttacking = new();
        
        public float CurrentHp { get; private set; }

        public int GetUniqueActivatedCount(EUniqueType eUniqueType)
        {
            return _uniqueActivateCount.GetValueOrDefault(eUniqueType);
        }

        public void IncreaseAttackCount()
        {
            _attackCount++;
        }
        
        public bool IsDead
        {
            get { return CurrentHp <= 0f; }
        }

        public Hero(string heroName, float startHp, float attackPower)
        {
            this.heroName = heroName;
            this.attackPower = attackPower;
            CurrentHp = startHp;
        }

        public void StartSetting()
        {
            _uniqueActivateCount.Clear();
        }
        
        // 장비 하나를 낀다. 같은 유니크가 달린 장비를 두 개 끼면 (고유 번호가 다르니) 두 개 다 들어간다.
        // 같은 장비(같은 고유 번호)는 두 번 낄 수 없다는 게 게임 규칙이라, 들어오면 버그 방어로 무시한다
        public void Equip(Gear gear)
        {
            if (!_equippedGearsByUniqueId.TryAdd(gear.UniqueId, gear))
            {
                return;
            }

            UniqueModBase uniqueMod = null;
            switch (gear.UniqueType)
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

            uniqueMod.Attach(this);
            _equippedUniqueMod.Add(gear.UniqueId, uniqueMod);
        }

        // 장비 고유 번호로 장비 하나를 뺀다. 끼지 않은 번호면 아무 일도 없다
        public void Unequip(ulong gearUniqueId)
        {
            _equippedGearsByUniqueId.Remove(gearUniqueId);

            if (_equippedUniqueMod.TryGetValue(gearUniqueId, out UniqueModBase uniqueMod))
            {
                uniqueMod.Detach();
                _equippedUniqueMod.Remove(gearUniqueId);
            }
        }


        // 피해를 받는다. 체력이 0 이하가 되면 0으로 두고 쓰러진다
        public void TakeDamage(float incomingDamage)
        {
            if (IsDead)
            {
                return;
            }
            
            DamageContext damageContext = new DamageContext(incomingDamage);
            DamageModifying.Notify(damageContext);
            
            
            // TakeDamage 함수에서 왜 DamageDecreaseRatio 를 해야하는가? 맞지 않는 설계 방식이다
            // 추후 개선을 할 예정이다.
            damageContext.Damage *= (1f - damageContext.DamageDecreaseRatio);
            DamageApplying.Notify(damageContext);
            
            if (CurrentHp - damageContext.Damage > 0f)
            {
                CurrentHp -= damageContext.Damage;
                return;
            }

            CurrentHp = 0f;
        }

        public void IncreaseUniqueActivateCount(EUniqueType eUniqueType, int count)
        {
            if (!_uniqueActivateCount.TryAdd(eUniqueType, count))
            {
                _uniqueActivateCount[eUniqueType] += count;
            }
        }


    }
}
