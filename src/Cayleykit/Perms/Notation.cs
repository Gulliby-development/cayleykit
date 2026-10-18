namespace Cayleykit;

public static class Notation
{
    public static IReadOnlyList<IReadOnlyList<int>> ParseCycles(string text)
    {
        if (text == "()")
        {
            return Array.Empty<IReadOnlyList<int>>();
        }

        if (!TryParseTokenCycles(text, out var tokenCycles))
        {
            throw InvalidPermutation(text);
        }

        var cycles = new List<List<int>>();

        foreach (var tokenCycle in tokenCycles)
        {
            var cycle = new List<int>();

            foreach (var token in tokenCycle)
            {
                if (!IsPointInRange(token))
                {
                    throw new CayleyException(
                        $"invalid point {Text.Show(token)} in {Text.Show(text)} (expected 1 to 64)");
                }

                cycle.Add(ParsePoint(token));
            }

            cycles.Add(cycle);
        }

        var seenPoints = new HashSet<int>();

        foreach (var cycle in cycles)
        {
            foreach (var point in cycle)
            {
                if (!seenPoints.Add(point))
                {
                    throw new CayleyException(
                        $"point {point.ToString(System.Globalization.CultureInfo.InvariantCulture)} is repeated in {Text.Show(text)}");
                }
            }
        }

        return cycles
            .Select(cycle => (IReadOnlyList<int>)cycle.AsReadOnly())
            .ToList()
            .AsReadOnly();
    }

    public static Perm Make(IReadOnlyList<IReadOnlyList<int>> cycles, int degree)
    {
        var images = new int[degree];

        for (var point = 1; point <= degree; point++)
        {
            images[point - 1] = point;
        }

        foreach (var cycle in cycles)
        {
            for (var index = 0; index < cycle.Count; index++)
            {
                images[cycle[index] - 1] = cycle[(index + 1) % cycle.Count];
            }
        }

        return Perm.FromImages(images);
    }

    public static (List<Perm> Perms, int Degree) ParseAll(IReadOnlyList<string> texts, long degree)
    {
        var parsedCycles = new List<IReadOnlyList<IReadOnlyList<int>>>();

        foreach (var text in texts)
        {
            parsedCycles.Add(ParseCycles(text));
        }

        var resolvedDegree = degree > 0
            ? checked((int)degree)
            : Math.Max(1, parsedCycles.SelectMany(cycles => cycles).SelectMany(cycle => cycle).DefaultIfEmpty(1).Max());

        for (var textIndex = 0; textIndex < parsedCycles.Count; textIndex++)
        {
            foreach (var cycle in parsedCycles[textIndex])
            {
                foreach (var point in cycle)
                {
                    if (point > resolvedDegree)
                    {
                        throw new CayleyException(
                            $"point {point.ToString(System.Globalization.CultureInfo.InvariantCulture)} of {Text.Show(texts[textIndex])} is above the degree {resolvedDegree.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
                    }
                }
            }
        }

        var perms = parsedCycles
            .Select(cycles => Make(cycles, resolvedDegree))
            .ToList();

        return (perms, resolvedDegree);
    }

    public static string Format(Perm p)
    {
        var cycles = p.Cycles()
            .Where(cycle => cycle.Count > 1)
            .Select(cycle => $"({string.Join(",", cycle.Select(point => point.ToString(System.Globalization.CultureInfo.InvariantCulture)))})");

        var result = string.Concat(cycles);
        return result.Length == 0 ? "()" : result;
    }

    private static bool TryParseTokenCycles(string text, out List<List<string>> cycles)
    {
        cycles = new List<List<string>>();

        if (text.Length == 0)
        {
            return false;
        }

        var position = 0;

        while (position < text.Length)
        {
            if (text[position] != '(')
            {
                return false;
            }

            position++;
            var cycle = new List<string>();

            if (!TryReadPointToken(text, ref position, out var firstToken))
            {
                return false;
            }

            cycle.Add(firstToken);

            if (position >= text.Length || text[position] != ',')
            {
                return false;
            }

            while (position < text.Length && text[position] == ',')
            {
                position++;

                if (!TryReadPointToken(text, ref position, out var token))
                {
                    return false;
                }

                cycle.Add(token);
            }

            if (position >= text.Length || text[position] != ')')
            {
                return false;
            }

            position++;
            cycles.Add(cycle);
        }

        return cycles.Count > 0;
    }

    private static bool TryReadPointToken(string text, ref int position, out string token)
    {
        token = string.Empty;

        if (position >= text.Length || text[position] is < '0' or > '9')
        {
            return false;
        }

        var start = position;

        if (text[position] == '0')
        {
            position++;

            if (position < text.Length && text[position] is >= '0' and <= '9')
            {
                return false;
            }
        }
        else
        {
            position++;

            while (position < text.Length && text[position] is >= '0' and <= '9')
            {
                position++;
            }
        }

        token = text[start..position];
        return true;
    }

    private static bool IsPointInRange(string token)
    {
        if (token == "0")
        {
            return false;
        }

        if (token.Length == 1)
        {
            return true;
        }

        return token.Length == 2 &&
               (token[0] < '6' || token[0] == '6' && token[1] <= '4');
    }

    private static int ParsePoint(string token)
    {
        return token.Length == 1
            ? token[0] - '0'
            : ((token[0] - '0') * 10) + token[1] - '0';
    }

    private static CayleyException InvalidPermutation(string text)
    {
        return new CayleyException(
            $"invalid permutation {Text.Show(text)} (expected cycles such as (1,2,3)(4,5), or ())");
    }
}
