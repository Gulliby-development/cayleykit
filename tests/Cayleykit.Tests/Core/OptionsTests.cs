namespace Cayleykit.Tests;

public class OptionsTests
{
    [Fact]
    public void Parse_accepts_an_inline_option_assignment()
    {
        var parsed = Options.Parse(
            ["--degree=8"],
            new[] { new Spec("--degree", 1, 64, 0) });

        Assert.Equal(8, parsed.Values["--degree"]);
        Assert.Empty(parsed.Args);
        Assert.Single(parsed.Values);
    }

    [Fact]
    public void Parse_consumes_a_separate_value_and_preserves_positional_arguments()
    {
        var parsed = Options.Parse(
            ["--degree", "8", "-", "-7", "P"],
            new[] { new Spec("--degree", 1, 64, 0) });

        Assert.Equal(8, parsed.Values["--degree"]);
        Assert.Equal(new[] { "-", "-7", "P" }, parsed.Args);
    }

    [Fact]
    public void Parse_uses_an_unvalidated_fallback_when_an_option_is_absent()
    {
        var parsed = Options.Parse(
            new[] { "P", "-", "-7" },
            new[] { new Spec("--degree", 1, 3, 0) });

        Assert.Equal(0, parsed.Values["--degree"]);
        Assert.Equal(new[] { "P", "-", "-7" }, parsed.Args);
        Assert.Single(parsed.Values);
    }

    [Fact]
    public void Parse_keeps_the_last_repeated_value_and_validates_only_that_value()
    {
        var parsed = Options.Parse(
            ["--degree=bad", "--degree", "9"],
            new[] { new Spec("--degree", 1, 64, 0) });

        Assert.Equal(9, parsed.Values["--degree"]);
        Assert.Empty(parsed.Args);
    }

    [Theory]
    [InlineData("--", "unknown option \"--\"")]
    [InlineData("--help", "unknown option \"--help\"")]
    [InlineData("--other=7", "unknown option \"--other=7\"")]
    [InlineData("--help=3", "unknown option \"--help=3\"")]
    [InlineData("--bad\n", "unknown option \"--bad?\"")]
    public void Parse_reports_unknown_options_using_the_whole_sanitized_argument(
        string argument,
        string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { argument },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Parse_reports_a_known_terminal_option_without_a_value()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--degree" },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal("--degree needs a value", exception.Message);
    }

    [Theory]
    [InlineData("--degree=", "invalid --degree \"\" (expected an integer from 1 to 64)")]
    [InlineData("--degree=8=9", "invalid --degree \"8=9\" (expected an integer from 1 to 64)")]
    public void Parse_validates_inline_values_exactly(string option, string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { option },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("--help", "invalid --degree \"--help\" (expected an integer from 1 to 64)")]
    [InlineData("-7", "invalid --degree \"-7\" (expected an integer from 1 to 64)")]
    public void Parse_consumes_a_dash_leading_separate_value(
        string value,
        string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--degree", value },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Parse_validates_the_last_repeated_value_when_it_is_invalid()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--degree=8", "--degree=bad" },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal(
            "invalid --degree \"bad\" (expected an integer from 1 to 64)",
            exception.Message);
    }

    [Fact]
    public void Parse_reports_structural_scan_errors_before_earlier_invalid_values()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--degree=bad", "--help" },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal("unknown option \"--help\"", exception.Message);
    }

    [Fact]
    public void Parse_reports_a_later_missing_value_before_validating_an_earlier_value()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--degree=bad", "--degree" },
                new[] { new Spec("--degree", 1, 64, 0) }));

        Assert.Equal("--degree needs a value", exception.Message);
    }

    [Fact]
    public void Parse_validates_supplied_options_in_spec_order_not_argument_order()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Options.Parse(
                new[] { "--b=bad", "--a=also" },
                new[]
                {
                    new Spec("--a", 1, 10, 0),
                    new Spec("--b", 1, 20, 0)
                }));

        Assert.Equal(
            "invalid --a \"also\" (expected an integer from 1 to 10)",
            exception.Message);
    }

    [Fact]
    public void Parse_returns_one_value_for_each_spec_and_preserves_argument_order()
    {
        var parsed = Options.Parse(
            new[] { "first", "--b=7", "-", "last" },
            new[]
            {
                new Spec("--a", 1, 10, 0),
                new Spec("--b", 1, 20, 0),
                new Spec("--c", 1, 30, -1)
            });

        Assert.Equal(3, parsed.Values.Count);
        Assert.Equal(0, parsed.Values["--a"]);
        Assert.Equal(7, parsed.Values["--b"]);
        Assert.Equal(-1, parsed.Values["--c"]);
        Assert.Equal(new[] { "first", "-", "last" }, parsed.Args);
    }
}
