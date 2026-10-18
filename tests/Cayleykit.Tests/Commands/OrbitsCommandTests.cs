using System.Diagnostics;
using System.Globalization;

namespace Cayleykit.Tests;

public class OrbitsCommandTests
{
    [Fact]
    public void Run_reports_two_sorted_disjoint_orbits_and_exact_stabilizers()
    {
        AssertSuccess(
            ["(1,2,3)", "(4,5)"],
            "degree      5\norder       6\norbits      2\norbit       {1,2,3}  size 3  stab 2\norbit       {4,5}  size 2  stab 3\ntransitive  no\n");
    }

    [Fact]
    public void Run_reports_the_transitive_symmetric_group_on_four_points()
    {
        AssertSuccess(
            ["(1,2,3,4)", "(1,2)"],
            "degree      4\norder       24\norbits      1\norbit       {1,2,3,4}  size 4  stab 6\ntransitive  yes\n");
    }

    [Fact]
    public void Run_reports_the_singleton_orbit_of_the_degree_one_identity()
    {
        AssertSuccess(
            ["()"],
            "degree      1\norder       1\norbits      1\norbit       {1}  size 1  stab 1\ntransitive  yes\n");
    }

    [Fact]
    public void Run_reports_fixed_points_and_exact_stabilizers_at_explicit_degree()
    {
        AssertSuccess(
            ["--degree", "3", "(1,2)"],
            "degree      3\norder       2\norbits      2\norbit       {1,2}  size 2  stab 1\norbit       {3}  size 1  stab 2\ntransitive  no\n");
    }

    [Fact]
    public void Run_accepts_an_inline_degree_option()
    {
        AssertSuccess(
            ["--degree=8", "()"],
            "degree      8\norder       1\norbits      8\norbit       {1}  size 1  stab 1\norbit       {2}  size 1  stab 1\norbit       {3}  size 1  stab 1\norbit       {4}  size 1  stab 1\norbit       {5}  size 1  stab 1\norbit       {6}  size 1  stab 1\norbit       {7}  size 1  stab 1\norbit       {8}  size 1  stab 1\ntransitive  no\n");
    }

    [Fact]
    public void Run_uses_the_last_repeated_degree_option_and_allows_interspersing()
    {
        AssertSuccess(
            ["--degree", "3", "(1,2)", "--degree=5"],
            "degree      5\norder       2\norbits      4\norbit       {1,2}  size 2  stab 1\norbit       {3}  size 1  stab 2\norbit       {4}  size 1  stab 2\norbit       {5}  size 1  stab 2\ntransitive  no\n");
    }

    [Fact]
    public void Run_infers_the_largest_point_and_counts_redundant_generators()
    {
        AssertSuccess(
            ["(1,2)", "()", "(1,2)", "(4,5)"],
            "degree      5\norder       4\norbits      3\norbit       {1,2}  size 2  stab 2\norbit       {3}  size 1  stab 4\norbit       {4,5}  size 2  stab 2\ntransitive  no\n");
    }

    [Fact]
    public void Run_accepts_explicit_degree_one()
    {
        AssertSuccess(
            ["--degree=1", "()"],
            "degree      1\norder       1\norbits      1\norbit       {1}  size 1  stab 1\ntransitive  yes\n");
    }

    [Fact]
    public void Run_accepts_point_sixty_four_at_the_degree_boundary()
    {
        AssertSuccess(
            ["--degree=64", "(1,64)"],
            "degree      64\norder       2\norbits      63\norbit       {1,64}  size 2  stab 1\norbit       {2}  size 1  stab 2\norbit       {3}  size 1  stab 2\norbit       {4}  size 1  stab 2\norbit       {5}  size 1  stab 2\norbit       {6}  size 1  stab 2\norbit       {7}  size 1  stab 2\norbit       {8}  size 1  stab 2\norbit       {9}  size 1  stab 2\norbit       {10}  size 1  stab 2\norbit       {11}  size 1  stab 2\norbit       {12}  size 1  stab 2\norbit       {13}  size 1  stab 2\norbit       {14}  size 1  stab 2\norbit       {15}  size 1  stab 2\norbit       {16}  size 1  stab 2\norbit       {17}  size 1  stab 2\norbit       {18}  size 1  stab 2\norbit       {19}  size 1  stab 2\norbit       {20}  size 1  stab 2\norbit       {21}  size 1  stab 2\norbit       {22}  size 1  stab 2\norbit       {23}  size 1  stab 2\norbit       {24}  size 1  stab 2\norbit       {25}  size 1  stab 2\norbit       {26}  size 1  stab 2\norbit       {27}  size 1  stab 2\norbit       {28}  size 1  stab 2\norbit       {29}  size 1  stab 2\norbit       {30}  size 1  stab 2\norbit       {31}  size 1  stab 2\norbit       {32}  size 1  stab 2\norbit       {33}  size 1  stab 2\norbit       {34}  size 1  stab 2\norbit       {35}  size 1  stab 2\norbit       {36}  size 1  stab 2\norbit       {37}  size 1  stab 2\norbit       {38}  size 1  stab 2\norbit       {39}  size 1  stab 2\norbit       {40}  size 1  stab 2\norbit       {41}  size 1  stab 2\norbit       {42}  size 1  stab 2\norbit       {43}  size 1  stab 2\norbit       {44}  size 1  stab 2\norbit       {45}  size 1  stab 2\norbit       {46}  size 1  stab 2\norbit       {47}  size 1  stab 2\norbit       {48}  size 1  stab 2\norbit       {49}  size 1  stab 2\norbit       {50}  size 1  stab 2\norbit       {51}  size 1  stab 2\norbit       {52}  size 1  stab 2\norbit       {53}  size 1  stab 2\norbit       {54}  size 1  stab 2\norbit       {55}  size 1  stab 2\norbit       {56}  size 1  stab 2\norbit       {57}  size 1  stab 2\norbit       {58}  size 1  stab 2\norbit       {59}  size 1  stab 2\norbit       {60}  size 1  stab 2\norbit       {61}  size 1  stab 2\norbit       {62}  size 1  stab 2\norbit       {63}  size 1  stab 2\ntransitive  no\n");
    }

    [Fact]
    public void Run_preserves_identity_and_duplicate_generators()
    {
        AssertSuccess(
            ["()", "(1,2)", "(1,2)", "()"],
            "degree      2\norder       2\norbits      1\norbit       {1,2}  size 2  stab 1\ntransitive  yes\n");
    }

    [Fact]
    public void Run_accepts_eight_generators_at_the_count_boundary()
    {
        AssertSuccess(
            ["()", "()", "()", "()", "()", "()", "()", "()"],
            "degree      1\norder       1\norbits      1\norbit       {1}  size 1  stab 1\ntransitive  yes\n");
    }

    [Fact]
    public void Run_reads_BOM_CRLF_tab_and_space_separated_generators_from_stdin()
    {
        AssertSuccess(
            ["-"],
            "degree      5\norder       120\norbits      1\norbit       {1,2,3,4,5}  size 5  stab 24\ntransitive  yes\n",
            expectedReadCount: 1,
            stdinBytes:
            [
                0xEF, 0xBB, 0xBF,
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)',', (byte)'3', (byte)',', (byte)'4', (byte)',', (byte)'5', (byte)')',
                (byte)'\r', (byte)'\n', (byte)' ', (byte)'\t',
                (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)'\n'
            ]);
    }

    [Fact]
    public void Run_with_literal_generator_arguments_matches_the_stdin_fixture_output()
    {
        AssertSuccess(
            ["(1,2,3,4,5)", "(1,2)"],
            "degree      5\norder       120\norbits      1\norbit       {1,2,3,4,5}  size 5  stab 24\ntransitive  yes\n");
    }

    [Fact]
    public void Run_rejects_missing_generators_without_reading_stdin()
    {
        AssertFailure([], "missing G");
    }

    [Fact]
    public void Run_checks_option_errors_before_missing_generators_and_reads()
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
    public void Run_rejects_unsupported_options(string option, string expectedMessage)
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
    public void Run_rejects_invalid_degree_values_with_exact_messages(string degree, string expectedMessage)
    {
        AssertFailure(["--degree", degree, "()"], expectedMessage);
    }

    [Fact]
    public void Run_checks_invalid_options_before_reading_stdin()
    {
        AssertFailure(
            ["--degree=0", "-"],
            "invalid --degree \"0\" (expected an integer from 1 to 64)");
    }

    [Fact]
    public void Run_checks_the_generator_limit_before_malformed_text()
    {
        AssertFailure(
            ["(1,2", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)", "(1,2)"],
            "too many generators (at most 8)");
    }

    [Fact]
    public void Run_checks_nine_stdin_words_before_parsing_them()
    {
        AssertFailure(
            ["-"],
            "too many generators (at most 8)",
            expectedReadCount: 1,
            stdinBytes:
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
    public void Run_treats_dash_alongside_another_argument_as_an_ordinary_word()
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
    public void Run_preserves_exact_malformed_permutation_messages(string generator, string expectedMessage)
    {
        AssertFailure([generator], expectedMessage);
    }

    [Theory]
    [InlineData("(1,65)", "invalid point \"65\" in \"(1,65)\" (expected 1 to 64)")]
    [InlineData("(0,1)", "invalid point \"0\" in \"(0,1)\" (expected 1 to 64)")]
    [InlineData("(1,2)(2,3)", "point 2 is repeated in \"(1,2)(2,3)\"")]
    public void Run_preserves_exact_point_and_repetition_messages(string generator, string expectedMessage)
    {
        AssertFailure([generator], expectedMessage);
    }

    [Fact]
    public void Run_checks_above_degree_points_after_parsing_all_texts()
    {
        AssertFailure(
            ["--degree=3", "(1,5)", "(1,2"],
            "invalid permutation \"(1,2\" (expected cycles such as (1,2,3)(4,5), or ())");
    }

    [Fact]
    public void Run_reports_the_first_point_above_explicit_degree()
    {
        AssertFailure(
            ["--degree", "3", "(1,5)"],
            "point 5 of \"(1,5)\" is above the degree 3");
    }

    [Fact]
    public void Run_checks_whole_text_shape_before_invalid_point_ranges()
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
        AssertFailure(["-"], "<stdin>: no input", expectedReadCount: 1, stdinBytes: []);
    }

    [Fact]
    public void Run_rejects_whitespace_only_stdin()
    {
        AssertFailure(
            ["-"],
            "<stdin>: no input",
            expectedReadCount: 1,
            stdinBytes: [(byte)' ', (byte)'\t', (byte)'\r', (byte)'\n']);
    }

    [Fact]
    public void Run_rejects_the_literal_malformed_UTF8_stdin_bytes()
    {
        AssertFailure(
            ["-"],
            "<stdin>: input is not valid UTF-8",
            expectedReadCount: 1,
            stdinBytes: [0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A]);
    }

    [Fact]
    public void Run_translates_a_stdin_read_IOException()
    {
        AssertFailure(
            ["-"],
            "<stdin>: cannot read the input",
            expectedReadCount: 1,
            readException: new IOException("unavailable"));
    }

    [Fact]
    public void Run_does_not_read_stdin_for_non_dash_generator_arguments()
    {
        AssertSuccess(
            ["--degree=1", "()"],
            "degree      1\norder       1\norbits      1\norbit       {1}  size 1  stab 1\ntransitive  yes\n");
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
                ["(1,2,3)", "(4,5)"],
                "degree      5\norder       6\norbits      2\norbit       {1,2,3}  size 3  stab 2\norbit       {4,5}  size 2  stab 3\ntransitive  no\n");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    // The exact orbit output and stabilizer are literal values from the pocket-cube fixture.
    [Fact]
    public void Run_reports_the_complete_pocket_cube_orbit_and_stabilizer()
    {
        AssertSuccess(
            [
                "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
                "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
                "(1,7,16,18)(2,5,15,20)(9,11,12,10)"
            ],
            "degree      24\norder       3674160\norbits      4\norbit       {1,2,3,4,5,6,7,8,9,10,11,12,14,15,16,18,19,20,21,23,24}  size 21  stab 174960\norbit       {13}  size 1  stab 3674160\norbit       {17}  size 1  stab 3674160\norbit       {22}  size 1  stab 3674160\ntransitive  no\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    // These three literal point sets are the corner stickers, edge stickers and fixed centers.
    [Fact]
    public void Run_reports_the_complete_three_by_three_cube_orbits_and_stabilizers()
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
            "degree      54\norder       43252003274489856000\norbits      8\norbit       {1,3,7,9,10,12,16,18,19,21,25,27,28,30,34,36,37,39,43,45,46,48,52,54}  size 24  stab 1802166803103744000\norbit       {2,4,6,8,11,13,15,17,20,22,24,26,29,31,33,35,38,40,42,44,47,49,51,53}  size 24  stab 1802166803103744000\norbit       {5}  size 1  stab 43252003274489856000\norbit       {14}  size 1  stab 43252003274489856000\norbit       {23}  size 1  stab 43252003274489856000\norbit       {32}  size 1  stab 43252003274489856000\norbit       {41}  size 1  stab 43252003274489856000\norbit       {50}  size 1  stab 43252003274489856000\ntransitive  no\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void Run_measures_a_degree_sixty_four_eight_generator_transitive_workload()
    {
        AssertSuccess(
            [
                "--degree=64",
                "(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64)",
                "()",
                "()",
                "()",
                "()",
                "()",
                "()",
                "()"
            ],
            "degree      64\norder       64\norbits      1\norbit       {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64}  size 64  stab 1\ntransitive  yes\n",
            maximumDuration: TimeSpan.FromSeconds(10));
    }

    private static void AssertSuccess(
        IReadOnlyList<string> argv,
        string expectedOutput,
        int expectedReadCount = 0,
        byte[]? stdinBytes = null,
        IOException? readException = null,
        TimeSpan? maximumDuration = null)
    {
        var io = new FakeIo(stdinBytes ?? [], readException);
        var stopwatch = Stopwatch.StartNew();

        var exitCode = OrbitsCommand.Run(argv, io);

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

        var exception = Assert.Throws<CayleyException>(() => OrbitsCommand.Run(argv, io));

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
