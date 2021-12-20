using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
///     сортируем стринги по человечески. числа будут идти по порядку 1 2 3 4 5 6 7 8 9 10 11 12
///     а не 1 10 11 12 2 3 4 5 6 7 8
/// </summary>
public static class HumanStringSorter
{
    private static readonly Dictionary<string, string[]> table = new Dictionary<string, string[]>();

    public static int InnerCompare(string xL, string yL)
    {
        if (xL == yL) return 0;
        if (xL == null || yL == null) return 0;
        string[] x1, y1;
        if (!table.TryGetValue(xL, out x1))
        {
            x1 = Regex.Split(xL.Replace(" ", ""), "([0-9]+)");
            table.Add(xL, x1);
        }

        if (!table.TryGetValue(yL, out y1))
        {
            y1 = Regex.Split(yL.Replace(" ", ""), "([0-9]+)");
            table.Add(yL, y1);
        }

        for (var i = 0; i < x1.Length && i < y1.Length; i++)
            if (x1[i] != y1[i])
                return PartCompare(x1[i], y1[i]);
        if (y1.Length > x1.Length)
            return 1;
        if (x1.Length > y1.Length)
            return -1;
        return 0;
    }

    private static int PartCompare(string left, string right)
    {
        int x, y;
        if (!int.TryParse(left, out x))
            return left.CompareTo(right);

        if (!int.TryParse(right, out y))
            return left.CompareTo(right);

        return x.CompareTo(y);
    }
}