using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// ItemKey의 A, B 두 방법을 모든 조합(ItemType 4개 × Grade 4개 = 16개)으로 검사한다.
// 사용법: 빈 GameObject에 붙이고 Play 하거나, Play 없이 컴포넌트 메뉴의 "키 검사 실행"을 누른다.
//
// Console에는 방법마다 로그가 4개 나온다. 검사 하나 = 로그 하나라서, 성공은 일반 로그, 실패는 빨간 에러로 보인다.
//   [A] 0. 키 표            : 16개 조합의 키를 2진수로 (클릭하면 아래 칸에 전부 보인다)
//   [A] 1. 키 겹침 검사     : 서로 다른 조합이 같은 키를 만들면 실패 (Dictionary 키로 못 쓴다)
//   [A] 2. ItemType 꺼내기  : 키에서 꺼낸 ItemType이 처음 넣은 값과 같아야 성공
//   [A] 3. Grade 꺼내기     : 키에서 꺼낸 Grade가 처음 넣은 값과 같아야 성공
// 실패 로그를 클릭하면 어느 조합이 틀렸는지 아래 칸에 나온다.
public class ItemKeyChecker : MonoBehaviour
{
    private const string SuccessWord = "<color=#5DC264>성공</color>";
    private const string FailWord = "<color=#FF6B6B>실패</color>";

    private void Start()
    {
        RunCheck();
    }

    [ContextMenu("키 검사 실행")]
    private void RunCheck()
    {
        CheckKeyMethod("A. 숫자 그대로 넣기",
            ItemKey.MakePackedKey, ItemKey.GetItemTypeFromPackedKey, ItemKey.GetGradeFromPackedKey);

        CheckKeyMethod("B. 비트 하나씩 켜기",
            ItemKey.MakeFlagKey, ItemKey.GetItemTypeFromFlagKey, ItemKey.GetGradeFromFlagKey);
    }

    // methodTitle: Console 로그 앞에 붙을 방법 이름
    private void CheckKeyMethod(string methodTitle,
        Func<ItemType, Grade, ulong> makeKey,
        Func<ulong, ItemType> getItemTypeFromKey,
        Func<ulong, Grade> getGradeFromKey)
    {
        var allItemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
        var allGrades = (Grade[])Enum.GetValues(typeof(Grade));
        int combinationCount = allItemTypes.Length * allGrades.Length;

        // 키 → 그 키를 처음 만든 조합 이름. 같은 키가 또 나오면 겹친 것이다
        var combinationNameByKey = new Dictionary<ulong, string>();

        var keyTableBuilder = new StringBuilder();
        var collisionDetailBuilder = new StringBuilder();
        var itemTypeFailDetailBuilder = new StringBuilder();
        var gradeFailDetailBuilder = new StringBuilder();

        int collisionCount = 0;
        int zeroKeyCount = 0;
        int itemTypeCorrectCount = 0;
        int gradeCorrectCount = 0;
        int defaultItemTypeCount = 0; // 꺼낸 ItemType이 0번(기본값)인 횟수. 전부 0번이면 아직 안 채운 것
        int defaultGradeCount = 0;

        foreach (ItemType itemType in allItemTypes)
        {
            foreach (Grade grade in allGrades)
            {
                string combinationName = $"{itemType}({(int)itemType}) + {grade}({(int)grade})";
                ulong key = makeKey(itemType, grade);

                keyTableBuilder.AppendLine($"{ToBinaryWithHalves(key)}   {combinationName}   키 값 {key}");

                // 1. 키 겹침
                if (key == 0)
                {
                    ++zeroKeyCount;
                }
                if (combinationNameByKey.TryGetValue(key, out string firstCombinationName))
                {
                    ++collisionCount;
                    collisionDetailBuilder.AppendLine($"{combinationName} 의 키가 {firstCombinationName} 의 키와 같다 (키 값 {key})");
                }
                else
                {
                    combinationNameByKey.Add(key, combinationName);
                }

                // 2. ItemType 꺼내기
                ItemType restoredItemType = getItemTypeFromKey(key);
                if (restoredItemType == itemType)
                {
                    ++itemTypeCorrectCount;
                }
                else
                {
                    itemTypeFailDetailBuilder.AppendLine($"{combinationName} → 꺼낸 ItemType: {restoredItemType}({(int)restoredItemType})");
                }
                if ((int)restoredItemType == 0)
                {
                    ++defaultItemTypeCount;
                }

                // 3. Grade 꺼내기
                Grade restoredGrade = getGradeFromKey(key);
                if (restoredGrade == grade)
                {
                    ++gradeCorrectCount;
                }
                else
                {
                    gradeFailDetailBuilder.AppendLine($"{combinationName} → 꺼낸 Grade: {restoredGrade}({(int)restoredGrade})");
                }
                if ((int)restoredGrade == 0)
                {
                    ++defaultGradeCount;
                }
            }
        }

        string logPrefix = $"[{methodTitle}]";

        // 0. 키 표
        Debug.Log($"{logPrefix} 0. 키 표 (클릭하면 아래 칸에 {combinationCount}줄)\n"
                  + "왼쪽 32칸 (63번 ~ 32번 비트) | 오른쪽 32칸 (31번 ~ 0번 비트)\n"
                  + keyTableBuilder);

        // 1. 키 겹침 검사
        if (collisionCount == 0)
        {
            Debug.Log($"{logPrefix} 1. 키 겹침 검사: {SuccessWord}  키 {combinationCount}개가 모두 다르다");
        }
        else
        {
            string collisionHint = zeroKeyCount == combinationCount
                ? "모든 키가 0이다. 키 만들기 메서드를 아직 안 채운 것 같다"
                : "겹친 조합은 아래 목록 참고";
            Debug.LogError($"{logPrefix} 1. 키 겹침 검사: {FailWord}  {collisionCount}개가 앞에서 나온 키와 겹친다\n"
                           + collisionHint + "\n\n"
                           + collisionDetailBuilder);
        }

        // 2, 3. 꺼내기 검사
        ReportRestoreResult(logPrefix, "2. ItemType 꺼내기", itemTypeCorrectCount, combinationCount,
            defaultItemTypeCount, itemTypeFailDetailBuilder);
        ReportRestoreResult(logPrefix, "3. Grade 꺼내기", gradeCorrectCount, combinationCount,
            defaultGradeCount, gradeFailDetailBuilder);
    }

    // checkTitle: "2. ItemType 꺼내기" 같은 검사 이름
    // defaultResultCount: 꺼낸 값이 0번(기본값)이었던 횟수
    private void ReportRestoreResult(string logPrefix, string checkTitle, int correctCount, int combinationCount,
        int defaultResultCount, StringBuilder failDetailBuilder)
    {
        if (correctCount == combinationCount)
        {
            Debug.Log($"{logPrefix} {checkTitle}: {SuccessWord}  {combinationCount}개 중 {correctCount}개 맞음");
            return;
        }

        string restoreHint = defaultResultCount == combinationCount
            ? "꺼낸 값이 전부 0번(기본값)이다. 꺼내기 메서드를 아직 안 채운 것 같다"
            : "틀린 조합은 아래 목록 참고";
        Debug.LogError($"{logPrefix} {checkTitle}: {FailWord}  {combinationCount}개 중 {correctCount}개 맞음\n"
                       + restoreHint + "\n\n"
                       + failDetailBuilder);
    }

    // 64자리 2진수를 왼쪽 32칸과 오른쪽 32칸 사이에 " | "를 넣어서 돌려준다
    // 왼쪽 끝이 63번 비트, 오른쪽 끝이 0번 비트
    private static string ToBinaryWithHalves(ulong key)
    {
        // Convert.ToString에는 ulong을 받는 2진수 버전이 없다. long으로 바꿔도 64비트 모양은 그대로다
        string keyBinary = Convert.ToString((long)key, 2).PadLeft(64, '0');
        return keyBinary.Substring(0, 32) + " | " + keyBinary.Substring(32);
    }
}
