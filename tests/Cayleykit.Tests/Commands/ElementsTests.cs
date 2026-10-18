using System.Diagnostics;
using System.Text;

namespace Cayleykit.Tests;

public class ElementsTests
{
    [Fact]
    public void Run_matches_the_complete_supplied_s3_elements_example()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(new[] { "(1,2,3)", "(1,2)" }, io);

        AssertResult(
            result,
            "order       6\n()\n(2,3)\n(1,2)\n(1,2,3)\n(1,3,2)\n(1,3)\n",
            io);
    }

    [Fact]
    public void Run_returns_the_trivial_group_for_an_identity_generator()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(new[] { "()" }, io);

        AssertResult(result, "order       1\n()\n", io);
    }

    [Fact]
    public void Run_lists_a_cyclic_group_in_numeric_permutation_order()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(new[] { "(1,2,3)" }, io);

        AssertResult(result, "order       3\n()\n(1,2,3)\n(1,3,2)\n", io);
    }

    [Fact]
    public void Run_does_not_duplicate_elements_for_duplicate_and_redundant_generators()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(
            new[] { "(1,2,3)", "(1,2)", "(1,2)", "(2,3)" },
            io);

        AssertResult(
            result,
            "order       6\n()\n(2,3)\n(1,2)\n(1,2,3)\n(1,3,2)\n(1,3)\n",
            io);
    }

    [Fact]
    public void Run_accepts_eight_generators_including_identity_and_duplicates()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(
            new[]
            {
                "(1,3)",
                "()",
                "(1,2)",
                "(1,2)",
                "(2,3)",
                "(1,3)",
                "()",
                "(1,2,3)"
            },
            io);

        AssertResult(
            result,
            "order       6\n()\n(2,3)\n(1,2)\n(1,2,3)\n(1,3,2)\n(1,3)\n",
            io);
    }

    [Fact]
    public void Run_accepts_an_equals_degree_option_and_the_last_repeated_degree()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(
            new[] { "--degree=6", "(7,8)", "--degree", "8" },
            io);

        AssertResult(result, "order       2\n()\n(7,8)\n", io);
    }

    [Fact]
    public void Run_accepts_the_maximum_degree_of_sixty_four()
    {
        var io = new FakeIo([]);

        var result = ElementsCommand.Run(
            new[] { "--degree", "64", "(63,64)" },
            io);

        AssertResult(result, "order       2\n()\n(63,64)\n", io);
    }

    [Fact]
    public void Run_reports_missing_generators_without_reading_stdin_or_writing()
    {
        AssertFailure("missing G", Array.Empty<string>(), new FakeIo([]));
    }

    [Fact]
    public void Run_checks_unknown_options_before_missing_arguments()
    {
        AssertFailure("unknown option \"--help\"", new[] { "--help" }, new FakeIo([]));
    }

    [Fact]
    public void Run_reports_a_missing_degree_value_before_missing_arguments()
    {
        AssertFailure("--degree needs a value", new[] { "--degree" }, new FakeIo([]));
    }

    [Fact]
    public void Run_validates_degree_before_missing_arguments()
    {
        AssertFailure(
            "invalid --degree \"0\" (expected an integer from 1 to 64)",
            new[] { "--degree", "0" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_nine_generators_before_parsing_malformed_generator_text()
    {
        AssertFailure(
            "too many generators (at most 8)",
            new[]
            {
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "(1,2)",
                "bad"
            },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_nine_generators_expanded_from_stdin_before_parsing_them()
    {
        var io = new FakeIo(Encoding.ASCII.GetBytes(
            "(1,2) (1,2) (1,2) (1,2) (1,2) (1,2) (1,2) (1,2) bad"));

        AssertFailure("too many generators (at most 8)", new[] { "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_reads_generator_words_with_a_bom_crlf_spaces_and_tabs()
    {
        var io = new FakeIo(
        [
            0xEF, 0xBB, 0xBF,
            0x28, 0x31, 0x2C, 0x32, 0x2C, 0x33, 0x29,
            0x0D, 0x0A, 0x20, 0x09,
            0x28, 0x31, 0x2C, 0x32, 0x29, 0x0A
        ]);

        var result = ElementsCommand.Run(new[] { "-" }, io);

        AssertResult(
            result,
            "order       6\n()\n(2,3)\n(1,2)\n(1,2,3)\n(1,3,2)\n(1,3)\n",
            io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_the_exact_malformed_utf8_input_bytes()
    {
        var io = new FakeIo(new byte[] { 0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A });

        AssertFailure(
            "<stdin>: input is not valid UTF-8",
            new[] { "-" },
            io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_translates_an_io_exception_while_reading_stdin()
    {
        var io = new FakeIo([]) { ReadException = new IOException("unavailable") };

        AssertFailure("<stdin>: cannot read the input", new[] { "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_reports_no_input_for_whitespace_only_stdin()
    {
        var io = new FakeIo(new byte[] { 0x20, 0x09, 0x0D, 0x0A });

        AssertFailure("<stdin>: no input", new[] { "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_does_not_read_stdin_when_dash_is_mixed_with_other_generator_arguments()
    {
        var io = new FakeIo([]);

        AssertFailure(
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "-", "(1,2)" },
            io);
        Assert.Equal(0, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_malformed_permutation_notation_without_writes()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "(1,2" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_point_above_sixty_four()
    {
        AssertFailure(
            "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)",
            new[] { "(1,65)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_repeated_point()
    {
        AssertFailure(
            "point 2 is repeated in \"(1,2)(2,3)\"",
            new[] { "(1,2)(2,3)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_point_above_the_explicit_degree()
    {
        AssertFailure(
            "point 5 of \"(1,5)\" is above the degree 3",
            new[] { "--degree", "3", "(1,5)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_checks_notation_syntax_before_explicit_degree_overflow()
    {
        AssertFailure(
            "invalid permutation \"bad\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "--degree", "3", "(1,5)", "bad" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_s7_above_the_five_hundred_element_limit_without_writes()
    {
        AssertFailure(
            "group order 5040 is above 500 (too large for elements)",
            new[] { "(1,2,3,4,5,6,7)", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_accepts_the_exact_five_hundred_element_limit_and_finishes_within_ten_seconds()
    {
        var io = new FakeIo([]);
        var stopwatch = Stopwatch.StartNew();

        var result = ElementsCommand.Run(
            new[]
            {
                "(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25)",
                "(26,27,28,29,30)",
                "(31,32,33,34)"
            },
            io);

        stopwatch.Stop();

        Assert.Equal(0, result);
        Assert.Empty(io.Errors);
        var output = Assert.Single(io.Output);
        Assert.EndsWith("\n", output);
        var lines = output[..^1].Split('\n');

        Assert.Equal(501, lines.Length);
        Assert.Equal("order       500", lines[0]);
        Assert.Equal("()", lines[1]);
        Assert.Equal("(31,32,33,34)", lines[2]);
        Assert.Equal("(31,33)(32,34)", lines[3]);
        Assert.Equal("(31,34,33,32)", lines[4]);
        Assert.Equal("(26,27,28,29,30)", lines[5]);
        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25)", lines[21]);
        Assert.Equal(
            "(1,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2)" +
            "(26,30,29,28,27)(31,34,33,32)",
            lines[500]);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(10),
            $"500-element workload took {stopwatch.Elapsed}.");
    }

    private static void AssertResult(int result, string expectedOutput, FakeIo io)
    {
        Assert.Equal(0, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
    }

    private static void AssertFailure(string expectedMessage, IReadOnlyList<string> argv, FakeIo io)
    {
        var exception = Assert.Throws<CayleyException>(() => ElementsCommand.Run(argv, io));

        Assert.Equal(expectedMessage, exception.Message);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
    }

    private sealed class FakeIo : IIo
    {
        private readonly byte[] bytes;

        public FakeIo(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public int ReadCount { get; private set; }

        public IOException? ReadException { get; init; }

        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public byte[] ReadStdin()
        {
            ReadCount++;
            if (ReadException is not null)
            {
                throw ReadException;
            }

            return bytes;
        }

        public void Out(string text) => Output.Add(text);

        public void Err(string text) => Errors.Add(text);
    }
}
