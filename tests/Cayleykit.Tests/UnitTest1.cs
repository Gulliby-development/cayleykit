using System.Diagnostics;
using System.Text;

namespace Cayleykit.Tests;

public class UnitTest1
{
    [Fact]
    public void Usage_and_standalone_help_are_exact()
    {
        const string expectedUsage =
            "usage: cayleykit show [--degree D] P\n" +
            "       cayleykit mul [--degree D] P Q...\n" +
            "       cayleykit pow [--degree D] P K\n" +
            "       cayleykit conj [--degree D] P Q\n" +
            "       cayleykit orbits [--degree D] G...\n" +
            "       cayleykit order [--degree D] G...\n" +
            "       cayleykit word [--degree D] P G...\n" +
            "       cayleykit elements [--degree D] G...\n" +
            "       cayleykit classes [--degree D] G...\n" +
            "       cayleykit centralizer [--degree D] P";

        Assert.Equal(expectedUsage, Cli.Usage);
        AssertCli(["--help"], 0, expectedUsage + "\n", "");
    }

    [Fact]
    public void Dispatches_show_with_the_supplied_literal_output()
    {
        AssertCli(
            ["show", "(1,2,3)(4,5)"],
            0,
            "perm        (1,2,3)(4,5)\ndegree      5\nimage       2 3 1 5 4\norder       6\nsign        odd\ntype        3 2\ninverse     (1,3,2)(4,5)\n",
            "");
    }

    [Fact]
    public void Dispatches_mul_with_the_supplied_literal_output()
    {
        AssertCli(
            ["mul", "(1,2,3)", "(1,2)"],
            0,
            "product     (2,3)\ndegree      3\norder       2\nsign        odd\n",
            "");
    }

    [Fact]
    public void Dispatches_pow_with_the_supplied_literal_output()
    {
        AssertCli(
            ["pow", "()", "5"],
            0,
            "perm        ()\norder       1\npower       ()\nreduced     0\n",
            "");
    }

    [Fact]
    public void Dispatches_conj_with_the_supplied_literal_output()
    {
        AssertCli(
            ["conj", "(1,2)", "(3,4)"],
            0,
            "perm        (1,2)\nby          (3,4)\nconj        (1,2)\ncomm        ()\ncommutes    yes\n",
            "");
    }

    [Fact]
    public void Dispatches_orbits_with_the_supplied_literal_output()
    {
        AssertCli(
            ["orbits", "(1,2,3)", "(4,5)"],
            0,
            "degree      5\norder       6\norbits      2\norbit       {1,2,3}  size 3  stab 2\norbit       {4,5}  size 2  stab 3\ntransitive  no\n",
            "");
    }

    [Fact]
    public void Dispatches_order_with_the_supplied_literal_output()
    {
        AssertCli(
            ["order", "()"],
            0,
            "degree      1\ngens        1\norder       1\nbase        none\nsizes       none\n",
            "");
    }

    [Fact]
    public void Dispatches_word_with_the_supplied_literal_output()
    {
        AssertCli(
            ["word", "()", "(1,2)"],
            0,
            "order       2\nlength      0\nword        (empty)\n",
            "");
    }

    [Fact]
    public void Dispatches_elements_with_the_supplied_literal_output()
    {
        AssertCli(
            ["elements", "(1,2,3)", "(1,2)"],
            0,
            "order       6\n()\n(2,3)\n(1,2)\n(1,2,3)\n(1,3,2)\n(1,3)\n",
            "");
    }

    [Fact]
    public void Dispatches_classes_with_the_supplied_literal_output()
    {
        AssertCli(
            ["classes", "(1,2,3,4)", "(1,2)"],
            0,
            "degree      4\norder       24\nclasses     5\ncenter      1\nabelian     no\nclass       size 1  order 1  type 1 1 1 1  rep ()\nclass       size 6  order 2  type 2 1 1  rep (3,4)\nclass       size 8  order 3  type 3 1  rep (2,3,4)\nclass       size 3  order 2  type 2 2  rep (1,2)(3,4)\nclass       size 6  order 4  type 4  rep (1,2,3,4)\n",
            "");
    }

    [Fact]
    public void Word_nonmembership_preserves_exit_one_and_exact_output()
    {
        AssertCli(
            ["word", "(1,2)", "(1,2,3)"],
            1,
            "order       3\nstatus      not in the group\n",
            "");
    }

    [Fact]
    public void Missing_command_uses_the_exact_error_envelope()
    {
        AssertCli(
            [],
            2,
            "",
            "cayleykit: missing command (show, mul, pow, conj, orbits, order, word, elements, classes or centralizer)\n");
    }

    [Fact]
    public void Unknown_command_uses_the_exact_error_envelope()
    {
        AssertCli(["frob"], 2, "", "cayleykit: unknown command \"frob\"\n");
    }

    [Fact]
    public void Help_followed_by_an_argument_is_an_unknown_command()
    {
        AssertCli(["--help", "show"], 2, "", "cayleykit: unknown command \"--help\"\n");
    }

    [Fact]
    public void Help_after_a_command_is_an_unknown_option()
    {
        AssertCli(["show", "--help"], 2, "", "cayleykit: unknown option \"--help\"\n");
    }

    [Fact]
    public void Missing_show_argument_uses_the_exact_error_envelope()
    {
        AssertCli(["show"], 2, "", "cayleykit: missing P\n");
    }

    [Fact]
    public void Missing_mul_operand_uses_the_exact_error_envelope()
    {
        AssertCli(["mul", "(1,2)"], 2, "", "cayleykit: missing Q\n");
    }

    [Fact]
    public void Invalid_option_value_precedes_a_malformed_permutation()
    {
        AssertCli(
            ["show", "--degree", "0", "(1,2"],
            2,
            "",
            "cayleykit: invalid --degree \"0\" (expected an integer from 1 to 64)\n");
    }

    [Fact]
    public void Malformed_permutation_uses_the_exact_error_envelope()
    {
        AssertCli(
            ["show", "(1,2"],
            2,
            "",
            "cayleykit: invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())\n");
    }

    [Fact]
    public void Unknown_command_text_sanitizes_a_literal_tab()
    {
        AssertCli(["frob\tx"], 2, "", "cayleykit: unknown command \"frob?x\"\n");
    }

    [Fact]
    public async Task Process_rejects_malformed_utf8_stdin_bytes_with_exact_stream_bytes()
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = AppContext.BaseDirectory,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.StartInfo.ArgumentList.Add("cayleykit.dll");
        process.StartInfo.ArgumentList.Add("order");
        process.StartInfo.ArgumentList.Add("-");

        Assert.True(process.Start(), "The cayleykit process should start.");

        var stdoutTask = ReadAllBytesAsync(process.StandardOutput.BaseStream);
        var stderrTask = ReadAllBytesAsync(process.StandardError.BaseStream);
        var stdinBytes = new byte[] { 0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A };
        await process.StandardInput.BaseStream.WriteAsync(stdinBytes);
        process.StandardInput.Close();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await process.WaitForExitAsync(timeout.Token);
        await Task.WhenAll(stdoutTask, stderrTask);

        Assert.Equal(2, process.ExitCode);
        Assert.Empty(await stdoutTask);
        Assert.Equal(
            Encoding.UTF8.GetBytes("cayleykit: <stdin>: input is not valid UTF-8\n"),
            await stderrTask);
    }

    private static void AssertCli(string[] args, int expectedExitCode, string expectedStdout, string expectedStderr)
    {
        var io = new FakeIo();

        var exitCode = Cli.Run(args, io);

        Assert.Equal(expectedExitCode, exitCode);
        Assert.Equal(0, io.ReadCount);
        if (expectedStdout.Length == 0)
        {
            Assert.Empty(io.Output);
        }
        else
        {
            Assert.Equal(new[] { expectedStdout }, io.Output);
        }

        if (expectedStderr.Length == 0)
        {
            Assert.Empty(io.Errors);
        }
        else
        {
            Assert.Equal(new[] { expectedStderr }, io.Errors);
        }
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
    {
        using var destination = new MemoryStream();
        await stream.CopyToAsync(destination);
        return destination.ToArray();
    }

    private sealed class FakeIo : IIo
    {
        public int ReadCount { get; private set; }

        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public byte[] ReadStdin()
        {
            ReadCount++;
            return [];
        }

        public void Out(string text) => Output.Add(text);

        public void Err(string text) => Errors.Add(text);
    }
}
