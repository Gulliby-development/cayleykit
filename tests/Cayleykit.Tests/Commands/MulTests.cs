using System.Diagnostics;
using System.Globalization;

namespace Cayleykit.Tests;

public class MulTests
{
    [Fact]
    public void Mul_matches_the_first_supplied_product_example()
    {
        AssertSuccess(
            ["(1,2,3)", "(1,2)"],
            "product     (2,3)\n" +
            "degree      3\n" +
            "order       2\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_matches_the_second_supplied_product_example()
    {
        AssertSuccess(
            ["(1,2)", "(2,3)", "(3,4)"],
            "product     (1,4,3,2)\n" +
            "degree      4\n" +
            "order       4\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_respects_the_reverse_order_of_the_asymmetric_product()
    {
        AssertSuccess(
            ["(1,2)", "(1,2,3)"],
            "product     (1,3)\n" +
            "degree      3\n" +
            "order       2\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_multiplies_two_identities()
    {
        AssertSuccess(
            ["()", "()"],
            "product     ()\n" +
            "degree      1\n" +
            "order       1\n" +
            "sign        even\n");
    }

    [Fact]
    public void Mul_accepts_exactly_eight_explicit_operands()
    {
        AssertSuccess(
            ["(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)"],
            "product     ()\n" +
            "degree      2\n" +
            "order       1\n" +
            "sign        even\n");
    }

    [Fact]
    public void Mul_accepts_the_equals_degree_option()
    {
        AssertSuccess(
            ["--degree=8", "(1,2)", "()"],
            "product     (1,2)\n" +
            "degree      8\n" +
            "order       2\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_uses_the_last_repeated_degree_option()
    {
        AssertSuccess(
            ["--degree", "8", "--degree=4", "(1,2)", "()"],
            "product     (1,2)\n" +
            "degree      4\n" +
            "order       2\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_output_is_invariant_under_a_custom_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            culture.NumberFormat.NumberGroupSeparator = "_";
            CultureInfo.CurrentCulture = culture;

            AssertSuccess(
                ["(1,2,3)", "(1,2)"],
                "product     (2,3)\n" +
                "degree      3\n" +
                "order       2\n" +
                "sign        odd\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void Mul_reports_missing_P()
    {
        AssertFailure("missing P");
    }

    [Fact]
    public void Mul_reports_missing_Q_for_one_operand()
    {
        AssertFailure("missing Q", "()");
    }

    [Fact]
    public void Mul_reports_too_many_operands_before_parsing_them()
    {
        AssertFailure(
            "too many arguments",
            "(1,2",
            "()",
            "()",
            "()",
            "()",
            "()",
            "()",
            "()",
            "()");
    }

    [Fact]
    public void Mul_validates_options_before_missing_operands()
    {
        AssertFailure(
            "invalid --degree \"0\" (expected an integer from 1 to 64)",
            "--degree",
            "0");
    }

    [Fact]
    public void Mul_reports_a_missing_option_value_before_missing_operands()
    {
        AssertFailure("--degree needs a value", "--degree");
    }

    [Fact]
    public void Mul_reports_unknown_options_before_deferred_degree_validation()
    {
        AssertFailure(
            "unknown option \"--bogus\"",
            "--degree=0",
            "--bogus");
    }

    [Fact]
    public void Mul_reports_invalid_permutation_grammar()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(1,2",
            "()");
    }

    [Fact]
    public void Mul_reports_a_point_above_the_notation_limit()
    {
        AssertFailure(
            "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)",
            "(1,65)",
            "()");
    }

    [Fact]
    public void Mul_reports_repeated_points_before_computing_the_product()
    {
        AssertFailure(
            "point 2 is repeated in \"(1,2)(2,3)\"",
            "(1,2)(2,3)",
            "()");
    }

    [Fact]
    public void Mul_checks_every_text_before_reporting_an_earlier_degree_overflow()
    {
        AssertFailure(
            "invalid permutation \"(2,\" (expected cycles such as (1,2,3)(4,5), or ())",
            "--degree=3",
            "(1,5)",
            "(2,");
    }

    [Fact]
    public void Mul_reports_an_explicit_degree_overflow_after_parsing_all_valid_texts()
    {
        AssertFailure(
            "point 5 of \"(1,5)\" is above the degree 3",
            "--degree",
            "3",
            "(1,5)",
            "()");
    }

    [Fact]
    public void Mul_treats_dash_as_an_operand_without_reading_stdin()
    {
        AssertFailure(
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())",
            "-",
            "()");
    }

    [Fact]
    public void Mul_accepts_point_64_at_explicit_degree_64()
    {
        AssertSuccess(
            ["--degree=64", "(1,64)", "()"],
            "product     (1,64)\n" +
            "degree      64\n" +
            "order       2\n" +
            "sign        odd\n");
    }

    [Fact]
    public void Mul_computes_a_literal_degree_64_product_within_ten_seconds()
    {
        var io = new FakeIo();
        var timer = Stopwatch.StartNew();

        var result = MulCommand.Run(["--degree=64", "(1,64)", "()"], io);

        timer.Stop();
        Assert.True(timer.Elapsed < TimeSpan.FromSeconds(10), $"Degree-64 multiplication took {timer.Elapsed}.");
        Assert.Equal(0, result);
        Assert.Equal(
            new[]
            {
                "product     (1,64)\n" +
                "degree      64\n" +
                "order       2\n" +
                "sign        odd\n"
            },
            io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = MulCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => MulCommand.Run(args, io));

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
