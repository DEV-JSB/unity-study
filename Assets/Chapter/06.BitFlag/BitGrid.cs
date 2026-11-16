using System;

// 8×8 격자의 켜짐/꺼짐을 int 8개에 비트로 담는다. (작업자가 채울 파일)
// int 하나가 한 줄(8칸)을 맡고, 그 int의 0~7번 비트가 그 줄의 칸 8개다.
// x와 y 중 어느 쪽을 배열 인덱스로, 어느 쪽을 비트 번호로 쓸지는 작업자가 정한다.
// 단, 모든 메서드에서 같은 규칙을 써야 한다.
//
// 좌표 규칙 (화면 쪽 BitGridView와의 약속)
//   x = 0~7, 왼쪽 → 오른쪽
//   y = 0~7, 위 → 아래
//   화면 왼쪽 위 칸이 (0, 0), 오른쪽 아래 칸이 (7, 7)
public class BitGrid
{
    public const int Size = 8;

    private readonly int[] _bits = new int[Size];

    // 화면 디버그 표시용. 배열 값을 그대로 돌려준다. (구현되어 있음)
    public int GetRaw(int index)
    {
        return _bits[index];
    }

    // (x, y) 칸이 켜져 있으면 true
    public bool IsOn(int x, int y)
    {
        int bit1 = 1 << x;
        int row = _bits[y];
        return (row & bit1) != 0;
    }

    // (x, y) 칸을 켠다. 이미 켜져 있으면 그대로
    public void Set(int x, int y)
    {
        int bit = 1 << x;
        _bits[y] |= bit;
    }

    // (x, y) 칸을 끈다. 이미 꺼져 있으면 그대로
    public void Clear(int x, int y)
    {
        int bit = 1 << x;
        bit = ~bit;
        _bits[y] &= bit;
    }

    // (x, y) 칸을 뒤집는다. 켜져 있으면 끄고, 꺼져 있으면 켠다
    public void Toggle(int x, int y)
    {
        int bit = 1 << x;
        _bits[y] ^= bit;
    }

    // 64칸 전부 끈다
    public void ClearAll()
    {
        for (int i = 0; i < Size; ++i)
        {
            _bits[i] = 0;
        }
    }

    // 64칸 전부 켠다
    public void FillAll()
    {
        for (int i = 0; i < Size; ++i)
        {
            _bits[i] = 255;
        }
    }

    // 64칸 전부 뒤집는다
    public void InvertAll()
    {
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                Toggle(j, i);
            }
        }
    }
}
