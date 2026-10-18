using System.Globalization;
using System.Text;

namespace Cayleykit.Tests;

public class ShowTests
{
    [Fact]
    public void Show_matches_the_supplied_disjoint_cycle_example()
    {
        AssertSuccess(
            ["(1,2,3)(4,5)"],
            "perm        (1,2,3)(4,5)\n" +
            "degree      5\n" +
            "image       2 3 1 5 4\n" +
            "order       6\n" +
            "sign        odd\n" +
            "type        3 2\n" +
            "inverse     (1,3,2)(4,5)\n");
    }

    [Fact]
    public void Show_matches_the_supplied_identity_example()
    {
        AssertSuccess(
            ["()"],
            "perm        ()\n" +
            "degree      1\n" +
            "image       1\n" +
            "order       1\n" +
            "sign        even\n" +
            "type        1\n" +
            "inverse     ()\n");
    }

    [Fact]
    public void Show_matches_the_supplied_explicit_degree_example()
    {
        AssertSuccess(
            ["--degree", "6", "(1,2)"],
            "perm        (1,2)\n" +
            "degree      6\n" +
            "image       2 1 3 4 5 6\n" +
            "order       2\n" +
            "sign        odd\n" +
            "type        2 1 1 1 1\n" +
            "inverse     (1,2)\n");
    }

    [Fact]
    public void Show_canonicalizes_cycle_rotation_and_cycle_order()
    {
        AssertSuccess(
            ["(3,1,2)(5,4)"],
            "perm        (1,2,3)(4,5)\n" +
            "degree      5\n" +
            "image       2 3 1 5 4\n" +
            "order       6\n" +
            "sign        odd\n" +
            "type        3 2\n" +
            "inverse     (1,3,2)(4,5)\n");
    }

    [Fact]
    public void Show_accepts_the_equals_form_of_degree_option()
    {
        AssertSuccess(
            ["--degree=8", "(1,2)"],
            "perm        (1,2)\n" +
            "degree      8\n" +
            "image       2 1 3 4 5 6 7 8\n" +
            "order       2\n" +
            "sign        odd\n" +
            "type        2 1 1 1 1 1 1\n" +
            "inverse     (1,2)\n");
    }

    [Fact]
    public void Show_uses_the_last_repeated_degree_option()
    {
        AssertSuccess(
            ["--degree", "8", "--degree=4", "(1,2)"],
            "perm        (1,2)\n" +
            "degree      4\n" +
            "image       2 1 3 4\n" +
            "order       2\n" +
            "sign        odd\n" +
            "type        2 1 1\n" +
            "inverse     (1,2)\n");
    }

    [Fact]
    public void Show_accepts_degree_and_point_64()
    {
        AssertSuccess(
            ["--degree=64", "(1,64)"],
            "perm        (1,64)\n" +
            "degree      64\n" +
            "image       64 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61 62 63 1\n" +
            "order       2\n" +
            "sign        odd\n" +
            "type        2 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1 1 1 1 1 1 1 1 1 " +
            "1 1\n" +
            "inverse     (1,64)\n");
    }

    [Fact]
    public void Show_output_is_invariant_under_a_custom_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            culture.NumberFormat.NumberGroupSeparator = "_";
            CultureInfo.CurrentCulture = culture;

            AssertSuccess(
                ["--degree=6", "(1,2)"],
                "perm        (1,2)\n" +
                "degree      6\n" +
                "image       2 1 3 4 5 6\n" +
                "order       2\n" +
                "sign        odd\n" +
                "type        2 1 1 1 1\n" +
                "inverse     (1,2)\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void Show_reports_missing_permutation()
    {
        AssertFailure("missing P");
    }

    [Fact]
    public void Show_checks_excess_operands_before_parsing_them()
    {
        AssertFailure("too many arguments", "(1,2", "()");
    }

    [Fact]
    public void Show_validates_options_before_missing_operands()
    {
        AssertFailure("invalid --degree \"0\" (expected an integer from 1 to 64)", "--degree", "0");
    }

    [Fact]
    public void Show_reports_a_missing_option_value_before_missing_permutation()
    {
        AssertFailure("--degree needs a value", "--degree");
    }

    [Fact]
    public void Show_rejects_help_as_an_unknown_option()
    {
        AssertFailure("unknown option \"--help\"", "--help");
    }

    [Fact]
    public void Show_reports_an_unknown_option_before_deferred_degree_validation()
    {
        AssertFailure("unknown option \"--bogus\"", "--degree=0", "--bogus");
    }

    [Fact]
    public void Show_reports_invalid_permutation_grammar()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(1,2");
    }

    [Fact]
    public void Show_checks_grammar_before_reporting_a_point_range_error()
    {
        AssertFailure(
            "invalid permutation \"(65,)\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(65,)");
    }

    [Fact]
    public void Show_reports_a_point_above_the_notation_limit()
    {
        AssertFailure("invalid point \"65\" in \"(1,65)\" (expected 1 to 64)", "(1,65)");
    }

    [Fact]
    public void Show_reports_zero_as_an_invalid_point()
    {
        AssertFailure("invalid point \"0\" in \"(0,1)\" (expected 1 to 64)", "(0,1)");
    }

    [Fact]
    public void Show_checks_point_range_before_duplicate_points()
    {
        AssertFailure("invalid point \"65\" in \"(65,65)\" (expected 1 to 64)", "(65,65)");
    }

    [Fact]
    public void Show_reports_repeated_points_across_cycles()
    {
        AssertFailure("point 2 is repeated in \"(1,2)(2,3)\"", "(1,2)(2,3)");
    }

    [Fact]
    public void Show_reports_a_point_above_an_explicit_degree()
    {
        AssertFailure("point 5 of \"(1,5)\" is above the degree 3", "--degree", "3", "(1,5)");
    }

    [Fact]
    public void Show_reads_a_BOM_prefixed_CRLF_permutation_from_stdin()
    {
        var stdin = new byte[] { 0xEF, 0xBB, 0xBF }
            .Concat(Encoding.UTF8.GetBytes("(1,2,3)(4,5)\r\n"))
            .ToArray();
        var io = new FakeIo(stdin);

        var result = ShowCommand.Run(["-"], io);

        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "perm        (1,2,3)(4,5)\n" +
                "degree      5\n" +
                "image       2 3 1 5 4\n" +
                "order       6\n" +
                "sign        odd\n" +
                "type        3 2\n" +
                "inverse     (1,3,2)(4,5)\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Show_rejects_multiple_permutations_from_stdin()
    {
        var io = new FakeIo(Encoding.UTF8.GetBytes("(1,2) (3,4)"));

        var exception = Assert.Throws<CayleyException>(() => ShowCommand.Run(["-"], io));

        Assert.Equal("too many arguments", exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Show_reports_empty_stdin()
    {
        var io = new FakeIo([]);

        var exception = Assert.Throws<CayleyException>(() => ShowCommand.Run(["-"], io));

        Assert.Equal("<stdin>: no input", exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Show_checks_too_many_arguments_before_reading_stdin()
    {
        var io = new FakeIo(Encoding.UTF8.GetBytes("()"));

        var exception = Assert.Throws<CayleyException>(() => ShowCommand.Run(["-", "()"], io));

        Assert.Equal("too many arguments", exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = ShowCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => ShowCommand.Run(args, io));

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
