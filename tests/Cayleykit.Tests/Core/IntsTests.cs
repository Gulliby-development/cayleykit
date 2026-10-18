using System.Globalization;

namespace Cayleykit.Tests;

public class IntsTests
{
    [Theory]
    [InlineData("0", long.MinValue, long.MaxValue, 0)]
    [InlineData("3", long.MinValue, long.MaxValue, 3)]
    [InlineData("-3", long.MinValue, long.MaxValue, -3)]
    [InlineData("9223372036854775807", long.MinValue, long.MaxValue, long.MaxValue)]
    [InlineData("-9223372036854775808", long.MinValue, long.MaxValue, long.MinValue)]
    [InlineData("1", 1, 1, 1)]
    [InlineData("-1", -1, -1, -1)]
    [InlineData("42", 0, 42, 42)]
    [InlineData("-42", -42, 0, -42)]
    [InlineData("1", 0, 1, 1)]
    [InlineData("-1", -1, 0, -1)]
    [InlineData("1000000000000000000", long.MinValue, long.MaxValue, 1000000000000000000)]
    public void Parse_accepts_canonical_ascii_integers_within_inclusive_bounds(
        string text,
        long lo,
        long hi,
        long expected)
    {
        Assert.Equal(expected, Ints.Parse("N", text, lo, hi));
    }

    [Theory]
    [InlineData("+3", -10, 10, "invalid N \"+3\" (expected an integer from -10 to 10)")]
    [InlineData("007", -10, 10, "invalid N \"007\" (expected an integer from -10 to 10)")]
    [InlineData("-0", -10, 10, "invalid N \"-0\" (expected an integer from -10 to 10)")]
    [InlineData("1.5", -10, 10, "invalid N \"1.5\" (expected an integer from -10 to 10)")]
    [InlineData("", -10, 10, "invalid N \"\" (expected an integer from -10 to 10)")]
    [InlineData(" ", -10, 10, "invalid N \"?\" (expected an integer from -10 to 10)")]
    [InlineData("1\t", -10, 10, "invalid N \"1?\" (expected an integer from -10 to 10)")]
    [InlineData("١", -10, 10, "invalid N \"?\" (expected an integer from -10 to 10)")]
    [InlineData("12345678901234567890", -10, 10, "invalid N \"12345678901234567890\" (expected an integer from -10 to 10)")]
    [InlineData("9223372036854775808", -10, 10, "invalid N \"9223372036854775808\" (expected an integer from -10 to 10)")]
    [InlineData("-9223372036854775809", -10, 10, "invalid N \"-9223372036854775809\" (expected an integer from -10 to 10)")]
    [InlineData("10", 0, 9, "invalid N \"10\" (expected an integer from 0 to 9)")]
    [InlineData("-10", -9, 0, "invalid N \"-10\" (expected an integer from -9 to 0)")]
    public void Parse_rejects_noncanonical_overflowing_or_out_of_range_values_with_exact_diagnostics(
        string text,
        long lo,
        long hi,
        string expectedMessage)
    {
        var exception = Assert.Throws<CayleyException>(() => Ints.Parse("N", text, lo, hi));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void Parse_reports_the_specified_K_diagnostic_exactly()
    {
        var exception = Assert.Throws<CayleyException>(
            () => Ints.Parse("K", "1.5", -1000000000000000000, 1000000000000000000));

        Assert.Equal(
            "invalid K \"1.5\" (expected an integer from -1000000000000000000 to 1000000000000000000)",
            exception.Message);
    }

    [Fact]
    public void Parse_formats_bounds_invariantly_under_a_custom_negative_sign()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            CultureInfo.CurrentCulture = culture;

            var exception = Assert.Throws<CayleyException>(() => Ints.Parse("K", "1.5", -5, 7));

            Assert.Equal("invalid K \"1.5\" (expected an integer from -5 to 7)", exception.Message);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
