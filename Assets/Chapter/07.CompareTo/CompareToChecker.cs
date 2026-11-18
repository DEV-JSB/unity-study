using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// List.Sort에 작업자의 비교 메서드를 넘겨서, 정렬 결과가 맞는지 검사한다.
// 사용법: 빈 GameObject에 붙이고 Play 하거나, Play 없이 컴포넌트 메뉴의 "정렬 검사 실행"을 누른다.
//
// Console에는 로그가 5개 나온다. 검사 하나 = 로그 하나라서, 성공은 일반 로그, 실패는 빨간 에러로 보인다.
//   0. enum CompareTo 반환값 표 : ItemGrade 기본 CompareTo가 실제로 어떤 숫자를 돌려주는지 (검사 아님, 값 확인용)
//   1. items.Sort(ItemComparisons.CompareByGradeAscending)          : 등급 낮은 순
//   2. items.Sort(ItemComparisons.CompareByGradeDescending)         : 등급 높은 순
//   3. items.Sort(ItemComparisons.CompareByGradeDescendingThenName) : 등급 높은 순, 같으면 이름 순
//   4. items.Sort()  (아무것도 안 넘김 → SortItem.CompareTo)       : 등급 낮은 순
// 로그를 클릭하면 경우마다 정렬 전 / 결과 / 정답이 아래 칸에 나온다.
//
// 1, 2, 4번은 등급 순서만 본다. List.Sort는 같은 자리(0)인 것끼리의 순서를 지켜 주지 않는다 (공식 문서: 불안정 정렬, unstable sort).
// 그래서 같은 등급끼리의 순서까지 정하고 싶으면 3번처럼 비교 기준을 하나 더 넣어야 한다.
public class CompareToChecker : MonoBehaviour
{
    private const string SuccessWord = "<color=#5DC264>성공</color>";
    private const string FailWord = "<color=#FF6B6B>실패</color>";

    private enum SortRule
    {
        GradeAscending,
        GradeDescending,
        GradeDescendingThenName,
    }

    // 정렬 전 아이템 묶음 하나와, 3번 규칙(등급 높은 순, 같으면 이름 순)일 때의 정답 이름 순서
    private class SortTestCase
    {
        public readonly string caseName;
        public readonly SortItem[] items;
        public readonly string[] expectedNamesForGradeDescendingThenName;

        public SortTestCase(string caseName, SortItem[] items, string[] expectedNamesForGradeDescendingThenName)
        {
            this.caseName = caseName;
            this.items = items;
            this.expectedNamesForGradeDescendingThenName = expectedNamesForGradeDescendingThenName;
        }
    }

    private static List<SortTestCase> MakeTestCases()
    {
        return new List<SortTestCase>
        {
            new SortTestCase("빈 리스트",
                new SortItem[0],
                new string[0]),
            new SortTestCase("한 개",
                new[] { new SortItem("Coin", ItemGrade.Epic) },
                new[] { "Coin" }),
            new SortTestCase("낮은 순으로 놓여 있음",
                new[]
                {
                    new SortItem("Club", ItemGrade.Normal), new SortItem("Mace", ItemGrade.Rare),
                    new SortItem("Staff", ItemGrade.Epic), new SortItem("Wand", ItemGrade.Legend),
                },
                new[] { "Wand", "Staff", "Mace", "Club" }),
            new SortTestCase("높은 순으로 놓여 있음",
                new[]
                {
                    new SortItem("Wand", ItemGrade.Legend), new SortItem("Staff", ItemGrade.Epic),
                    new SortItem("Mace", ItemGrade.Rare), new SortItem("Club", ItemGrade.Normal),
                },
                new[] { "Wand", "Staff", "Mace", "Club" }),
            new SortTestCase("같은 등급 섞임",
                new[]
                {
                    new SortItem("Sword", ItemGrade.Epic), new SortItem("Axe", ItemGrade.Normal),
                    new SortItem("Bow", ItemGrade.Epic), new SortItem("Ring", ItemGrade.Legend),
                    new SortItem("Cap", ItemGrade.Normal), new SortItem("Dagger", ItemGrade.Rare),
                },
                new[] { "Ring", "Bow", "Sword", "Dagger", "Axe", "Cap" }),
            new SortTestCase("전부 같은 등급",
                new[]
                {
                    new SortItem("Rope", ItemGrade.Rare), new SortItem("Lamp", ItemGrade.Rare),
                    new SortItem("Gem", ItemGrade.Rare),
                },
                new[] { "Gem", "Lamp", "Rope" }),
        };
    }

    private void Start()
    {
        RunAllChecks();
    }

    [ContextMenu("정렬 검사 실행")]
    private void RunAllChecks()
    {
        LogEnumCompareToTable();

        CheckSort("1. 등급 낮은 순",
            "items.Sort(ItemComparisons.CompareByGradeAscending)",
            items => items.Sort(ItemComparisons.CompareByGradeAscending),
            ItemComparisons.CompareByGradeAscending,
            SortRule.GradeAscending);

        CheckSort("2. 등급 높은 순",
            "items.Sort(ItemComparisons.CompareByGradeDescending)",
            items => items.Sort(ItemComparisons.CompareByGradeDescending),
            ItemComparisons.CompareByGradeDescending,
            SortRule.GradeDescending);

        CheckSort("3. 등급 높은 순, 같으면 이름 순",
            "items.Sort(ItemComparisons.CompareByGradeDescendingThenName)",
            items => items.Sort(ItemComparisons.CompareByGradeDescendingThenName),
            ItemComparisons.CompareByGradeDescendingThenName,
            SortRule.GradeDescendingThenName);

        CheckSort("4. 아무것도 안 넘김 → SortItem.CompareTo",
            "items.Sort()",
            items => items.Sort(),
            (leftItem, rightItem) => leftItem.CompareTo(rightItem),
            SortRule.GradeAscending);
    }

    // 0. ItemGrade 기본 CompareTo가 돌려주는 실제 숫자.
    // "-1, 0, 1만 나오나?"를 눈이 아니라 값으로 확인한다.
    // 참고: enum의 CompareTo는 매개변수가 object라서, 괄호 안 값이 박싱(boxing)된다 (01 BoxingUnBoxing 챕터).
    private void LogEnumCompareToTable()
    {
        var allGrades = (ItemGrade[])Enum.GetValues(typeof(ItemGrade));
        var tableBuilder = new StringBuilder();
        var seenResults = new SortedSet<int>();

        foreach (ItemGrade thisGrade in allGrades)
        {
            foreach (ItemGrade otherGrade in allGrades)
            {
                int compareResult = thisGrade.CompareTo(otherGrade);
                seenResults.Add(compareResult);
                tableBuilder.AppendLine($"{thisGrade}({(int)thisGrade}).CompareTo({otherGrade}({(int)otherGrade})) = {compareResult}");
            }
        }

        Debug.Log($"[0. enum CompareTo 반환값 표] 나온 숫자 종류: {string.Join(", ", seenResults)}  (클릭하면 {allGrades.Length * allGrades.Length}줄)\n"
                  + tableBuilder);
    }

    // checkTitle: Console 로그 앞에 붙을 검사 이름
    // callShape: 실제로 부르는 코드 모양 (로그에 그대로 보여 준다)
    // sortItems: 리스트를 정렬하는 호출
    // comparison: 정렬 전에 따로 불러서 "안 채웠나 / 같은 것끼리 0인가"를 보는 비교 메서드
    private void CheckSort(string checkTitle, string callShape, Action<List<SortItem>> sortItems,
        Comparison<SortItem> comparison, SortRule rule)
    {
        string logPrefix = $"[{checkTitle}]";

        // 정렬 전 점검: 등급마다 견본 하나씩. 이름은 같게 둬서 등급만 다르게 한다
        var allGrades = (ItemGrade[])Enum.GetValues(typeof(ItemGrade));
        var sampleItems = new List<SortItem>();
        foreach (ItemGrade grade in allGrades)
        {
            sampleItems.Add(new SortItem("Sample", grade));
        }

        bool isAlwaysZero = true;
        var selfCompareProblemBuilder = new StringBuilder();
        foreach (SortItem leftItem in sampleItems)
        {
            foreach (SortItem rightItem in sampleItems)
            {
                int compareResult = comparison(leftItem, rightItem);
                if (ReferenceEquals(leftItem, rightItem))
                {
                    if (compareResult != 0)
                    {
                        selfCompareProblemBuilder.AppendLine($"{leftItem} 와 자기 자신을 비교했는데 {compareResult} (0이어야 한다)");
                    }
                }
                else if (compareResult != 0)
                {
                    isAlwaysZero = false;
                }
            }
        }

        // 정렬해 보기
        List<SortTestCase> testCases = MakeTestCases();
        var detailBuilder = new StringBuilder();
        int correctCaseCount = 0;
        int exceptionCaseCount = 0;

        foreach (SortTestCase testCase in testCases)
        {
            var sortedItems = new List<SortItem>(testCase.items);
            string exceptionText = null;
            try
            {
                sortItems(sortedItems);
            }
            catch (Exception sortException)
            {
                exceptionText = $"{sortException.GetType().Name}: {sortException.Message}";
                ++exceptionCaseCount;
            }

            bool isCorrect = exceptionText == null && IsCorrectOrder(sortedItems, testCase, rule);
            if (isCorrect)
            {
                ++correctCaseCount;
            }

            detailBuilder.AppendLine($"{(isCorrect ? "맞음" : "틀림")}  {testCase.caseName}");
            detailBuilder.AppendLine($"    정렬 전 : {JoinItems(new List<SortItem>(testCase.items))}");
            if (exceptionText != null)
            {
                detailBuilder.AppendLine($"    예외    : {exceptionText}");
            }
            else
            {
                detailBuilder.AppendLine($"    결과    : {JoinItems(sortedItems)}");
            }
            detailBuilder.AppendLine($"    정답    : {MakeExpectedText(testCase, rule)}");
        }

        if (correctCaseCount == testCases.Count && selfCompareProblemBuilder.Length == 0)
        {
            Debug.Log($"{logPrefix} {SuccessWord}  {testCases.Count}개 경우 모두 맞음 (클릭하면 경우별 결과)\n"
                      + $"부른 코드: {callShape}\n\n"
                      + detailBuilder);
            return;
        }

        string hint;
        if (isAlwaysZero)
        {
            hint = "등급이 다른데도 비교 결과가 전부 0이다. 아직 안 채운 것 같다";
        }
        else if (exceptionCaseCount > 0)
        {
            hint = "Sort 도중 예외가 났다. 같은 두 값에 0을 안 돌려주거나, (A, B)와 (B, A)의 답이 서로 앞뒤가 안 맞는 것 같다";
        }
        else if (selfCompareProblemBuilder.Length > 0)
        {
            hint = "같은 아이템끼리 비교했는데 0이 아니다. '같을 때'를 빠뜨린 것 같다\n"
                   + "(정렬 결과가 맞게 나와도 실패로 친다. 데이터가 바뀌면 예외가 나거나 순서가 틀어질 수 있다)\n"
                   + selfCompareProblemBuilder;
        }
        else
        {
            hint = "틀린 경우는 아래 목록 참고";
        }

        Debug.LogError($"{logPrefix} {FailWord}  {testCases.Count}개 경우 중 {correctCaseCount}개 맞음\n"
                       + $"부른 코드: {callShape}\n"
                       + hint + "\n\n"
                       + detailBuilder);
    }

    private static bool IsCorrectOrder(List<SortItem> sortedItems, SortTestCase testCase, SortRule rule)
    {
        if (rule == SortRule.GradeDescendingThenName)
        {
            string[] expectedNames = testCase.expectedNamesForGradeDescendingThenName;
            if (sortedItems.Count != expectedNames.Length)
            {
                return false;
            }
            for (int index = 0; index < expectedNames.Length; ++index)
            {
                if (sortedItems[index].itemName != expectedNames[index])
                {
                    return false;
                }
            }
            return true;
        }

        List<ItemGrade> expectedGrades = MakeExpectedGrades(testCase.items, rule == SortRule.GradeDescending);
        if (sortedItems.Count != expectedGrades.Count)
        {
            return false;
        }
        for (int index = 0; index < expectedGrades.Count; ++index)
        {
            if (sortedItems[index].grade != expectedGrades[index])
            {
                return false;
            }
        }
        return true;
    }

    // 정답 등급 순서: 등급마다 몇 개 있는지 세어서, 낮은 등급부터(또는 높은 등급부터) 개수만큼 늘어놓는다.
    // 작업자의 비교 메서드를 쓰지 않으므로, 비교 메서드가 틀려도 정답은 흔들리지 않는다.
    private static List<ItemGrade> MakeExpectedGrades(SortItem[] items, bool isDescending)
    {
        int gradeKindCount = Enum.GetValues(typeof(ItemGrade)).Length;
        var countByGrade = new int[gradeKindCount];
        foreach (SortItem item in items)
        {
            ++countByGrade[(int)item.grade];
        }

        var expectedGrades = new List<ItemGrade>();
        for (int step = 0; step < gradeKindCount; ++step)
        {
            int gradeNumber = isDescending ? gradeKindCount - 1 - step : step;
            for (int repeat = 0; repeat < countByGrade[gradeNumber]; ++repeat)
            {
                expectedGrades.Add((ItemGrade)gradeNumber);
            }
        }
        return expectedGrades;
    }

    private static string MakeExpectedText(SortTestCase testCase, SortRule rule)
    {
        if (rule == SortRule.GradeDescendingThenName)
        {
            string[] expectedNames = testCase.expectedNamesForGradeDescendingThenName;
            return expectedNames.Length == 0 ? "(비어 있음)" : string.Join(", ", expectedNames);
        }

        List<ItemGrade> expectedGrades = MakeExpectedGrades(testCase.items, rule == SortRule.GradeDescending);
        return expectedGrades.Count == 0
            ? "(비어 있음)"
            : string.Join(", ", expectedGrades) + "  (등급 순서만 본다. 같은 등급끼리 순서는 상관없음)";
    }

    private static string JoinItems(List<SortItem> items)
    {
        return items.Count == 0 ? "(비어 있음)" : string.Join(", ", items);
    }
}
