using System.Diagnostics;
using System.Globalization;

namespace Cayleykit.Tests;

public class OrderTests
{
    [Fact]
    public void Run_infers_degree_and_reports_the_exact_S5_chain()
    {
        AssertSuccess(
            ["(1,2,3,4,5)", "(1,2)"],
            "degree      5\ngens        2\norder       120\nbase        1 2 3 4\nsizes       5 4 3 2\n");
    }

    [Fact]
    public void Run_reports_the_trivial_group_for_an_identity_generator()
    {
        AssertSuccess(
            ["()"],
            "degree      1\ngens        1\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_preserves_the_supplied_generator_count_for_identity_generators()
    {
        AssertSuccess(
            ["()", "()"],
            "degree      1\ngens        2\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_preserves_repeated_generators_in_the_generator_count()
    {
        AssertSuccess(
            ["(1,2)", "(1,2)", "()"],
            "degree      2\ngens        3\norder       2\nbase        1\nsizes       2\n");
    }

    [Fact]
    public void Run_accepts_eight_generators_at_the_count_boundary()
    {
        AssertSuccess(
            ["()", "()", "()", "()", "()", "()", "()", "()"],
            "degree      1\ngens        8\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_accepts_an_inline_degree_option()
    {
        AssertSuccess(
            ["--degree=8", "()"],
            "degree      8\ngens        1\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_uses_the_last_repeated_degree_option_and_allows_interspersing()
    {
        AssertSuccess(
            ["--degree", "3", "()", "--degree=5"],
            "degree      5\ngens        1\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_infers_the_largest_written_point()
    {
        AssertSuccess(
            ["(1,2)", "(4,5)"],
            "degree      5\ngens        2\norder       4\nbase        1 4\nsizes       2 2\n");
    }

    [Fact]
    public void Run_accepts_explicit_degree_one()
    {
        AssertSuccess(
            ["--degree", "1", "()"],
            "degree      1\ngens        1\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_accepts_point_sixty_four_at_the_degree_boundary()
    {
        AssertSuccess(
            ["--degree", "64", "(1,64)"],
            "degree      64\ngens        1\norder       2\nbase        1\nsizes       2\n");
    }

    // Fixture: pocket cube, copied from the request's literal generators and output.
    [Fact]
    public void Run_reports_the_pocket_cube_order_base_and_sizes()
    {
        AssertSuccess(
            [
                "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
                "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
                "(1,7,16,18)(2,5,15,20)(9,11,12,10)"
            ],
            "degree      24\ngens        3\norder       3674160\nbase        1 2 3 4 5 6\nsizes       21 18 15 12 9 6\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    // Fixture: 3x3x3 cube, copied from the request's literal generators and output.
    [Fact]
    public void Run_reports_the_exact_three_by_three_cube_order_base_and_sizes()
    {
        AssertSuccess(
            [
                "(1,7,9,3)(2,4,8,6)(16,25,43,52)(17,26,44,53)(18,27,45,54)",
                "(3,52,30,21)(6,49,33,24)(9,46,36,27)(10,16,18,12)(11,13,17,15)",
                "(1,16,36,39)(2,13,35,42)(3,10,34,45)(19,25,27,21)(20,22,26,24)",
                "(10,46,37,19)(11,47,38,20)(12,48,39,21)(28,34,36,30)(29,31,35,33)",
                "(1,19,28,54)(4,22,31,51)(7,25,34,48)(37,43,45,39)(38,40,44,42)",
                "(7,37,30,18)(8,40,29,15)(9,43,28,12)(46,52,54,48)(47,49,53,51)"
            ],
            "degree      54\ngens        6\norder       43252003274489856000\nbase        1 2 3 4 6 7 8 9 10 11 12 13 15 19 20 22 29 31\nsizes       24 24 21 22 20 18 18 15 12 16 9 14 12 6 10 8 6 2\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Run_reads_BOM_CRLF_tab_and_space_separated_generators_from_stdin()
    {
        AssertSuccess(
            ["-"],
            "degree      5\ngens        2\norder       120\nbase        1 2 3 4\nsizes       5 4 3 2\n",
            1,
            [
                0xEF, 0xBB, 0xBF,
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)',', (byte)'3', (byte)',', (byte)'4', (byte)',', (byte)'5', (byte)')',
                (byte)'\r', (byte)'\n', (byte)' ', (byte)'\t',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)'\n'
            ]);
    }

    [Fact]
    public void Run_rejects_a_missing_generator_without_reading_stdin()
    {
        AssertFailure([], "missing G");
    }

    [Fact]
    public void Run_checks_options_before_missing_generators()
    {
        AssertFailure(
            ["--degree=0"],
            "invalid --degree \"0\" (expected an integer from 1 to 64)");
    }

    [Fact]
    public void Run_reports_unknown_options_before_deferred_invalid_values()
    {
        AssertFailure(
            ["--degree=0", "--help"],
            "unknown option \"--help\"");
    }

    [Theory]
    [InlineData("--", "unknown option \"--\"")]
    [InlineData("--help", "unknown option \"--help\"")]
    [InlineData("--verbose", "unknown option \"--verbose\"")]
    public void Run_rejects_every_unsupported_option(string option, string expectedMessage)
    {
        AssertFailure([option], expectedMessage);
    }

    [Fact]
    public void Run_reports_a_degree_option_without_a_value()
    {
        AssertFailure(["--degree"], "--degree needs a value");
    }

    [Theory]
    [InlineData("0", "invalid --degree \"0\" (expected an integer from 1 to 64)")]
    [InlineData("65", "invalid --degree \"65\" (expected an integer from 1 to 64)")]
    [InlineData("+3", "invalid --degree \"+3\" (expected an integer from 1 to 64)")]
    [InlineData("007", "invalid --degree \"007\" (expected an integer from 1 to 64)")]
    [InlineData("1.5", "invalid --degree \"1.5\" (expected an integer from 1 to 64)")]
    [InlineData("٣", "invalid --degree \"?\" (expected an integer from 1 to 64)")]
    public void Run_rejects_invalid_degree_values_with_exact_diagnostics(
        string degree,
        string expectedMessage)
    {
        AssertFailure(["--degree", degree, "()"], expectedMessage);
    }

    [Fact]
    public void Run_checks_invalid_options_before_reading_stdin()
    {
        AssertFailure(["--degree=0", "-"], "invalid --degree \"0\" (expected an integer from 1 to 64)");
    }

    [Fact]
    public void Run_checks_the_generator_limit_before_parsing_malformed_generators()
    {
        AssertFailure(
            ["(1,2", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)"],
            "too many generators (at most 8)");
    }

    [Fact]
    public void Run_checks_the_generator_limit_for_nine_stdin_words()
    {
        AssertFailure(
            ["-"],
            "too many generators (at most 8)",
            1,
            [
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)' ',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')'
            ]);
    }

    [Fact]
    public void Run_treats_dash_with_another_argument_as_an_ordinary_word()
    {
        AssertFailure(
            ["-", "()"],
            "invalid permutation \"-\" (expected cycles such as (1,2,3)(4,5), or ())");
    }

    [Theory]
    [InlineData("(1,2", "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1)", "invalid permutation \"(1)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1, 2)", "invalid permutation \"(1,?2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(+1,2)", "invalid permutation \"(+1,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(01,2)", "invalid permutation \"(01,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,2) (3,4)", "invalid permutation \"(1,2)?(3,4)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(١,2)", "invalid permutation \"(?,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    [InlineData("(1,,2)", "invalid permutation \"(1,,2)\" (expected cycles such as (1,2,3)(4,5), or ())")]
    public void Run_preserves_exact_malformed_permutation_diagnostics(
        string generator,
        string expectedMessage)
    {
        AssertFailure([generator], expectedMessage);
    }

    [Theory]
    [InlineData("(1,65)", "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)")]
    [InlineData("(0,1)", "invalid point \"0\" in \"(0,1)\" (expected 1 to 64)")]
    [InlineData("(1,2)(2,3)", "point 2 is repeated in \"(1,2)(2,3)\"")]
    public void Run_preserves_exact_invalid_point_and_repetition_diagnostics(
        string generator,
        string expectedMessage)
    {
        AssertFailure([generator], expectedMessage);
    }

    [Fact]
    public void Run_checks_above_degree_points_after_parsing_all_texts()
    {
        AssertFailure(
            ["--degree", "3", "(1,5)", "(1,2"],
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())");
    }

    [Fact]
    public void Run_reports_an_above_degree_point_after_validating_each_text()
    {
        AssertFailure(
            ["--degree", "3", "(1,5)"],
            "point 5 of \"(1,5)\" is above the degree 3");
    }

    [Fact]
    public void Run_checks_invalid_degree_before_generator_count_and_notation()
    {
        AssertFailure(
            ["--degree=0", "(1,2", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)"],
            "invalid --degree \"0\" (expected an integer from 1 to 64)");
    }

    [Fact]
    public void Run_checks_invalid_degree_before_a_single_malformed_generator()
    {
        AssertFailure(
            ["--degree=0", "(1,2"],
            "invalid --degree \"0\" (expected an integer from 1 to 64)");
    }

    [Fact]
    public void Run_checks_whole_cycle_shape_before_point_ranges()
    {
        AssertFailure(
            ["(0,65)x"],
            "invalid permutation \"(0,65)x\" (expected cycles such as (1,2,3)(4,5), or ())");
    }

    [Fact]
    public void Run_checks_point_ranges_before_repeated_points()
    {
        AssertFailure(
            ["(1,1)(2,65)"],
            "invalid point \"65\" in \"(1,1)(2,65)\" (expected 1 to 64)");
    }

    [Fact]
    public void Run_rejects_empty_stdin()
    {
        AssertFailure(["-"], "<stdin>: no input", 1, []);
    }

    [Fact]
    public void Run_rejects_whitespace_only_stdin()
    {
        AssertFailure(
            ["-"],
            "<stdin>: no input",
            1,
            [(byte)' ', (byte)'\t', (byte)'\r', (byte)'\n']);
    }

    [Fact]
    public void Run_rejects_malformed_UTF8_stdin_bytes()
    {
        AssertFailure(
            ["-"],
            "<stdin>: input is not valid UTF-8",
            1,
            [0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A]);
    }

    [Fact]
    public void Run_translates_stdin_read_IOExceptions()
    {
        AssertFailure(
            ["-"],
            "<stdin>: cannot read the input",
            1,
            readException: new IOException("unavailable"));
    }

    [Fact]
    public void Run_reads_stdin_exactly_once_only_for_the_sole_dash()
    {
        AssertSuccess(
            ["--degree=1", "()"],
            "degree      1\ngens        1\norder       1\nbase        none\nsizes       none\n");
    }

    [Fact]
    public void Run_uses_invariant_decimal_output_under_a_different_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

            AssertSuccess(
                ["(1,2,3,4,5)", "(1,2)"],
                "degree      5\ngens        2\norder       120\nbase        1 2 3 4\nsizes       5 4 3 2\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Fact]
    public void Run_handles_a_literal_degree_sixty_four_eight_generator_workload()
    {
        AssertSuccess(
            [
                "--degree=64",
                "(1,2)",
                "(2,3)",
                "(3,4)",
                "(4,5)",
                "(5,6)",
                "(6,7)",
                "(7,8)",
                "(8,9)"
            ],
            "degree      64\ngens        8\norder       362880\nbase        1 2 3 4 5 6 7 8\nsizes       9 8 7 6 5 4 3 2\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    private static void AssertSuccess(
        IReadOnlyList<string> argv,
        string expectedOutput,
        int expectedReadCount = 0,
        byte[]? stdinBytes = null,
        TimeSpan? maximumDuration = null)
    {
        var io = new FakeIo(stdinBytes ?? []);
        var stopwatch = Stopwatch.StartNew();

        var exitCode = OrderCommand.Run(argv, io);

        stopwatch.Stop();
        Assert.Equal(0, exitCode);
        Assert.Equal(expectedReadCount, io.ReadCount);
        Assert.Equal(1, io.OutCalls);
        Assert.Single(io.Output);
        Assert.Equal(expectedOutput, io.Output[0]);
        Assert.Empty(io.Errors);
        Assert.EndsWith("\n", io.Output[0]);
        Assert.DoesNotContain('\r', io.Output[0]);
        Assert.DoesNotContain('\uFEFF', io.Output[0]);
        if (maximumDuration is not null)
        {
            Assert.True(
                stopwatch.Elapsed < maximumDuration,
                $"Run took {stopwatch.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture)} seconds; allowed {maximumDuration.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture)} seconds.");
        }
    }

    private static void AssertFailure(
        IReadOnlyList<string> argv,
        string expectedMessage,
        int expectedReadCount = 0,
        byte[]? stdinBytes = null,
        IOException? readException = null)
    {
        var io = new FakeIo(stdinBytes ?? [], readException);

        var exception = Assert.Throws<CayleyException>(() => OrderCommand.Run(argv, io));

        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(expectedReadCount, io.ReadCount);
        Assert.Equal(0, io.OutCalls);
        Assert.Equal(0, io.ErrCalls);
        Assert.Empty(io.Output);
        Assert.Empty(io.Errors);
    }

    private sealed class FakeIo : IIo
    {
        private readonly byte[] bytes;
        private readonly IOException? readException;

        public FakeIo(byte[] bytes, IOException? readException = null)
        {
            this.bytes = bytes;
            this.readException = readException;
        }

        public int ReadCount { get; private set; }

        public int OutCalls { get; private set; }

        public int ErrCalls { get; private set; }

        public List<string> Output { get; } = [];

        public List<string> Errors { get; } = [];

        public byte[] ReadStdin()
        {
            ReadCount++;
            if (readException is not null)
            {
                throw readException;
            }

            return bytes;
        }

        public void Out(string text)
        {
            OutCalls++;
            Output.Add(text);
        }

        public void Err(string text)
        {
            ErrCalls++;
            Errors.Add(text);
        }
    }
}
