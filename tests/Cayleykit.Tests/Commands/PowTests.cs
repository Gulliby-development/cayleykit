using System.Diagnostics;
using System.Globalization;

namespace Cayleykit.Tests;

public class PowTests
{
    [Fact]
    public void Pow_matches_the_supplied_negative_exponent_example()
    {
        AssertSuccess(
            ["(1,2,3)(4,5)", "-7"],
            "perm        (1,2,3)(4,5)\n" +
            "order       6\n" +
            "power       (1,3,2)(4,5)\n" +
            "reduced     5\n");
    }

    [Fact]
    public void Pow_matches_the_supplied_large_exponent_example()
    {
        AssertSuccess(
            ["(1,2,3,4)", "1000000000000000000"],
            "perm        (1,2,3,4)\n" +
            "order       4\n" +
            "power       ()\n" +
            "reduced     0\n");
    }

    [Fact]
    public void Pow_matches_the_supplied_identity_example()
    {
        AssertSuccess(
            ["()", "5"],
            "perm        ()\n" +
            "order       1\n" +
            "power       ()\n" +
            "reduced     0\n");
    }

    [Fact]
    public void Pow_zero_exponent_returns_the_identity()
    {
        AssertSuccess(
            ["(1,2,3)", "0"],
            "perm        (1,2,3)\n" +
            "order       3\n" +
            "power       ()\n" +
            "reduced     0\n");
    }

    [Fact]
    public void Pow_reduces_a_positive_exponent_with_nonzero_remainder()
    {
        AssertSuccess(
            ["(1,2,3)", "5"],
            "perm        (1,2,3)\n" +
            "order       3\n" +
            "power       (1,3,2)\n" +
            "reduced     2\n");
    }

    [Fact]
    public void Pow_reduces_a_negative_exponent_to_the_mathematical_remainder()
    {
        AssertSuccess(
            ["(1,2,3)", "-1"],
            "perm        (1,2,3)\n" +
            "order       3\n" +
            "power       (1,3,2)\n" +
            "reduced     2\n");
    }

    [Fact]
    public void Pow_accepts_the_lower_exponent_bound()
    {
        AssertSuccess(
            ["(1,2,3)", "-1000000000000000000"],
            "perm        (1,2,3)\n" +
            "order       3\n" +
            "power       (1,3,2)\n" +
            "reduced     2\n");
    }

    [Fact]
    public void Pow_accepts_the_upper_exponent_bound()
    {
        AssertSuccess(
            ["(1,2,3)", "1000000000000000000"],
            "perm        (1,2,3)\n" +
            "order       3\n" +
            "power       (1,2,3)\n" +
            "reduced     1\n");
    }

    [Fact]
    public void Pow_handles_point_64_at_degree_64()
    {
        AssertSuccess(
            ["--degree=64", "(1,64)", "-1"],
            "perm        (1,64)\n" +
            "order       2\n" +
            "power       (1,64)\n" +
            "reduced     1\n");
    }

    [Fact]
    public void Pow_accepts_equals_and_repeated_degree_options()
    {
        AssertSuccess(
            ["--degree=8", "--degree", "6", "(1,2)", "1"],
            "perm        (1,2)\n" +
            "order       2\n" +
            "power       (1,2)\n" +
            "reduced     1\n");
    }

    [Fact]
    public void Pow_output_is_invariant_under_a_custom_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            culture.NumberFormat.NumberGroupSeparator = "_";
            CultureInfo.CurrentCulture = culture;

            AssertSuccess(
                ["(1,2,3)", "-7"],
                "perm        (1,2,3)\n" +
                "order       3\n" +
                "power       (1,3,2)\n" +
                "reduced     2\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void Pow_reports_missing_P()
    {
        AssertFailure("missing P");
    }

    [Fact]
    public void Pow_reports_missing_K_before_parsing_P()
    {
        AssertFailure("missing K", "(1,2");
    }

    [Fact]
    public void Pow_reports_too_many_arguments_before_parsing_operands()
    {
        AssertFailure("too many arguments", "(1,2", "bad", "extra");
    }

    [Fact]
    public void Pow_validates_options_before_missing_operands()
    {
        AssertFailure(
            "invalid --degree \"0\" (expected an integer from 1 to 64)",
            "--degree",
            "0");
    }

    [Fact]
    public void Pow_reports_a_missing_option_value_before_missing_operands()
    {
        AssertFailure("--degree needs a value", "--degree");
    }

    [Fact]
    public void Pow_reports_an_unknown_option_before_deferred_degree_validation()
    {
        AssertFailure(
            "unknown option \"--bogus\"",
            "--degree=0",
            "--bogus");
    }

    [Fact]
    public void Pow_reports_help_as_an_unknown_option()
    {
        AssertFailure("unknown option \"--help\"", "--help");
    }

    [Fact]
    public void Pow_reports_invalid_permutation_grammar_before_parsing_K()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(1,2",
            "1.5");
    }

    [Fact]
    public void Pow_reports_malformed_grammar_before_point_range()
    {
        AssertFailure(
            "invalid permutation \"(65,)\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(65,)",
            "bad");
    }

    [Fact]
    public void Pow_reports_invalid_point_zero_before_parsing_K()
    {
        AssertFailure(
            "invalid point \"0\" in \"(0,1)\" (expected 1 to 64)",
            "(0,1)",
            "bad");
    }

    [Fact]
    public void Pow_reports_repeated_points_before_parsing_K()
    {
        AssertFailure(
            "point 2 is repeated in \"(1,2)(2,3)\"",
            "(1,2)(2,3)",
            "bad");
    }

    [Fact]
    public void Pow_reports_explicit_degree_overflow_before_an_invalid_K()
    {
        AssertFailure(
            "point 5 of \"(1,5)\" is above the degree 3",
            "--degree",
            "3",
            "(1,5)",
            "1.5");
    }

    [Fact]
    public void Pow_treats_dash_as_an_ordinary_permutation_operand()
    {
        AssertFailure(
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())",
            "-",
            "1");
    }

    [Fact]
    public void Pow_rejects_negative_zero_as_a_noncanonical_integer()
    {
        AssertFailure(
            "invalid K \"-0\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "-0");
    }

    [Fact]
    public void Pow_rejects_a_leading_plus_sign()
    {
        AssertFailure(
            "invalid K \"+3\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "+3");
    }

    [Fact]
    public void Pow_rejects_leading_zeroes()
    {
        AssertFailure(
            "invalid K \"007\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "007");
    }

    [Fact]
    public void Pow_rejects_the_literal_TEXT_placeholder()
    {
        AssertFailure(
            "invalid K \"TEXT\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "TEXT");
    }

    [Fact]
    public void Pow_rejects_decimal_exponents()
    {
        AssertFailure(
            "invalid K \"1.5\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "1.5");
    }

    [Fact]
    public void Pow_rejects_an_empty_exponent()
    {
        AssertFailure(
            "invalid K \"\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "");
    }

    [Fact]
    public void Pow_rejects_non_ascii_digits()
    {
        AssertFailure(
            "invalid K \"?\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "٣");
    }

    [Fact]
    public void Pow_rejects_the_value_above_the_upper_exponent_bound()
    {
        AssertFailure(
            "invalid K \"1000000000000000001\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "1000000000000000001");
    }

    [Fact]
    public void Pow_rejects_the_value_below_the_lower_exponent_bound()
    {
        AssertFailure(
            "invalid K \"-1000000000000000001\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            "()",
            "-1000000000000000001");
    }

    [Fact]
    public void Pow_computes_a_literal_bound_exponent_within_ten_seconds()
    {
        var io = new FakeIo();
        var timer = Stopwatch.StartNew();

        var result = PowCommand.Run(["--degree=64", "(1,64)", "1000000000000000000"], io);

        timer.Stop();
        Assert.True(timer.Elapsed < TimeSpan.FromSeconds(10), $"Bound exponentiation took {timer.Elapsed}.");
        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "perm        (1,64)\n" +
                "order       2\n" +
                "power       ()\n" +
                "reduced     0\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = PowCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => PowCommand.Run(args, io));

        Assert.Equal(expectedMessage, exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private sealed class FakeIo : IIo
    {
        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public int ReadCount { get; private set; }

        public byte[] ReadStdin()
        {
            ReadCount++;
            return [];
        }

        public void Out(string text) => Output.Add(text);

        public void Err(string text) => Errors.Add(text);
    }
}
