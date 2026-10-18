using System.Globalization;
using System.Numerics;

namespace Cayleykit;

public static class ElementsCommand
{
    private static readonly IReadOnlyList<Spec> DegreeOptions =
    [
        new Spec("--degree", 1, 64, 0)
    ];

    public static int Run(IReadOnlyList<string> argv, IIo io)
    {
        var options = Options.Parse(argv, DegreeOptions);
        var args = options.Args;

        if (args.Count == 0)
        {
            throw new CayleyException("missing G");
        }

        var generatorTexts = Words.Of(io, args);
        if (generatorTexts.Count > 8)
        {
            throw new CayleyException("too many generators (at most 8)");
        }

        var parsed = Notation.ParseAll(generatorTexts, options.Values["--degree"]);
        var generators = parsed.Perms;
        var groupOrder = Chain.Build(parsed.Degree, generators).Order;

        if (groupOrder > 500)
        {
            throw TooLarge(groupOrder);
        }

        var elements = Closure.Elements(parsed.Degree, generators, 500);
        if (elements is null)
        {
            throw TooLarge(groupOrder);
        }

        var lines = new List<string>
        {
            Text.Line("order", groupOrder.ToString(CultureInfo.InvariantCulture))
        };
        lines.AddRange(elements.Select(Notation.Format));

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }

    private static CayleyException TooLarge(BigInteger order)
    {
        return new CayleyException(
            $"group order {order.ToString(CultureInfo.InvariantCulture)} is above 500 (too large for elements)");
    }
}
