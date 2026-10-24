namespace Cayleykit;

public static class Cli
{
    public const string Usage =
        "usage: cayleykit show [--degree D] P\n" +
        "       cayleykit mul [--degree D] P Q...\n" +
        "       cayleykit pow [--degree D] P K\n" +
        "       cayleykit conj [--degree D] P Q\n" +
        "       cayleykit orbits [--degree D] G...\n" +
        "       cayleykit order [--degree D] G...\n" +
        "       cayleykit word [--degree D] P G...\n" +
        "       cayleykit elements [--degree D] G...\n" +
        "       cayleykit classes [--degree D] G...\n" +
        "       cayleykit centralizer [--degree D] P";

    private static readonly Dictionary<string, Func<IReadOnlyList<string>, IIo, int>> Commands =
        new(StringComparer.Ordinal)
        {
            ["show"] = ShowCommand.Run,
            ["mul"] = MulCommand.Run,
            ["pow"] = PowCommand.Run,
            ["conj"] = ConjCommand.Run,
            ["orbits"] = OrbitsCommand.Run,
            ["order"] = OrderCommand.Run,
            ["word"] = WordCommand.Run,
            ["elements"] = ElementsCommand.Run,
            ["classes"] = ClassesCommand.Run,
            ["centralizer"] = CentralizerCommand.Run
        };

    public static int Run(string[] args, IIo io)
    {
        try
        {
            if (args.Length == 0)
            {
                throw new CayleyException(
                    "missing command (show, mul, pow, conj, orbits, order, word, elements, classes or centralizer)");
            }

            if (args.Length == 1 && args[0] == "--help")
            {
                io.Out(Usage + "\n");
                return 0;
            }

            if (args[0] == "--help")
            {
                throw new CayleyException($"unknown command {Text.Show(args[0])}");
            }

            if (!Commands.TryGetValue(args[0], out var command))
            {
                throw new CayleyException($"unknown command {Text.Show(args[0])}");
            }

            return command(args[1..], io);
        }
        catch (CayleyException exception)
        {
            io.Err($"cayleykit: {exception.Message}\n");
            return 2;
        }
    }
}
