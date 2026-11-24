using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SkillFeatureStudy
{
    // 유니크 세 개(A, B, C)의 동작을 시나리오로 돌려서 정답 값과 비교한다.
    // 사용법: 빈 GameObject에 붙이고 Play 하거나, Play 없이 컴포넌트 메뉴의 "유니크 동작 검사 실행"을 누른다.
    //
    // 이 검사기의 역할: 리팩토링 전후 동작이 같은지 확인하는 안전망 (특성 테스트, characterization test).
    //   리팩토링은 동작은 그대로 두고 구조만 바꾸는 것이다. 그래서 리팩토링 뒤에도 이 파일은 고치지 않고,
    //   전부 성공으로 나와야 한다.
    //
    // 검사기가 부르는 바깥 버튼 — 리팩토링 뒤에도 이 이름과 모양은 남겨야 한다
    //   new Hero(heroName, startHp, attackPower), new Gear(uniqueId, uniqueType),
    //   hero.Equip(gear), hero.Unequip(gearUniqueId), DamageCalculator.ApplyAttackResult(target, attackResult),
    //   hero.CurrentHp, hero.IsDead, new Stage(), stage.StartStage,
    //   new Battle(), battle.BaseAttack, AttackResult.isCritical
    //
    // 장비 기준으로 바뀐 이유: 유니크 모드는 장비에 달려 있고, 장비는 고유 번호(ulong)로 구분한다 (TBH 와 같은 가정).
    //   같은 유니크가 달린 장비를 두 개 껴도 뺄 때 고유 번호로 어느 장비인지 정확히 고른다.
    //   이 변경 때 바뀐 것은 부르는 모양뿐이고, 각 시나리오의 정답 값은 그대로다.
    //
    // Console에는 시나리오 하나 = 로그 하나. 성공은 일반 로그, 실패는 빨간 에러. 클릭하면 단계별 값이 나온다.
    public class UniqueBehaviourChecker : MonoBehaviour
    {
        private const string SuccessWord = "<color=#5DC264>성공</color>";
        private const string FailWord = "<color=#FF6B6B>실패</color>";
        private const float HpTolerance = 0.001f;

        // 시나리오 하나의 단계별 기록. 단계마다 정답과 실제 값을 한 줄씩 남긴다
        private class ScenarioRecord
        {
            public readonly string scenarioTitle;
            public readonly StringBuilder stepLines = new StringBuilder();
            public int stepCount;
            public int failedStepCount;

            public ScenarioRecord(string scenarioTitle)
            {
                this.scenarioTitle = scenarioTitle;
            }

            public void ExpectHp(string stepDescription, Hero hero, float expectedHp)
            {
                bool isCorrect = Math.Abs(hero.CurrentHp - expectedHp) <= HpTolerance;
                AddLine(stepDescription, isCorrect, $"정답 체력 {expectedHp}", $"실제 체력 {hero.CurrentHp}");
            }

            public void ExpectDead(string stepDescription, Hero hero, bool expectedIsDead)
            {
                bool isCorrect = hero.IsDead == expectedIsDead;
                AddLine(stepDescription, isCorrect, $"정답 쓰러짐 {expectedIsDead}", $"실제 쓰러짐 {hero.IsDead}");
            }

            public void ExpectText(string stepDescription, string expectedText, string actualText)
            {
                bool isCorrect = expectedText == actualText;
                AddLine(stepDescription, isCorrect, $"정답 {expectedText}", $"실제 {actualText}");
            }

            private void AddLine(string stepDescription, bool isCorrect, string expectedText, string actualText)
            {
                ++stepCount;
                if (!isCorrect)
                {
                    ++failedStepCount;
                }
                stepLines.AppendLine($"{(isCorrect ? "맞음" : "틀림")}  {stepDescription}");
                stepLines.AppendLine($"    {expectedText}  /  {actualText}");
            }
        }

        private void Start()
        {
            RunAllScenarios();
        }

        // 공격자 없이 피해만 준다 (함정·독 같은 피해 자리). 피해는 Hero 가 아니라 중재자(DamageCalculator)를 거쳐 들어간다.
        // 중재자의 피해 입구가 바뀌면 이 한 곳만 고친다
        private static void DealDamage(Hero target, float damage)
        {
            DamageCalculator.ApplyAttackResult(target, new AttackResult(damage, false));
        }

        [ContextMenu("유니크 동작 검사 실행")]
        private void RunAllScenarios()
        {
            // 게임 시작 때 한 번 부르는 자리를 대신한다
            UniqueModHandler.Initialize();

            var scenarioRunners = new List<Func<ScenarioRecord>>
            {
                // 7번이 맨 앞인 이유: 사용 기록이 static 이라 시나리오끼리 공유된다.
                //   앞 시나리오의 영웅이 치명상 버티기 장비를 낀 채로 남아 있으면, 그 장비 쪽에서 기록을 대신 지워 줘서 버그가 가려진다.
                RunSurviveLethalUnequippedAcrossStageScenario,
                RunNoUniqueScenario,
                RunSurviveLethalScenario,
                RunSurviveLethalReEquipScenario,
                RunEveryFifthCriticalScenario,
                RunDamageTakenReductionScenario,
                RunSurviveLethalWithReductionScenario,
                RunUniqueOnlyForOwnerScenario,
            };

            foreach (Func<ScenarioRecord> runScenario in scenarioRunners)
            {
                ScenarioRecord scenarioRecord;
                try
                {
                    scenarioRecord = runScenario();
                }
                catch (Exception scenarioException)
                {
                    // 리팩토링 도중 null 참조 같은 예외가 나도 나머지 시나리오는 계속 돈다
                    Debug.LogError($"{FailWord}  시나리오 도중 예외: {scenarioException.GetType().Name}: {scenarioException.Message}\n"
                                   + scenarioException.StackTrace);
                    continue;
                }
                LogScenario(scenarioRecord);
            }
        }

        // 1. 유니크 없음: 기본 피해 처리만 확인한다
        private ScenarioRecord RunNoUniqueScenario()
        {
            var scenarioRecord = new ScenarioRecord("1. 유니크 없음");
            var hero = new Hero("Knight", 100f, 10f);
            var stage = new Stage();
            stage.StartStage();

            DealDamage(hero, 30f);
            scenarioRecord.ExpectHp("체력 100에서 피해 30", hero, 70f);

            DealDamage(hero, 200f);
            scenarioRecord.ExpectHp("체력 70에서 피해 200 (버티기 없음)", hero, 0f);
            scenarioRecord.ExpectDead("쓰러졌나", hero, true);
            return scenarioRecord;
        }

        // 2. A: 스테이지당 한 번 버티고, 새 스테이지가 시작되면 다시 쓸 수 있다
        private ScenarioRecord RunSurviveLethalScenario()
        {
            var scenarioRecord = new ScenarioRecord("2. A 스테이지당 한 번 치명상 버티기");
            var hero = new Hero("Knight", 100f, 10f);
            var surviveLethalGear = new Gear(201UL, EUniqueType.SurviveLethalOncePerStage);
            hero.Equip(surviveLethalGear);
            var stage = new Stage();
            stage.StartStage();

            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("첫 스테이지, 체력 100에서 피해 150 → 버팀", hero, 1f);

            stage.StartStage();
            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("새 스테이지, 체력 1에서 피해 150 → 다시 버팀", hero, 1f);

            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("같은 스테이지, 체력 1에서 피해 150 → 이미 써서 쓰러짐", hero, 0f);
            scenarioRecord.ExpectDead("쓰러졌나", hero, true);
            return scenarioRecord;
        }

        // 3. A: 같은 스테이지에서 뺐다 다시 껴도 "썼음" 표시는 남아 있다 (끼었다 뺐다로 횟수 초기화 방지)
        private ScenarioRecord RunSurviveLethalReEquipScenario()
        {
            var scenarioRecord = new ScenarioRecord("3. A 같은 스테이지에서 뺐다 끼기");
            var hero = new Hero("Knight", 100f, 10f);
            var surviveLethalGear = new Gear(301UL, EUniqueType.SurviveLethalOncePerStage);
            hero.Equip(surviveLethalGear);
            var stage = new Stage();
            stage.StartStage();

            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("체력 100에서 피해 150 → 버팀", hero, 1f);

            hero.Unequip(surviveLethalGear.UniqueId);
            hero.Equip(surviveLethalGear);
            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("A를 뺐다 다시 끼고 피해 150 → 이미 써서 쓰러짐", hero, 0f);
            return scenarioRecord;
        }

        // 7. A: 장비를 뺀 채로 스테이지가 바뀌어도, 다시 끼면 새 스테이지 기준으로 다시 버틴다 (스테이지 초기화를 놓치지 않기)
        private ScenarioRecord RunSurviveLethalUnequippedAcrossStageScenario()
        {
            var scenarioRecord = new ScenarioRecord("7. A 뺀 채로 스테이지가 바뀜");
            var hero = new Hero("Knight", 100f, 10f);
            var surviveLethalGear = new Gear(701UL, EUniqueType.SurviveLethalOncePerStage);
            hero.Equip(surviveLethalGear);
            var stage = new Stage();
            stage.StartStage();

            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("첫 스테이지, 체력 100에서 피해 150 → 버팀", hero, 1f);

            hero.Unequip(surviveLethalGear.UniqueId);
            stage.StartStage();
            hero.Equip(surviveLethalGear);
            DealDamage(hero, 150f);
            scenarioRecord.ExpectHp("뺀 사이 새 스테이지, 다시 끼고 피해 150 → 다시 버팀", hero, 1f);
            return scenarioRecord;
        }

        // 4. B: 평타 다섯 번째마다 치명타. B를 안 낀 영웅은 치명타가 없다
        private ScenarioRecord RunEveryFifthCriticalScenario()
        {
            var scenarioRecord = new ScenarioRecord("4. B 다섯 번째 평타마다 치명타");

            var attackerWithUnique = new Hero("Archer", 100f, 10f);
            attackerWithUnique.Equip(new Gear(401UL, EUniqueType.EveryFifthBaseAttackCritical));
            var targetOfUnique = new Hero("Dummy", 1000f, 0f);
            var stage = new Stage();
            stage.StartStage();
            var battle = new Battle();

            string criticalAttackNumbersText = AttackTenTimes(battle, attackerWithUnique, targetOfUnique);
            scenarioRecord.ExpectText("B를 낀 영웅이 평타 10번, 치명타가 난 번호", "5, 10", criticalAttackNumbersText);
            scenarioRecord.ExpectHp("공격력 10, 평타 8번 + 치명타 2번(20씩) = 피해 120", targetOfUnique, 880f);

            var attackerWithoutUnique = new Hero("Archer", 100f, 10f);
            var targetOfNoUnique = new Hero("Dummy", 1000f, 0f);
            var stageWithoutUnique = new Stage();
            stageWithoutUnique.StartStage();
            var battleWithoutUnique = new Battle();

            string noUniqueCriticalNumbersText = AttackTenTimes(battleWithoutUnique, attackerWithoutUnique, targetOfNoUnique);
            scenarioRecord.ExpectText("B를 안 낀 영웅이 평타 10번, 치명타가 난 번호", "(없음)", noUniqueCriticalNumbersText);
            scenarioRecord.ExpectHp("공격력 10, 평타 10번 = 피해 100", targetOfNoUnique, 900f);
            return scenarioRecord;
        }

        // 5. C: 하나 끼면 10%, 둘 끼면 20% 감소. 빼면 그만큼 돌아온다
        private ScenarioRecord RunDamageTakenReductionScenario()
        {
            var scenarioRecord = new ScenarioRecord("5. C 받는 피해 10% 감소 (겹치면 더함)");
            var hero = new Hero("Knight", 1000f, 10f);
            var stage = new Stage();
            stage.StartStage();

            var firstReductionGear = new Gear(501UL, EUniqueType.DamageTakenReduction10);
            var secondReductionGear = new Gear(502UL, EUniqueType.DamageTakenReduction10);

            hero.Equip(firstReductionGear);
            DealDamage(hero, 100f);
            scenarioRecord.ExpectHp("C 한 개, 피해 100 → 90 받음", hero, 910f);

            hero.Equip(secondReductionGear);
            DealDamage(hero, 100f);
            scenarioRecord.ExpectHp("C 두 개, 피해 100 → 80 받음", hero, 830f);

            hero.Unequip(secondReductionGear.UniqueId);
            DealDamage(hero, 100f);
            scenarioRecord.ExpectHp("C 하나 뺌(한 개 남음), 피해 100 → 90 받음", hero, 740f);

            hero.Unequip(firstReductionGear.UniqueId);
            DealDamage(hero, 100f);
            scenarioRecord.ExpectHp("C 다 뺌, 피해 100 → 100 받음", hero, 640f);
            return scenarioRecord;
        }

        // 8. A, C: 유니크는 그 장비를 낀 영웅이 맞을 때만 동작한다. 다른 영웅이 맞을 때 끼어들면 안 된다
        private ScenarioRecord RunUniqueOnlyForOwnerScenario()
        {
            var scenarioRecord = new ScenarioRecord("8. 유니크는 낀 영웅에게만");
            var reductionOwner = new Hero("ReductionOwner", 1000f, 10f);
            var surviveOwner = new Hero("SurviveOwner", 10f, 10f);
            var heroWithoutUnique = new Hero("NoUnique", 100f, 10f);
            reductionOwner.Equip(new Gear(801UL, EUniqueType.DamageTakenReduction10));
            surviveOwner.Equip(new Gear(802UL, EUniqueType.SurviveLethalOncePerStage));
            var stage = new Stage();
            stage.StartStage();

            DealDamage(heroWithoutUnique, 50f);
            scenarioRecord.ExpectHp("유니크 없는 영웅이 피해 50 → 감소·버티기 끼어들지 않음", heroWithoutUnique, 50f);

            DealDamage(reductionOwner, 100f);
            scenarioRecord.ExpectHp("C를 낀 영웅이 피해 100 → 90 받음", reductionOwner, 910f);

            DealDamage(surviveOwner, 50f);
            scenarioRecord.ExpectHp("A를 낀 영웅(체력 10)이 피해 50 → 버팀 (C는 남의 것이라 안 줄어듦)", surviveOwner, 1f);
            return scenarioRecord;
        }

        // 6. A + C: 감소가 먼저, 쓰러지나 판정은 그 다음. 순서가 바뀌면 값이 달라진다
        private ScenarioRecord RunSurviveLethalWithReductionScenario()
        {
            var scenarioRecord = new ScenarioRecord("6. A + C 순서 (감소 먼저, 치명상 판정 나중)");
            var hero = new Hero("Knight", 100f, 10f);
            hero.Equip(new Gear(601UL, EUniqueType.SurviveLethalOncePerStage));
            hero.Equip(new Gear(602UL, EUniqueType.DamageTakenReduction10));
            var stage = new Stage();
            stage.StartStage();

            DealDamage(hero, 105f);
            scenarioRecord.ExpectHp("체력 100에서 피해 105 → 10% 줄어 94.5 → 안 쓰러짐(A 안 씀)", hero, 5.5f);

            DealDamage(hero, 200f);
            scenarioRecord.ExpectHp("피해 200 → 180 → 쓰러질 피해라 A로 버팀", hero, 1f);

            DealDamage(hero, 200f);
            scenarioRecord.ExpectHp("피해 200 → A 이미 써서 쓰러짐", hero, 0f);
            return scenarioRecord;
        }

        // 평타 10번을 치고, 치명타가 난 번호(1부터)를 "5, 10" 모양 글자로 돌려준다
        private static string AttackTenTimes(Battle battle, Hero attacker, Hero target)
        {
            var criticalAttackNumbers = new List<int>();
            for (int attackNumber = 1; attackNumber <= 10; ++attackNumber)
            {
                AttackResult attackResult = battle.BaseAttack(attacker, target);
                if (attackResult.isCritical)
                {
                    criticalAttackNumbers.Add(attackNumber);
                }
            }
            return criticalAttackNumbers.Count == 0 ? "(없음)" : string.Join(", ", criticalAttackNumbers);
        }

        private static void LogScenario(ScenarioRecord scenarioRecord)
        {
            string logPrefix = $"[{scenarioRecord.scenarioTitle}]";
            if (scenarioRecord.failedStepCount == 0)
            {
                Debug.Log($"{logPrefix} {SuccessWord}  {scenarioRecord.stepCount}단계 모두 맞음 (클릭하면 단계별 값)\n\n"
                          + scenarioRecord.stepLines);
                return;
            }

            Debug.LogError($"{logPrefix} {FailWord}  {scenarioRecord.stepCount}단계 중 {scenarioRecord.failedStepCount}단계 틀림\n\n"
                           + scenarioRecord.stepLines);
        }
    }
}
