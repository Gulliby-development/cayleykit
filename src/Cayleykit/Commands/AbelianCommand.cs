using System.Globalization;

namespace Cayleykit;

public static class AbelianCommand
{
    public static int Run(IReadOnlyList<string> argv, IIo io)
    {
        var parsed = Options.Parse(argv, [new Spec("--degree", 1, 64, 0)]);
        if (parsed.Args.Count == 0)
        {
            throw new CayleyException("missing G");
        }

        var generatorTexts = Words.Of(io, parsed.Args);
        if (generatorTexts.Count > 8)
        {
            throw new CayleyException("too many generators (at most 8)");
        }

        var (generators, degree) = Notation.ParseAll(generatorTexts, parsed.Values["--degree"]);
        var invariant = CultureInfo.InvariantCulture;
        var lines = new List<string>
        {
            Text.Line("degree", degree.ToString(invariant)),
            Text.Line("gens", generators.Count.ToString(invariant))
        };

        var pair = FirstNonCommutingPair(generators);
        if (pair is null)
        {
            lines.Add(Text.Line("abelian", "yes"));
        }
        else
        {
            var (left, right) = pair.Value;
            var commutator = Structure.Commutator(generators[left], generators[right]);
            lines.Add(Text.Line("abelian", "no"));
            lines.Add(Text.Line("witness", $"g{(left + 1).ToString(invariant)} g{(right + 1).ToString(invariant)}"));
            lines.Add(Text.Line("comm", Notation.Format(commutator)));
        }

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }

    private static (int Left, int Right)? FirstNonCommutingPair(IReadOnlyList<Perm> generators)
    {
        for (var left = 0; left < generators.Count; left++)
        {
            for (var right = left + 1; right < generators.Count; right++)
            {
                if (!Structure.Commutator(generators[left], generators[right]).IsIdentity)
                {
                    return (left, right);
                }
            }
        }

        return null;
    }
}
