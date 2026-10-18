using System.Text;

namespace Cayleykit;

public static class Program
{
    public static int Main(string[] args)
    {
        var io = new ConsoleIo();
        return Cli.Run(args, io);
    }

    private sealed class ConsoleIo : IIo
    {
        private static readonly UTF8Encoding Utf8 = new(false);

        public byte[] ReadStdin()
        {
            using var input = Console.OpenStandardInput();
            using var buffer = new MemoryStream();
            input.CopyTo(buffer);
            return buffer.ToArray();
        }

        public void Out(string text)
        {
            Write(Console.OpenStandardOutput(), text);
        }

        public void Err(string text)
        {
            Write(Console.OpenStandardError(), text);
        }

        private static void Write(Stream stream, string text)
        {
            stream.Write(Utf8.GetBytes(text));
            stream.Flush();
        }
    }
}
