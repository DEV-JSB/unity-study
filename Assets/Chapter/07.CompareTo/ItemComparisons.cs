// List.Sort에 넘길 비교 메서드 모음. (작업자가 채울 파일)
//
// 쓰는 모양 (실무에서 정렬은 직접 구현하지 않고 List.Sort에 비교만 넘긴다)
//   이름 있는 메서드로 넘기기 : items.Sort(ItemComparisons.CompareByGradeAscending);
//   람다로 그 자리에서 쓰기   : items.Sort((leftItem, rightItem) => ...);   ← 하는 일은 같다
//
// 비교 메서드의 약속 (공식 문서) — CompareTo와 같은 약속이다
//   leftItem이 rightItem보다 앞(왼쪽)이어야 한다 → 0보다 작은 값
//   같은 자리                                     → 0
//   leftItem이 rightItem보다 뒤(오른쪽)이어야 한다 → 0보다 큰 값
//   List.Sort는 이 숫자의 부호만 보고, 여러 번 불러서 최종 순서를 만든다.
//
// 지금은 셋 다 항상 0(같은 자리)을 돌려준다. 검사기의 1 ~ 3번이 실패로 나온다.

public static class ItemComparisons
{
    // 1. 등급 낮은 순 (Normal → Rare → Epic → Legend)
    public static int CompareByGradeAscending(SortItem leftItem, SortItem rightItem)
    {
        return leftItem.grade.CompareTo(rightItem.grade);
    }

    // 2. 등급 높은 순 (Legend → Epic → Rare → Normal)
    public static int CompareByGradeDescending(SortItem leftItem, SortItem rightItem)
    {
        return rightItem.grade.CompareTo(leftItem.grade);
    }

    // 3. 등급 높은 순, 등급이 같으면 이름(itemName) 알파벳 순 (A → Z)
    public static int CompareByGradeDescendingThenName(SortItem leftItem, SortItem rightItem)
    {
        int result = rightItem.grade.CompareTo(leftItem.grade);
        if (result == 0)
        {
            return leftItem.itemName.CompareTo(rightItem.itemName);
        }
        return rightItem.grade.CompareTo(leftItem.grade);
    }
}
