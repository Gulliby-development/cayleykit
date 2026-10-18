namespace Cayleykit;

public static class Text
{
    public static string Show(string text)
    {
        var result = new System.Text.StringBuilder(text.Length + 2);
        result.Append('"');

        foreach (var character in text)
        {
            result.Append(character is >= '!' and <= '~' ? character : '?');
        }

        result.Append('"');
        return result.ToString();
    }

    public static string Line(string key, string value)
    {
        return key.PadRight(10) + "  " + value;
    }

    public static string Join(IEnumerable<int> numbers)
    {
        return string.Join(" ", numbers.Select(number => number.ToString(System.Globalization.CultureInfo.InvariantCulture)));
    }
}
