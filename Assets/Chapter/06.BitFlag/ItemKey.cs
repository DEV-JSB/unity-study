// 아이템 종류(ItemType)와 등급(Grade), enum 두 개를 Dictionary 키 하나(ulong)로 묶는다. (작업자가 채울 파일)
//
// ulong 64비트를 2진수로 가로로 적었을 때, 가운데를 잘라 왼쪽과 오른쪽으로 나눈다.
// (Console의 2진수 출력과 같은 방향. 맨 왼쪽이 63번 비트, 맨 오른쪽이 0번 비트)
//   왼쪽 32칸 (32번 ~ 63번 비트) = Grade 몫
//   오른쪽 32칸 (0번 ~ 31번 비트) = ItemType 몫
// 다른 코드나 문서에서는 왼쪽 32칸을 "상위 32비트 (high bits)", 오른쪽 32칸을 "하위 32비트 (low bits)"라고 부른다.
//
// 같은 일을 두 가지 방법으로 만든다. 예시는 Armor(1) + Epic(2)
//   A. 숫자 그대로 넣기 (bit packing)
//      왼쪽 32칸에 숫자 2를 2진수 그대로, 오른쪽 32칸에 숫자 1을 2진수 그대로 넣는다
//      → 켜지는 비트: 33번(왼쪽 32칸 안의 2 = 10), 0번(오른쪽 32칸 안의 1 = 01)
//   B. 비트 하나씩 켜기 (bit flag)
//      왼쪽 32칸의 2번 칸 하나, 오른쪽 32칸의 1번 칸 하나만 켠다 (BitGrid의 Set과 같은 생각)
//      → 켜지는 비트: 34번(왼쪽 32칸 안의 2번 칸), 1번(오른쪽 32칸 안의 1번 칸)
//
// Get...From...Key 메서드는 MakeKey의 반대 방향이다. 키 하나에서 enum 하나씩 따로 꺼낸다.
// 검사는 ItemKeyChecker가 한다. 채우기 전에는 모두 0과 기본값을 돌려주므로 검사가 전부 실패로 나온다.

public enum ItemType
{
    Weapon, // 0
    Armor,  // 1
    Potion, // 2
    Ring,   // 3
}

public enum Grade
{
    Normal, // 0
    Rare,   // 1
    Epic,   // 2
    Legend, // 3
}

public static class ItemKey
{
    // ───────── A. 숫자 그대로 넣기 ─────────

    public static ulong MakePackedKey(ItemType itemType, Grade grade)
    {
        ulong key = 0;
        
        key |= (ulong)itemType;
        key |= ((ulong)grade << 32);
        
        return key;
    }

    public static ItemType GetItemTypeFromPackedKey(ulong packedKey)
    {
        ulong key = packedKey;
        // 1. & 로 Grade 부는 다 끄기
        key &= (1UL << 32) - 1;
        int value = (int)key;
        
        // 2. << 로 뒷부분 날려서 >> 로 다시 땅겨서 계산하기
        key = packedKey;
        key = key << 32;
        key = key >> 32;
        value = (int)key;

        // 3. 칸 잘려나가게 하기
        value = (int)key;
        
        return (ItemType)value;
    }

    public static Grade GetGradeFromPackedKey(ulong packedKey)
    {
        ulong key = packedKey;
        int value;
        
        
        key = packedKey;
        key = key >> 32;
        value = (int)key;
        
        return (Grade)value;
    }

    // ───────── B. 비트 하나씩 켜기 ─────────

    public static ulong MakeFlagKey(ItemType itemType, Grade grade)
    {
        ulong key = 0;
        
        ulong itemFlag = 1UL << (int)itemType;
        ulong gradeFlag = (1UL << 32) << (int)grade;
        
        key = itemFlag | gradeFlag;
        return key;
    }

    public static ItemType GetItemTypeFromFlagKey(ulong flagKey)
    {
        for (int i = 0; i < 32; ++i)
        {
            ulong flag = 1UL << i;

            if ((flagKey & flag) != 0)
            {
                return (ItemType)i;
            }

        }
        return default;
    }
    
    public static Grade GetGradeFromFlagKey(ulong flagKey)
    {
        ulong key = flagKey;
        key = key >> 32;
        for (int i = 0; i < 32; ++i)
        {
            ulong flag = 1UL << i;

            if ((key & flag) != 0)
            {
                return (Grade)i;
            }

        }
        return default;
    }
}
