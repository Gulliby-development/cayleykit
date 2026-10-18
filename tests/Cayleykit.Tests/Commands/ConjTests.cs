using System.Globalization;

namespace Cayleykit.Tests;

public class ConjTests
{
    [Fact]
    public void Conj_matches_the_supplied_noncommuting_example()
    {
        AssertSuccess(
            ["(1,2,3)", "(1,2)"],
            "perm        (1,2,3)\n" +
            "by          (1,2)\n" +
            "conj        (1,3,2)\n" +
            "comm        (1,2,3)\n" +
            "commutes    no\n");
    }

    [Fact]
    public void Conj_matches_the_supplied_disjoint_commuting_example()
    {
        AssertSuccess(
            ["(1,2)", "(3,4)"],
            "perm        (1,2)\n" +
            "by          (3,4)\n" +
            "conj        (1,2)\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_of_the_identity_and_a_transposition_is_the_identity()
    {
        AssertSuccess(
            ["()", "(1,2)"],
            "perm        ()\n" +
            "by          (1,2)\n" +
            "conj        ()\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_by_the_identity_preserves_the_permutation()
    {
        AssertSuccess(
            ["(1,2,3)", "()"],
            "perm        (1,2,3)\n" +
            "by          ()\n" +
            "conj        (1,2,3)\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_canonicalizes_cycle_rotation_and_orders_disjoint_cycles()
    {
        AssertSuccess(
            ["(4,5)(3,1,2)", "()"],
            "perm        (1,2,3)(4,5)\n" +
            "by          ()\n" +
            "conj        (1,2,3)(4,5)\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_matches_the_reverse_asymmetric_pair()
    {
        AssertSuccess(
            ["(1,2)", "(1,2,3)"],
            "perm        (1,2)\n" +
            "by          (1,2,3)\n" +
            "conj        (2,3)\n" +
            "comm        (1,3,2)\n" +
            "commutes    no\n");
    }

    [Fact]
    public void Conj_accepts_point_64_with_degree_64()
    {
        AssertSuccess(
            ["--degree=64", "(1,64)", "()"],
            "perm        (1,64)\n" +
            "by          ()\n" +
            "conj        (1,64)\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_accepts_equals_and_uses_the_last_repeated_degree_option()
    {
        AssertSuccess(
            ["--degree=8", "--degree", "64", "(1,64)", "()"],
            "perm        (1,64)\n" +
            "by          ()\n" +
            "conj        (1,64)\n" +
            "comm        ()\n" +
            "commutes    yes\n");
    }

    [Fact]
    public void Conj_reports_missing_P()
    {
        AssertFailure("missing P");
    }

    [Fact]
    public void Conj_reports_missing_Q_before_parsing_P()
    {
        AssertFailure("missing Q", "(1,2");
    }

    [Fact]
    public void Conj_reports_too_many_arguments_before_parsing_operands()
    {
        AssertFailure("too many arguments", "(1,2", "()", "extra");
    }

    [Fact]
    public void Conj_validates_options_before_missing_operands()
    {
        AssertFailure(
            "invalid --degree \"0\" (expected an integer from 1 to 64)",
            "--degree",
            "0");
    }

    [Fact]
    public void Conj_reports_a_missing_option_value_before_missing_operands()
    {
        AssertFailure("--degree needs a value", "--degree");
    }

    [Fact]
    public void Conj_reports_help_as_an_unknown_option()
    {
        AssertFailure("unknown option \"--help\"", "--help");
    }

    [Fact]
    public void Conj_reports_an_unknown_option_before_deferred_degree_validation()
    {
        AssertFailure("unknown option \"--bogus\"", "--degree=0", "--bogus");
    }

    [Fact]
    public void Conj_reports_explicit_degree_overflow()
    {
        AssertFailure(
            "point 7 of \"(1,7)\" is above the degree 6",
            "--degree=6",
            "(1,7)",
            "()");
    }

    [Fact]
    public void Conj_uses_the_last_repeated_degree_when_checking_point_ranges()
    {
        AssertFailure(
            "point 7 of \"(1,7)\" is above the degree 6",
            "--degree=8",
            "--degree",
            "6",
            "(1,7)",
            "()");
    }

    [Fact]
    public void Conj_rejects_a_dash_as_an_ordinary_permutation_operand()
    {
        AssertFailure(
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())",
            "-",
            "()");
    }

    [Fact]
    public void Conj_reports_invalid_permutation_grammar()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(1,2",
            "()");
    }

    [Fact]
    public void Conj_reports_malformed_grammar_before_point_range()
    {
        AssertFailure(
            "invalid permutation \"(65,)\" (expected cycles such as (1,2,3)(4,5), or ())",
            "(65,)",
            "()");
    }

    [Fact]
    public void Conj_reports_point_range_before_repeated_point()
    {
        AssertFailure(
            "invalid point \"65\" in \"(65,65)\" (expected 1 to 64)",
            "(65,65)",
            "()");
    }

    [Fact]
    public void Conj_reports_repeated_points()
    {
        AssertFailure("point 2 is repeated in \"(1,2)(2,3)\"", "(1,2)(2,3)", "()");
    }

    [Fact]
    public void Conj_parses_every_text_before_checking_explicit_degree_overflow()
    {
        AssertFailure(
            "invalid permutation \"(2,3\" (expected cycles such as (1,2,3)(4,5), or ())",
            "--degree",
            "3",
            "(1,5)",
            "(2,3");
    }

    [Fact]
    public void Conj_output_is_invariant_under_a_custom_current_culture()
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
                "perm        (1,2,3)\n" +
                "by          (1,2)\n" +
                "conj        (1,3,2)\n" +
                "comm        (1,2,3)\n" +
                "commutes    no\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    private static void AssertSuccess(string[] args, string expectedOutput)
    {
        var io = new FakeIo();

        var result = ConjCommand.Run(args, io);

        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
        Assert.Equal(0, io.ReadCount);
    }

    private static void AssertFailure(string expectedMessage, params string[] args)
    {
        var io = new FakeIo();

        var exception = Assert.Throws<CayleyException>(() => ConjCommand.Run(args, io));

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
