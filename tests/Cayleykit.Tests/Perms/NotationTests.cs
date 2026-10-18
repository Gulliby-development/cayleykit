namespace Cayleykit.Tests;

public class NotationTests
{
    [Fact]
    public void ParseCycles_accepts_the_identity_as_no_written_cycles()
    {
        Assert.Empty(Notation.ParseCycles("()"));
    }

    [Fact]
    public void ParseCycles_preserves_written_cycle_and_point_order()
    {
        var cycles = Notation.ParseCycles("(3,1,2)(5,4)");

        Assert.Equal(2, cycles.Count);
        Assert.Equal(new[] { 3, 1, 2 }, cycles[0]);
        Assert.Equal(new[] { 5, 4 }, cycles[1]);
    }

    [Fact]
    public void ParseCycles_accepts_the_largest_supported_point()
    {
        var cycles = Notation.ParseCycles("(1,64)");

        Assert.Single(cycles);
        Assert.Equal(new[] { 1, 64 }, cycles[0]);
    }

    [Theory]
    [InlineData("", "invalid permutation \"\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1)", "invalid permutation \"(1)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("()(1,2)", "invalid permutation \"()(1,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(01,2)", "invalid permutation \"(01,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1, 2)", "invalid permutation \"(1,?2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(+1,2)", "invalid permutation \"(+1,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(١,2)", "invalid permutation \"(?,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,2", "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,2)x", "invalid permutation \"(1,2)x\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("x(1,2)", "invalid permutation \"x(1,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,,2)", "invalid permutation \"(1,,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,2,)", "invalid permutation \"(1,2,)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(,1,2)", "invalid permutation \"(,1,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("((1,2))", "invalid permutation \"((1,2))\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,2) (3,4)", "invalid permutation \"(1,2)?(3,4)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(١,3)", "invalid permutation \"(?,3)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    public void ParseCycles_rejects_invalid_grammar_with_exact_sanitized_diagnostics(
        string text,
        string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(() => Notation.ParseCycles(text));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ParseCycles_sanitizes_each_unpaired_surrogate_code_unit()
    {
        var text = new string(new[] { '(', '1', ',', '\uD800', ')' });
        var exception = Assert.Throws<CayleyException>(() => Notation.ParseCycles(text));

        Assert.Equal(
            "invalid permutation \"(1,?)\" (expected cycles such as (1,2,3)(4,5), or ())",
            exception.Message);
    }

    [Theory]
    [InlineData("(0,65)", "invalid point \"0\" in \"(0,65)\" (expected 1 to 64)")]
    [InlineData("(65,0)", "invalid point \"65\" in \"(65,0)\" (expected 1 to 64)")]
    [InlineData("(1,65)", "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)")]
    [InlineData("(1,999999999999999999999999999999)", "invalid point \"999999999999999999999999999999\" in \"(1,999999999999999999999999999999)\" (expected 1 to 64)")]
    [InlineData("(1,1)(2,65)", "invalid point \"65\" in \"(1,1)(2,65)\" (expected 1 to 64)")]
    [InlineData("(1,2)(2,3)", "point 2 is repeated in \"(1,2)(2,3)\"")]
    [InlineData("(2,2)(1,1)", "point 2 is repeated in \"(2,2)(1,1)\"")]
    [InlineData("(3,1,3)(2,2)", "point 3 is repeated in \"(3,1,3)(2,2)\"")]
    public void ParseCycles_validates_ranges_before_repeats_and_reports_first_offender(
        string text,
        string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(() => Notation.ParseCycles(text));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void ParseCycles_checks_whole_text_grammar_before_point_ranges()
    {
        var exception = Assert.Throws<CayleyException>(() => Notation.ParseCycles("(0,65)x"));

        Assert.Equal(
            "invalid permutation \"(0,65)x\" (expected cycles such as (1,2,3)(4,5), or ())",
            exception.Message);
    }

    [Fact]
    public void Make_maps_each_written_cycle_and_leaves_unwritten_points_fixed()
    {
        var permutation = Notation.Make(new IReadOnlyList<int>[] { new[] { 3, 1, 2 } }, 4);

        Assert.Equal(new[] { 2, 3, 1, 4 }, permutation.Images);
    }

    [Fact]
    public void ParseAll_infers_degree_one_for_multiple_identity_texts()
    {
        var result = Notation.ParseAll(new[] { "()", "()" }, 0);

        Assert.Equal(1, result.Degree);
        Assert.Equal(2, result.Perms.Count);
        Assert.Equal(new[] { 1 }, result.Perms[0].Images);
        Assert.Equal(new[] { 1 }, result.Perms[1].Images);
    }

    [Fact]
    public void ParseAll_infers_the_largest_point_and_keeps_unwritten_points_fixed()
    {
        var result = Notation.ParseAll(new[] { "(1,2)", "(3,5)" }, 0);

        Assert.Equal(5, result.Degree);
        Assert.Equal(new[] { 2, 1, 3, 4, 5 }, result.Perms[0].Images);
        Assert.Equal(new[] { 1, 2, 5, 4, 3 }, result.Perms[1].Images);
    }

    [Fact]
    public void ParseAll_uses_a_positive_explicit_degree_and_pads_images()
    {
        var result = Notation.ParseAll(new[] { "(1,2)" }, 6);

        Assert.Equal(6, result.Degree);
        Assert.Equal(new[] { 2, 1, 3, 4, 5, 6 }, result.Perms[0].Images);
    }

    [Fact]
    public void ParseAll_parses_every_text_before_checking_an_above_degree_point()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Notation.ParseAll(new[] { "(1,5)", "(1,2" }, 3));

        Assert.Equal(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            exception.Message);
    }

    [Fact]
    public void ParseAll_parses_all_point_ranges_before_checking_above_degree()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Notation.ParseAll(new[] { "(1,5)", "(1,65)" }, 3));

        Assert.Equal(
            "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)",
            exception.Message);
    }

    [Fact]
    public void ParseAll_reports_an_earlier_repeat_before_a_later_syntax_error()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Notation.ParseAll(new[] { "(1,2)(2,3)", "(1,2" }, 3));

        Assert.Equal("point 2 is repeated in \"(1,2)(2,3)\"", exception.Message);
    }

    [Fact]
    public void ParseAll_checks_above_degree_in_text_then_point_order()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Notation.ParseAll(new[] { "(5,4)", "(1,6)" }, 3));

        Assert.Equal("point 5 of \"(5,4)\" is above the degree 3", exception.Message);
    }

    [Fact]
    public void ParseAll_infers_degree_sixty_four()
    {
        var result = Notation.ParseAll(new[] { "(1,64)" }, 0);

        Assert.Equal(64, result.Degree);
        Assert.Equal(64, result.Perms[0].Degree);
        Assert.Equal(64, result.Perms[0].Image(1));
        Assert.Equal(1, result.Perms[0].Image(64));
        Assert.Equal(2, result.Perms[0].Image(2));
    }

    [Theory]
    [InlineData("(3,1,2)(5,4)", 6, "(1,2,3)(4,5)")]
    [InlineData("()", 1, "()")]
    [InlineData("(1,2)(3,4)", 4, "(1,2)(3,4)")]
    public void Format_returns_canonical_cycle_text_without_a_newline(string text, long degree, string expected)
    {
        var permutation = Notation.ParseAll(new[] { text }, degree).Perms[0];

        var formatted = Notation.Format(permutation);

        Assert.Equal(expected, formatted);
        Assert.False(formatted.EndsWith("\n", StringComparison.Ordinal));
    }
}
