namespace Cayleykit;

public static class ConjCommand
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

        if (parsed.Args.Count > 2)
        {
            throw new CayleyException("too many arguments");
        }

        var (perms, _) = Notation.ParseAll(parsed.Args, parsed.Values["--degree"]);
        var perm = perms[0];
        var by = perms[1];
        var conjugate = Structure.Conjugate(perm, by);
        var commutator = Structure.Commutator(perm, by);

        var lines = new[]
        {
            Text.Line("perm", Notation.Format(perm)),
            Text.Line("by", Notation.Format(by)),
            Text.Line("conj", Notation.Format(conjugate)),
            Text.Line("comm", Notation.Format(commutator)),
            Text.Line("commutes", commutator.IsIdentity ? "yes" : "no")
        };

        io.Out(string.Join("\n", lines) + "\n");
        return 0;
    }
}
