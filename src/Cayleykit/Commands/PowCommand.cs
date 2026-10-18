namespace Cayleykit;

public static class PowCommand
{
    public static int Run(IReadOnlyList<string> argv, IIo io)
    {
        var parsed = Options.Parse(argv, [new Spec("--degree", 1, 64, 0)]);
        if (parsed.Args.Count == 0)
        {
            throw new CayleyException("missing P");
        }

        if (parsed.Args.Count == 1)
        {
            throw new CayleyException("missing K");
        }

        if (parsed.Args.Count > 2)
        {
            throw new CayleyException("too many arguments");
        }

        var (perms, _) = Notation.ParseAll([parsed.Args[0]], parsed.Values["--degree"]);
        var exponent = Ints.Parse("K", parsed.Args[1], -1000000000000000000, 1000000000000000000);
        var perm = perms[0];
        var order = perm.Order();
        var reduced = exponent % order;
        if (reduced < 0)
        {
            reduced += order;
        }

        var invariant = System.Globalization.CultureInfo.InvariantCulture;
        var lines = new[]
        {
            Text.Line("perm", Notation.Format(perm)),
            Text.Line("order", order.ToString(invariant)),
            Text.Line("power", Notation.Format(perm.Power(exponent))),
            Text.Line("reduced", reduced.ToString(invariant))
        };

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }
}
