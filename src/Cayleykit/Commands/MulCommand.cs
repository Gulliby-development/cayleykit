namespace Cayleykit;

public static class MulCommand
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
            throw new CayleyException("missing Q");
        }

        if (parsed.Args.Count > 8)
        {
            throw new CayleyException("too many arguments");
        }

        var (perms, degree) = Notation.ParseAll(parsed.Args, parsed.Values["--degree"]);
        var product = perms[0];

        for (var index = 1; index < perms.Count; index++)
        {
            product = product.Then(perms[index]);
        }

        var invariant = System.Globalization.CultureInfo.InvariantCulture;
        var lines = new[]
        {
            Text.Line("product", Notation.Format(product)),
            Text.Line("degree", degree.ToString(invariant)),
            Text.Line("order", product.Order().ToString(invariant)),
            Text.Line("sign", Structure.Sign(product))
        };

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }
}
