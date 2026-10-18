namespace Cayleykit;

public static class Ints
{
    public static long Parse(string name, string text, long lo, long hi)
    {
        if (!IsCanonicalInteger(text) ||
            !long.TryParse(
                text,
                System.Globalization.NumberStyles.AllowLeadingSign,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value) ||
            value < lo ||
            value > hi)
        {
            throw new CayleyException(
                $"invalid {name} {Text.Show(text)} (expected an integer from {lo.ToString(System.Globalization.CultureInfo.InvariantCulture)} to {hi.ToString(System.Globalization.CultureInfo.InvariantCulture)})");
        }

        return value;
    }

    private static bool IsCanonicalInteger(string text)
    {
        if (text.Length == 0)
        {
            return false;
        }

        var firstDigit = text[0] == '-' ? 1 : 0;
        var digitCount = text.Length - firstDigit;

        if (digitCount == 0 || digitCount > 19)
        {
            return false;
        }

        if (text[firstDigit] == '0')
        {
            return digitCount == 1 && firstDigit == 0;
        }

        if (text[firstDigit] is < '1' or > '9')
        {
            return false;
        }

        for (var index = firstDigit + 1; index < text.Length; index++)
        {
            if (text[index] is < '0' or > '9')
            {
                return false;
            }
        }

        return true;
    }
}
