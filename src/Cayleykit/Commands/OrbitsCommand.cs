using System.Globalization;
using System.Numerics;

namespace Cayleykit;

public static class OrbitsCommand
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
        var chain = Chain.Build(degree, generators);
        var orbits = Orbits.Of(degree, generators);
        var lines = new List<string>
        {
            Text.Line("degree", degree.ToString(CultureInfo.InvariantCulture)),
            Text.Line("order", chain.Order.ToString(CultureInfo.InvariantCulture)),
            Text.Line("orbits", orbits.Count.ToString(CultureInfo.InvariantCulture))
        };

        foreach (var orbit in orbits)
        {
            var points = string.Join(",", orbit.Select(point => point.ToString(CultureInfo.InvariantCulture)));
            var stabilizerOrder = chain.Order / new BigInteger(orbit.Count);
            var value = $"{{{points}}}  size {orbit.Count.ToString(CultureInfo.InvariantCulture)}  stab {stabilizerOrder.ToString(CultureInfo.InvariantCulture)}";
            lines.Add(Text.Line("orbit", value));
        }

        lines.Add(Text.Line("transitive", orbits.Count == 1 ? "yes" : "no"));
        var output = string.Concat(string.Join("\n", lines), "\n");

        io.Out(output);
        return 0;
    }
}
