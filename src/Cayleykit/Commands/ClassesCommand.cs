using System.Globalization;
using System.Numerics;

namespace Cayleykit;

public static class ClassesCommand
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

        if (groupOrder > 720)
        {
            throw TooLarge(groupOrder);
        }

        var elements = Closure.Elements(parsed.Degree, generators, 720);
        if (elements is null)
        {
            throw TooLarge(groupOrder);
        }

        var classes = Classes.Of(elements, generators);
        var centerSize = classes.Count(conjugacyClass => conjugacyClass.Size == 1);
        var lines = new List<string>
        {
            Text.Line("degree", parsed.Degree.ToString(CultureInfo.InvariantCulture)),
            Text.Line("order", groupOrder.ToString(CultureInfo.InvariantCulture)),
            Text.Line("classes", classes.Count.ToString(CultureInfo.InvariantCulture)),
            Text.Line("center", centerSize.ToString(CultureInfo.InvariantCulture)),
            Text.Line("abelian", new BigInteger(centerSize) == groupOrder ? "yes" : "no")
        };

        lines.AddRange(classes.Select(conjugacyClass =>
            Text.Line(
                "class",
                $"size {conjugacyClass.Size.ToString(CultureInfo.InvariantCulture)}  " +
                $"order {conjugacyClass.Representative.Order().ToString(CultureInfo.InvariantCulture)}  " +
                $"type {Structure.CycleType(conjugacyClass.Representative)}  " +
                $"rep {Notation.Format(conjugacyClass.Representative)}")));

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }

    private static CayleyException TooLarge(BigInteger order)
    {
        return new CayleyException(
            $"group order {order.ToString(CultureInfo.InvariantCulture)} is above 720 (too large for classes)");
    }
}
