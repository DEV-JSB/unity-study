using System;
using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 리팩토링 "전" 영웅. 장착 목록, 피해 처리, 평타 횟수.
    // 유니크 A·C 의 동작이 TakeDamage 안에 if 분기로 있다. A 사용 기록은 StartSetting 이 스테이지마다 지운다
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
        // 이미 낀 장비(같은 고유 번호)를 또 끼면 아무 일도 없다
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
            // 추 후 개선ㅇ르 할 예정이다.
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
            if (!_uniqueActivateCount.TryAdd(eUniqueType, 1))
            {
                _uniqueActivateCount[eUniqueType] += count;
            }
        }


    }
}
