using System.Globalization;

namespace Cayleykit.Tests;

public class TextTests
{
    [Theory]
    [InlineData("", "\"\"")]
    [InlineData("!~", "\"!~\"")]
    [InlineData("!", "\"!\"")]
    [InlineData("~", "\"~\"")]
    [InlineData(" ", "\"?\"")]
    [InlineData("\u001f", "\"?\"")]
    [InlineData("\u007f", "\"?\"")]
    [InlineData(" a\t\n\u007f", "\"?a???\"")]
    [InlineData("é", "\"?\"")]
    [InlineData("\"\\", "\"\"\\\"")]
    [InlineData("\"a\\b\"", "\"\"a\\b\"\"")]
    [InlineData("\uD83D\uDE00", "\"??\"")]
    public void Show_quotes_text_and_replaces_each_nonprintable_utf16_code_unit(string text, string expected)
    {
        Assert.Equal(expected, Text.Show(text));
    }

    [Fact]

    public void Show_replaces_a_lone_high_surrogate_with_one_question_mark()
    {
        Assert.Equal("\"?\"", Text.Show("\uD800"));
    }

    [Fact]
    public void Show_replaces_a_lone_low_surrogate_with_one_question_mark()
    {
        Assert.Equal("\"?\"", Text.Show("\uDC00"));
    }

    [Fact]
    public void Line_pads_short_keys_and_adds_no_line_ending()
    {
        var line = Text.Line("perm", "()");

        Assert.Equal("perm        ()", line);
        Assert.DoesNotContain('\n', line);
        Assert.DoesNotContain('\r', line);
    }

    [Fact]
    public void Line_does_not_pad_a_ten_character_key()
    {
        Assert.Equal("transitive  yes", Text.Line("transitive", "yes"));
    }

    [Fact]
    public void Join_of_an_empty_sequence_is_empty()
    {
        Assert.Equal("", Text.Join([]));
    }

    [Fact]
    public void Join_of_one_number_has_no_extra_separator()
    {
        Assert.Equal("-7", Text.Join(new[] { -7 }));
    }

    [Fact]
    public void Join_uses_single_spaces_for_multiple_numbers_and_formats_endpoints_invariantly()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            CultureInfo.CurrentCulture = culture;

            Assert.Equal("-2147483648 0 2147483647", Text.Join([-2147483648, 0, 2147483647]));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void Call_examples_have_the_expected_formatted_results()
    {
        Assert.Equal("perm        ()", Text.Line("perm", "()"));
        Assert.Equal("transitive  yes", Text.Line("transitive", "yes"));
        Assert.Equal("", Text.Join([]));
        Assert.Equal("-2147483648 0 2147483647", Text.Join([-2147483648, 0, 2147483647]));
    }

    [Fact]
    public void Cayley_exception_preserves_its_message()
    {
        var exception = new CayleyException("input failed");

        Assert.Equal("input failed", exception.Message);
    }
}
