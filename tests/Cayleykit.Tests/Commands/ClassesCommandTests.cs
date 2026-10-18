using System.Diagnostics;

namespace Cayleykit.Tests;

public class ClassesCommandTests
{
    [Fact]
    public void Run_matches_the_complete_supplied_s4_classes_example()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(new[] { "(1,2,3,4)", "(1,2)" }, io);

        AssertResult(
            result,
            "degree      4\n" +
            "order       24\n" +
            "classes     5\n" +
            "center      1\n" +
            "abelian     no\n" +
            "class       size 1  order 1  type 1 1 1 1  rep ()\n" +
            "class       size 6  order 2  type 2 1 1  rep (3,4)\n" +
            "class       size 8  order 3  type 3 1  rep (2,3,4)\n" +
            "class       size 3  order 2  type 2 2  rep (1,2)(3,4)\n" +
            "class       size 6  order 4  type 4  rep (1,2,3,4)\n",
            io);
    }

    [Fact]
    public void Run_matches_the_complete_supplied_quaternion_classes_example()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(
            new[] { "(1,3,2,4)(5,7,6,8)", "(1,5,2,6)(3,8,4,7)" },
            io);

        AssertResult(
            result,
            "degree      8\n" +
            "order       8\n" +
            "classes     5\n" +
            "center      2\n" +
            "abelian     no\n" +
            "class       size 1  order 1  type 1 1 1 1 1 1 1 1  rep ()\n" +
            "class       size 1  order 2  type 2 2 2 2  rep (1,2)(3,4)(5,6)(7,8)\n" +
            "class       size 2  order 4  type 4 4  rep (1,3,2,4)(5,7,6,8)\n" +
            "class       size 2  order 4  type 4 4  rep (1,5,2,6)(3,8,4,7)\n" +
            "class       size 2  order 4  type 4 4  rep (1,7,2,8)(3,5,4,6)\n",
            io);
    }

    [Fact]
    public void Run_returns_one_abelian_class_for_the_trivial_group()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(new[] { "()" }, io);

        AssertResult(
            result,
            "degree      1\n" +
            "order       1\n" +
            "classes     1\n" +
            "center      1\n" +
            "abelian     yes\n" +
            "class       size 1  order 1  type 1  rep ()\n",
            io);
    }

    [Fact]
    public void Run_returns_each_element_as_a_class_for_a_cyclic_group()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(new[] { "(1,2,3)" }, io);

        AssertResult(
            result,
            "degree      3\n" +
            "order       3\n" +
            "classes     3\n" +
            "center      3\n" +
            "abelian     yes\n" +
            "class       size 1  order 1  type 1 1 1  rep ()\n" +
            "class       size 1  order 3  type 3  rep (1,2,3)\n" +
            "class       size 1  order 3  type 3  rep (1,3,2)\n",
            io);
    }

    [Fact]
    public void Run_does_not_duplicate_classes_for_duplicate_and_redundant_generators()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(
            new[] { "()", "(1,2,3)", "(1,2)", "(1,2)", "(2,3)" },
            io);

        AssertResult(
            result,
            "degree      3\n" +
            "order       6\n" +
            "classes     3\n" +
            "center      1\n" +
            "abelian     no\n" +
            "class       size 1  order 1  type 1 1 1  rep ()\n" +
            "class       size 3  order 2  type 2 1  rep (2,3)\n" +
            "class       size 2  order 3  type 3  rep (1,2,3)\n",
            io);
    }

    [Fact]
    public void Run_uses_the_last_repeated_equals_degree_option()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(
            new[] { "--degree=5", "--degree=8", "(7,8)" },
            io);

        AssertResult(
            result,
            "degree      8\n" +
            "order       2\n" +
            "classes     2\n" +
            "center      2\n" +
            "abelian     yes\n" +
            "class       size 1  order 1  type 1 1 1 1 1 1 1 1  rep ()\n" +
            "class       size 1  order 2  type 2 1 1 1 1 1 1  rep (7,8)\n",
            io);
    }

    [Fact]
    public void Run_accepts_degree_sixty_four_and_a_generator_at_its_boundary()
    {
        var io = new FakeIo([]);

        var result = ClassesCommand.Run(new[] { "--degree", "64", "(63,64)" }, io);

        AssertResult(
            result,
            "degree      64\n" +
            "order       2\n" +
            "classes     2\n" +
            "center      2\n" +
            "abelian     yes\n" +
            "class       size 1  order 1  type 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1  rep ()\n" +
            "class       size 1  order 2  type 2 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1  rep (63,64)\n",
            io);
    }

    [Fact]
    public void Run_rejects_the_seven_point_symmetric_group_above_720()
    {
        AssertFailure(
            "group order 5040 is above 720 (too large for classes)",
            new[] { "(1,2,3,4,5,6,7)", "(1,2)" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_accepts_the_exact_720_element_six_point_symmetric_group_within_the_time_budget()
    {
        var io = new FakeIo([]);
        var stopwatch = Stopwatch.StartNew();

        var result = ClassesCommand.Run(new[] { "(1,2,3,4,5,6)", "(1,2)" }, io);

        stopwatch.Stop();

        AssertResult(
            result,
            "degree      6\n" +
            "order       720\n" +
            "classes     11\n" +
            "center      1\n" +
            "abelian     no\n" +
            "class       size 1  order 1  type 1 1 1 1 1 1  rep ()\n" +
            "class       size 15  order 2  type 2 1 1 1 1  rep (5,6)\n" +
            "class       size 40  order 3  type 3 1 1 1  rep (4,5,6)\n" +
            "class       size 45  order 2  type 2 2 1 1  rep (3,4)(5,6)\n" +
            "class       size 90  order 4  type 4 1 1  rep (3,4,5,6)\n" +
            "class       size 120  order 6  type 3 2 1  rep (2,3)(4,5,6)\n" +
            "class       size 144  order 5  type 5 1  rep (2,3,4,5,6)\n" +
            "class       size 15  order 2  type 2 2 2  rep (1,2)(3,4)(5,6)\n" +
            "class       size 90  order 4  type 4 2  rep (1,2)(3,4,5,6)\n" +
            "class       size 40  order 3  type 3 3  rep (1,2,3)(4,5,6)\n" +
            "class       size 120  order 6  type 6  rep (1,2,3,4,5,6)\n",
            io);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(10),
            $"S6 class enumeration took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Run_rejects_unknown_help_option_before_missing_generators()
    {
        AssertFailure(
            "unknown option \"--help\"",
            new[] { "--help" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_rejects_a_missing_degree_value_before_missing_generators()
    {
        AssertFailure(
            "--degree needs a value",
            new[] { "--degree" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_validates_degree_before_missing_generators()
    {
        AssertFailure(
            "invalid --degree \"0\" (expected an integer from 1 to 64)",
            new[] { "--degree", "0" },
            new FakeIo([]));
    }

    [Fact]
    public void Run_reports_missing_generators_without_reading_stdin_or_writing()
    {
        AssertFailure("missing G", Array.Empty<string>(), new FakeIo([]));
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
        var io = new FakeIo(System.Text.Encoding.ASCII.GetBytes(
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

        var result = ClassesCommand.Run(new[] { "-" }, io);

        AssertResult(
            result,
            "degree      3\n" +
            "order       6\n" +
            "classes     3\n" +
            "center      1\n" +
            "abelian     no\n" +
            "class       size 1  order 1  type 1 1 1  rep ()\n" +
            "class       size 3  order 2  type 2 1  rep (2,3)\n" +
            "class       size 2  order 3  type 3  rep (1,2,3)\n",
            io);
        Assert.Equal(1, io.ReadCount);
    }

    [Fact]
    public void Run_rejects_the_exact_malformed_utf8_bytes_from_stdin()
    {
        var io = new FakeIo(new byte[] { 0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A });

        AssertFailure("<stdin>: input is not valid UTF-8", new[] { "-" }, io);
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
    public void Run_does_not_read_stdin_when_dash_is_mixed_with_other_generators()
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
    public void Run_checks_all_notation_syntax_before_explicit_degree_overflow()
    {
        AssertFailure(
            "invalid permutation \"bad\" (expected cycles such as (1,2,3)(4,5), or ())",
            new[] { "--degree", "3", "(1,5)", "bad" },
            new FakeIo([]));
    }

    private static void AssertResult(int result, string expectedOutput, FakeIo io)
    {
        Assert.Equal(0, result);
        Assert.Equal(expectedOutput, Assert.Single(io.Output));
        Assert.Empty(io.Errors);
    }

    private static void AssertFailure(string expectedMessage, IReadOnlyList<string> argv, FakeIo io)
    {
        var exception = Assert.Throws<CayleyException>(() => ClassesCommand.Run(argv, io));

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
