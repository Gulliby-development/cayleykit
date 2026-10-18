using System.Globalization;
using System.Numerics;

namespace Cayleykit;

public static class WordCommand
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
            throw new CayleyException("missing P");
        }

        if (args.Count == 1)
        {
            throw new CayleyException("missing G");
        }

        var generatorTexts = Words.Of(io, args.Skip(1).ToList());
        if (generatorTexts.Count > 8)
        {
            throw new CayleyException("too many generators (at most 8)");
        }

        var texts = new List<string>(generatorTexts.Count + 1) { args[0] };
        texts.AddRange(generatorTexts);

        var parsed = Notation.ParseAll(texts, options.Values["--degree"]);
        var target = parsed.Perms[0];
        var generators = parsed.Perms.Skip(1).ToList();
        var groupOrder = Chain.Build(parsed.Degree, generators).Order;

        if (groupOrder > 100000)
        {
            throw TooLarge(groupOrder);
        }

        var word = WordSearch.Find(target, generators);
        var lines = new List<string>
        {
            Text.Line("order", groupOrder.ToString(CultureInfo.InvariantCulture))
        };

        if (word is null)
        {
            lines.Add(Text.Line("status", "not in the group"));
            io.Out(string.Join("\n", lines) + "\n");
            return 1;
        }

        lines.Add(Text.Line("length", word.Count.ToString(CultureInfo.InvariantCulture)));
        var wordText = word.Count == 0
            ? "(empty)"
            : string.Join(" ", word.Select(number => "g" + number.ToString(CultureInfo.InvariantCulture)));
        lines.Add(Text.Line("word", wordText));

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }

    private static CayleyException TooLarge(BigInteger order)
    {
        return new CayleyException(
            $"group order {order.ToString(CultureInfo.InvariantCulture)} is above 100000 (too large for word)");
    }
}
