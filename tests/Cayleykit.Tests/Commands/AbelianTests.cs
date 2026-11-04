using System.Text;

namespace Cayleykit.Tests;

public class AbelianTests
{
    [Fact]
    public void Abelian_says_yes_for_a_single_generator()
    {
        AssertSuccess(
            ["(1,2,3)(4,5)"],
            "degree      5\n" +
            "gens        1\n" +
            "abelian     yes\n");
    }

    [Fact]
    public void Abelian_says_yes_for_disjoint_generators()
    {
        AssertSuccess(
            ["(1,2)", "(3,4)", "(5,6,7)"],
            "degree      7\n" +
            "gens        3\n" +
            "abelian     yes\n");
    }

    [Fact]
    public void Abelian_says_yes_for_a_generator_and_its_power()
    {
        AssertSuccess(
            ["(1,2,3,4)", "(1,3)(2,4)"],
            "degree      4\n" +
            "gens        2\n" +
            "abelian     yes\n");
    }

    [Fact]
    public void Abelian_names_the_first_pair_that_does_not_commute()
    {
        AssertSuccess(
            ["(1,2)", "(1,3)"],
            "degree      3\n" +
            "gens        2\n" +
            "abelian     no\n" +
            "witness     g1 g2\n" +
            "comm        (1,3,2)\n");
    }

    [Fact]
    public void Abelian_checks_the_pairs_in_generator_order()
    {
        AssertSuccess(
            ["(1,2)", "(3,4)", "(2,3)", "(5,6)"],
            "degree      6\n" +
            "gens        4\n" +
            "abelian     no\n" +
            "witness     g1 g3\n" +
            "comm        (1,2,3)\n");
    }

    [Fact]
    public void Abelian_reports_a_later_pair_when_the_first_ones_commute()
    {
        AssertSuccess(
            ["(1,2)", "(3,4)", "(1,2)(3,4)", "(4,5)"],
            "degree      5\n" +
            "gens        4\n" +
            "abelian     no\n" +
            "witness     g2 g4\n" +
            "comm        (3,4,5)\n");
    }

    [Fact]
    public void Abelian_accepts_the_identity_and_an_explicit_degree()
    {
        AssertSuccess(
            ["--degree", "6", "()", "(1,2)"],
            "degree      6\n" +
            "gens        2\n" +
            "abelian     yes\n");
    }

    [Fact]
    public void Abelian_reads_the_generators_from_stdin()
    {
        var io = new FakeIo(Encoding.UTF8.GetBytes("(1,2)\n(1,3)\n"));

        var result = AbelianCommand.Run(["-"], io);

        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "degree      3\n" +
                "gens        2\n" +
                "abelian     no\n" +
                "witness     g1 g2\n" +
                "comm        (1,3,2)\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Abelian_accepts_eight_generators_and_rejects_nine()
    {
        AssertSuccess(
            ["(1,2)", "(3,4)", "(5,6)", "(7,8)", "(9,10)", "(11,12)", "(13,14)", "(15,16)"],
            "degree      16\n" +
            "gens        8\n" +
            "abelian     yes\n");
        AssertFailure(
            "too many generators (at most 8)",
            "(1,2)", "(3,4)", "(5,6)", "(7,8)", "(9,10)", "(11,12)", "(13,14)", "(15,16)", "(17,18)");
    }

    [Fact]
    public void Abelian_reports_missing_generators()
    {
        AssertFailure("missing G");
    }

    [Fact]
    public void Abelian_reports_an_unknown_option()
    {
        AssertFailure("unknown option \"--bogus\"", "--bogus", "(1,2)");
    }

    [Fact]
    public void Abelian_reports_a_point_above_the_degree()
    {
        AssertFailure("point 4 of \"(1,4)\" is above the degree 3", "--degree", "3", "(1,2)", "(1,4)");
    }

    [Fact]
    public void Cli_dispatches_the_abelian_command()
    {
        var io = new FakeIo();

        var result = Cli.Run(["abelian", "(1,2)", "(1,3)"], io);

        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "degree      3\n" +
                "gens        2\n" +
                "abelian     no\n" +
                "witness     g1 g2\n" +
                "comm        (1,3,2)\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = AbelianCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => AbelianCommand.Run(args, io));

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
