namespace Cayleykit;

public static class ShowCommand
{
    public static int Run(IReadOnlyList<string> argv, IIo io)
    {
        var parsed = Options.Parse(argv, [new Spec("--degree", 1, 64, 0)]);
        if (parsed.Args.Count == 0)
        {
            throw new CayleyException("missing P");
        }

        if (parsed.Args.Count > 1)
        {
            throw new CayleyException("too many arguments");
        }

        var words = Words.Of(io, parsed.Args);
        if (words.Count > 1)
        {
            throw new CayleyException("too many arguments");
        }

        var (perms, degree) = Notation.ParseAll(words, parsed.Values["--degree"]);
        var perm = perms[0];
        var inverse = perm.Inverse();
        var invariant = System.Globalization.CultureInfo.InvariantCulture;
        var lines = new[]
        {
            Text.Line("perm", Notation.Format(perm)),
            Text.Line("degree", degree.ToString(invariant)),
            Text.Line("image", Structure.OneLine(perm)),
            Text.Line("order", perm.Order().ToString(invariant)),
            Text.Line("sign", Structure.Sign(perm)),
            Text.Line("type", Structure.CycleType(perm)),
            Text.Line("inverse", Notation.Format(inverse))
        };

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }
}
