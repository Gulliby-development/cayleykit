using System.Globalization;
using System.Numerics;

namespace Cayleykit;

public static class OrderCommand
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
        var basePoints = chain.Base.Count == 0 ? "none" : Text.Join(chain.Base);
        var sizes = chain.Sizes.Count == 0 ? "none" : Text.Join(chain.Sizes);
        var output = string.Concat(
            Text.Line("degree", degree.ToString(CultureInfo.InvariantCulture)), "\n",
            Text.Line("gens", generators.Count.ToString(CultureInfo.InvariantCulture)), "\n",
            Text.Line("order", chain.Order.ToString(CultureInfo.InvariantCulture)), "\n",
            Text.Line("base", basePoints), "\n",
            Text.Line("sizes", sizes), "\n");

        io.Out(output);
        return 0;
    }
}
