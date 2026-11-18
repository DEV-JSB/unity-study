using System;

// 정렬 실습 재료: 등급(ItemGrade) enum과, 등급을 가진 아이템(SortItem) 클래스.
// 06의 Grade와 이름이 겹치지 않도록 ItemGrade로 따로 만들었다. 숫자 배치는 같다.
//
// 작업자가 채울 곳: SortItem.CompareTo 하나 (검사기 4번).
//   items.Sort(비교 메서드)는 넘긴 메서드로 비교하고,
//   items.Sort()처럼 아무것도 안 넘기면 SortItem 자신의 CompareTo로 비교한다. 그래서 IComparable<SortItem>을 붙였다.
//
// CompareTo 약속 (공식 문서)
//   this.CompareTo(otherItem)는 otherItem을 수평선의 0 자리에 세워 두고, this가 어느 쪽에 떨어지는지 돌려준다.
//     this가 앞(왼쪽)이어야 한다 → 0보다 작은 값
//     같은 자리                   → 0
//     this가 뒤(오른쪽)이어야 한다 → 0보다 큰 값
//   약속은 "0보다 작다 / 0 / 0보다 크다" 뿐이다. -1, 1이 나와야 한다는 약속은 없다.
//   방향만 알려 주고 거리(몇 칸)는 알려 주지 않는다. 최종 칸 번호는 List.Sort가 여러 번 비교해서 만든다.

public enum ItemGrade
{
    Normal, // 0
    Rare,   // 1
    Epic,   // 2
    Legend, // 3
}

public class SortItem : IComparable<SortItem>
{
    public readonly string itemName;
    public readonly ItemGrade grade;

    public SortItem(string itemName, ItemGrade grade)
    {
        this.itemName = itemName;
        this.grade = grade;
    }

    // items.Sort() (아무것도 안 넘길 때)의 기본 순서: 등급 낮은 순.
    // 지금은 항상 0(같은 자리)을 돌려준다. 검사기의 4번이 실패로 나온다.
    public int CompareTo(SortItem otherItem)
    {
        return this.grade.CompareTo(otherItem.grade);
    }

    public override string ToString()
    {
        return $"{itemName}({grade})";
    }
}
