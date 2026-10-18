namespace Cayleykit;

public sealed record Spec(string Name, long Lo, long Hi, long Fallback);

public sealed record Parsed(IReadOnlyDictionary<string, long> Values, IReadOnlyList<string> Args);

public static class Options
{
    public static Parsed Parse(IReadOnlyList<string> argv, IReadOnlyList<Spec> specs)
    {
        var specsByName = new Dictionary<string, Spec>(StringComparer.Ordinal);
        foreach (var spec in specs)
        {
            specsByName.Add(spec.Name, spec);
        }

        var suppliedValues = new Dictionary<string, string>(StringComparer.Ordinal);
        var args = new List<string>();

        for (var index = 0; index < argv.Count; index++)
        {
            var argument = argv[index];
            if (!argument.StartsWith("--", StringComparison.Ordinal))
            {
                args.Add(argument);
                continue;
            }

            var equalsIndex = argument.IndexOf('=');
            var name = equalsIndex >= 0 ? argument[..equalsIndex] : argument;
            if (!specsByName.ContainsKey(name))
            {
                throw new CayleyException($"unknown option {Text.Show(argument)}");
            }

            string value;
            if (equalsIndex >= 0)
            {
                value = argument[(equalsIndex + 1)..];
            }
            else
            {
                if (index + 1 >= argv.Count)
                {
                    throw new CayleyException($"{name} needs a value");
                }

                value = argv[++index];
            }

            suppliedValues[name] = value;
        }

        var values = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var spec in specs)
        {
            values[spec.Name] = suppliedValues.TryGetValue(spec.Name, out var text)
                ? Ints.Parse(spec.Name, text, spec.Lo, spec.Hi)
                : spec.Fallback;
        }

        return new Parsed(values, args);
    }
}
