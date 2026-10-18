using System.Text;

namespace Cayleykit;

public interface IIo
{
    byte[] ReadStdin();

    void Out(string text);

    void Err(string text);
}

public static class Words
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static List<string> Of(IIo io, IReadOnlyList<string> args)
    {
        if (args.Count != 1 || args[0] != "-")
        {
            return new List<string>(args);
        }

        byte[] bytes;
        try
        {
            bytes = io.ReadStdin();
        }
        catch (IOException)
        {
            throw new CayleyException("<stdin>: cannot read the input");
        }

        var text = DecodeUtf8(bytes);
        if (text.Length > 0 && text[0] == '\uFEFF')
        {
            text = text[1..];
        }

        var words = text.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            throw new CayleyException("<stdin>: no input");
        }

        return new List<string>(words);
    }

    private static string DecodeUtf8(byte[] bytes)
    {
        try
        {
            return StrictUtf8.GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            throw new CayleyException("<stdin>: input is not valid UTF-8");
        }
    }
}
