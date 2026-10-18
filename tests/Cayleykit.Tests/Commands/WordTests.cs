using System.Diagnostics;
using System.Text;

namespace Cayleykit.Tests;

public class WordTests
{
    [Fact]
    public void Run_matches_the_supplied_shortest_dictionary_first_s3_example()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,3)", "(1,2)", "(2,3)" }, io);

        AssertResult(
            result,
            0,
            "order       6\nlength      3\nword        g1 g2 g1\n",
            io);
    }

    [Fact]
    public void Run_matches_the_supplied_s4_word_example()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,2,3)", "(1,2)", "(1,2,3,4)" }, io);

        AssertResult(
            result,
            0,
            "order       24\nlength      6\nword        g1 g2 g1 g2 g2 g1\n",
            io);
    }

    [Fact]
    public void Run_matches_the_supplied_identity_word_example()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "()", "(1,2)" }, io);

        AssertResult(result, 0, "order       2\nlength      0\nword        (empty)\n", io);
    }

    [Fact]
    public void Run_matches_the_supplied_nonmembership_example()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,2)", "(1,2,3)" }, io);

        AssertResult(result, 1, "order       3\nstatus      not in the group\n", io);
    }

    [Fact]
    public void Run_uses_a_positive_word_for_an_inverse_generator()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,3,2)", "(1,2,3)" }, io);

        AssertResult(result, 0, "order       3\nlength      2\nword        g1 g1\n", io);
    }

    [Fact]
    public void Run_preserves_the_generator_index_after_an_identity_generator()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,2)", "()", "(1,2)" }, io);

        AssertResult(result, 0, "order       2\nlength      1\nword        g2\n", io);
    }

    [Fact]
    public void Run_accepts_eight_duplicate_and_identity_generators_and_keeps_original_indices()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(
            new[]
            {
                "(1,3)",
                "()",
                "(1,2)",
                "(1,2)",
                "(2,3)",
                "(1,3)",
                "()",
                "(1,2,3)",
                "(1,3,2)"
            },
            io);

        AssertResult(result, 0, "order       6\nlength      1\nword        g5\n", io);
    }

    [Fact]
    public void Run_uses_a_trivial_group_order_and_identity_word_for_an_identity_generator()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "()", "()" }, io);

        AssertResult(result, 0, "order       1\nlength      0\nword        (empty)\n", io);
    }

    [Fact]
    public void Run_includes_the_target_when_inferring_the_degree()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(new[] { "(1,4)", "()" }, io);

        AssertResult(result, 1, "order       1\nstatus      not in the group\n", io);
    }

    [Fact]
    public void Run_accepts_an_equals_degree_option_and_uses_the_last_repeated_value()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(
            new[] { "(7,8)", "--degree=6", "(7,8)", "--degree", "8" },
            io);

        AssertResult(result, 0, "order       2\nlength      1\nword        g1\n", io);
    }

    [Fact]
    public void Run_accepts_the_maximum_degree_of_sixty_four()
    {
        var io = new FakeIo([]);

        var result = WordCommand.Run(
            new[] { "--degree", "64", "(63,64)", "(63,64)" },
            io);

        AssertResult(result, 0, "order       2\nlength      1\nword        g1\n", io);
    }

    [Fact]
    public void Run_reports_missing_target_without_reading_stdin_or_writing()
    {
        AssertFailure("missing P", Array.Empty<string>(), new FakeIo([]));
    }

    [Fact]
    public void Run_reports_missing_generators_without_reading_stdin_or_writing()
    {
        AssertFailure("missing G", new[] { "()" }, new FakeIo([]));
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
                "()",
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
    public void Run_reads_only_generators_before_parsing_the_target()
    {
        var io = new FakeIo([]);

        AssertFailure("<stdin>: no input", new[] { "bad", "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_malformed_target_notation_without_writes()
    {
        AssertFailure(
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "(1,2", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_point_above_sixty_four()
    {
        AssertFailure(
            "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)",
            new[] { "(1,65)", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_repeated_point_in_the_target()
    {
        AssertFailure(
            "point 2 is repeated in \"(1,2)(2,3)\"",
            new[] { "(1,2)(2,3)", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_target_point_above_the_explicit_degree()
    {
        AssertFailure(
            "point 5 of \"(1,5)\" is above the degree 3",
            new[] { "--degree", "3", "(1,5)", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_checks_all_syntax_before_any_explicit_degree_overflow()
    {
        AssertFailure(
            "invalid permutation \"bad\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "--degree", "3", "(1,5)", "bad" },
            new FakeIo([]));
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

        var result = WordCommand.Run(new[] { "(1,3)", "-" }, io);

        AssertResult(result, 0, "order       6\nlength      2\nword        g2 g1\n", io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_the_exact_malformed_utf8_input_bytes()
    {
        var io = new FakeIo(new byte[] { 0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A });

        AssertFailure(
            "<stdin>: input is not valid UTF-8",
            new[] { "()", "-" },
            io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_translates_an_io_exception_while_reading_stdin()
    {
        var io = new FakeIo([]) { ReadException = new IOException("unavailable") };

        AssertFailure("<stdin>: cannot read the input", new[] { "()", "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_reports_no_input_for_whitespace_only_stdin()
    {
        var io = new FakeIo(new byte[] { 0x20, 0x09, 0x0D, 0x0A });

        AssertFailure("<stdin>: no input", new[] { "()", "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_does_not_read_stdin_when_dash_is_mixed_with_other_generator_arguments()
    {
        var io = new FakeIo([]);

        AssertFailure(
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "()", "-", "(1,2)" },
            io);
        Assert.Equal(0, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_nine_generators_expanded_from_stdin_before_parsing_them()
    {
        var io = new FakeIo(Encoding.ASCII.GetBytes(
            "(1,2) (1,2) (1,2) (1,2) (1,2) (1,2) (1,2) (1,2) bad"));

        AssertFailure("too many generators (at most 8)", new[] { "()", "-" }, io);
        Assert.Equal(1, io.ReadCount);
    }

    [Theory]
    [InlineData("(1,2)")]
    [InlineData("()")]
    [InlineData("(9,10)")]
    public void Run_rejects_every_target_once_the_exact_group_order_is_above_the_limit(string target)
    {
        AssertFailure(
            "group order 362880 is above 100000 (too large for word)",
            new[]
            {
                "--degree",
                "10",
                target,
                "(1,2,3,4,5,6,7,8,9)",
                "(1,2)"
            },
            new FakeIo([]));
    }

    [Fact]
    public void Run_finishes_a_handwritten_s8_nonmembership_workload_within_ten_seconds()
    {
        var io = new FakeIo([]);
        var stopwatch = Stopwatch.StartNew();

        var result = WordCommand.Run(
            new[] { "(8,9)", "(1,2,3,4,5,6,7,8)", "(1,2)" },
            io);

        stopwatch.Stop();

        AssertResult(result, 1, "order       40320\nstatus      not in the group\n", io);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"S8 word workload took {stopwatch.Elapsed}.");
    }

    private static void AssertResult(int result, int expectedResult, string expectedOutput, FakeIo io)
    {
        Assert.Equal(expectedResult, result);
        Assert.Equal(new[] { expectedOutput }, io.Output);
        Assert.Empty(io.Errors);
    }

    private static void AssertFailure(string expectedMessage, IReadOnlyList<string> argv, FakeIo io)
    {
        var exception = Assert.Throws<CayleyException>(() => WordCommand.Run(argv, io));

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
