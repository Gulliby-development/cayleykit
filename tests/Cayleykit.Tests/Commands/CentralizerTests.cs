using System.Text;

namespace Cayleykit.Tests;

public class CentralizerTests
{
    [Fact]
    public void Centralizer_matches_the_worked_example()
    {
        AssertSuccess(
            ["--degree", "7", "(1,2,3)(4,5)"],
            "perm        (1,2,3)(4,5)\n" +
            "degree      7\n" +
            "type        3 2 1 1\n" +
            "commute     12\n" +
            "class       420\n");
    }

    [Fact]
    public void Centralizer_infers_the_degree_from_the_largest_point()
    {
        AssertSuccess(
            ["(1,2,3)(4,5)"],
            "perm        (1,2,3)(4,5)\n" +
            "degree      5\n" +
            "type        3 2\n" +
            "commute     6\n" +
            "class       20\n");
    }

    [Fact]
    public void Centralizer_counts_equal_cycle_lengths()
    {
        AssertSuccess(
            ["(1,2)(3,4)"],
            "perm        (1,2)(3,4)\n" +
            "degree      4\n" +
            "type        2 2\n" +
            "commute     8\n" +
            "class       3\n");
    }

    [Fact]
    public void Centralizer_accepts_the_equals_form_for_the_identity()
    {
        AssertSuccess(
            ["--degree=4", "()"],
            "perm        ()\n" +
            "degree      4\n" +
            "type        1 1 1 1\n" +
            "commute     24\n" +
            "class       1\n");
    }

    [Fact]
    public void Centralizer_is_exact_at_degree_64()
    {
        AssertSuccess(
            ["--degree", "64", "()"],
            "perm        ()\n" +
            "degree      64\n" +
            "type        " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1\n" +
            "commute     126886932185884164103433389335161480802865516174545192198801894375214704230400000000000000\n" +
            "class       1\n");
    }

    [Fact]
    public void Centralizer_reads_the_permutation_from_stdin()
    {
        var io = new FakeIo(Encoding.UTF8.GetBytes("(1,2,3)\n"));

        var result = CentralizerCommand.Run(["-"], io);

        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "perm        (1,2,3)\n" +
                "degree      3\n" +
                "type        3\n" +
                "commute     3\n" +
                "class       2\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Centralizer_reports_missing_permutation()
    {
        AssertFailure("missing P");
    }

    [Fact]
    public void Centralizer_reports_too_many_arguments()
    {
        AssertFailure("too many arguments", "(1,2)", "(3,4)");
    }

    [Fact]
    public void Centralizer_reports_an_unknown_option()
    {
        AssertFailure("unknown option \"--bogus\"", "--bogus", "(1,2)");
    }

    [Fact]
    public void Centralizer_reports_an_invalid_degree()
    {
        AssertFailure("invalid --degree \"65\" (expected an integer from 1 to 64)", "--degree", "65", "(1,2)");
    }

    [Fact]
    public void Centralizer_reports_a_point_above_the_degree()
    {
        AssertFailure("point 4 of \"(1,4)\" is above the degree 3", "--degree", "3", "(1,4)");
    }

    [Fact]
    public void Cli_dispatches_the_centralizer_command()
    {
        var io = new FakeIo();

        var result = Cli.Run(["centralizer", "(1,2)(3,4)"], io);

        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "perm        (1,2)(3,4)\n" +
                "degree      4\n" +
                "type        2 2\n" +
                "commute     8\n" +
                "class       3\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = CentralizerCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => CentralizerCommand.Run(args, io));

        Assert.Equal(expectedMessage, exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private sealed class FakeIo(byte[]? stdin = null) : IIo
    {
        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public int ReadCount { get; private set; }

        public byte[] ReadStdin()
        {
            ReadCount++;
            return stdin ?? [];
        }

        public void Out(string text) => Output.Add(text);

        public void Err(string text) => Errors.Add(text);
    }
}
