namespace Cayleykit.Tests;

public class IoTests
{
    [Fact]
    public void Of_decodes_the_specified_BOM_separators_and_permutation_words()
    {
        var io = new FakeIo(
        [
            0xEF, 0xBB, 0xBF,
            (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)',', (byte)'3', (byte)',', (byte)'4', (byte)',', (byte)'5', (byte)')',
            (byte)'\r', (byte)'\n', (byte)' ', (byte)'\t',
            (byte)'(', (byte)'1', (byte)',', (byte)'2', (byte)')', (byte)'\n'
        ]);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "(1,2,3,4,5)", "(1,2)" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_preserves_valid_multibyte_UTF8()
    {
        var io = new FakeIo([0xC3, 0xA9, (byte)' ', 0xF0, 0x9F, 0x98, 0x80]);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "é", "😀" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_removes_exactly_one_leading_BOM()
    {
        var io = new FakeIo([0xEF, 0xBB, 0xBF, 0xEF, 0xBB, 0xBF, (byte)'x']);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "\uFEFFx" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_preserves_a_BOM_inside_a_word()
    {
        var io = new FakeIo([(byte)'x', 0xEF, 0xBB, 0xBF, (byte)'y']);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "x\uFEFFy" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_splits_only_the_four_specified_separators_and_drops_empty_parts()
    {
        var io = new FakeIo(
        [
            (byte)' ', (byte)'\t', (byte)'\r', (byte)'\n',
            (byte)'a', (byte)' ', (byte)' ', (byte)'b', (byte)'\t', (byte)'\t',
            (byte)'c', (byte)'\r', (byte)'\r', (byte)'d', (byte)'\n', (byte)'\n'
        ]);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "a", "b", "c", "d" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_preserves_nonbreaking_space_and_vertical_tab_inside_words()
    {
        var io = new FakeIo([(byte)'a', 0xC2, 0xA0, (byte)'b', (byte)' ', (byte)'c', 0x0B, (byte)'d']);

        var words = Words.Of(io, new[] { "-" });

        Assert.Equal(new[] { "a\u00A0b", "c\u000Bd" }, words);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_rejects_bytes_28_31_2C_32_29_FF_0A()
    {
        var io = new FakeIo(new byte[] { 0x28, 0x31, 0x2C, 0x32, 0x29, 0xFF, 0x0A });

        var exception = Assert.Throws<CayleyException>(() => Words.Of(io, new[] { "-" }));

        Assert.Equal("<stdin>: input is not valid UTF-8", exception.Message);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Theory]
    [InlineData(new byte[] { 0xC3 })]
    [InlineData(new byte[] { 0xC0, 0xAF })]
    [InlineData(new byte[] { 0xED, 0xA0, 0x80 })]
    [InlineData(new byte[] { 0xE2, 0x28, 0xA1 })]
    public void Of_rejects_malformed_UTF8_with_the_exact_error(byte[] bytes)
    {
        var io = new FakeIo(bytes);

        var exception = Assert.Throws<CayleyException>(() => Words.Of(io, new[] { "-" }));

        Assert.Equal("<stdin>: input is not valid UTF-8", exception.Message);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_translates_an_IOException_from_reading()
    {
        var io = new FakeIo([]) { ReadException = new IOException("unavailable") };

        var exception = Assert.Throws<CayleyException>(() => Words.Of(io, new[] { "-" }));

        Assert.Equal("<stdin>: cannot read the input", exception.Message);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    [Fact]
    public void Of_reports_no_input_for_empty_bytes()
    {
        AssertNoInput(new byte[] { });
    }

    [Fact]
    public void Of_reports_no_input_for_BOM_only_bytes()
    {
        AssertNoInput(new byte[] { 0xEF, 0xBB, 0xBF });
    }

    [Fact]
    public void Of_reports_no_input_for_only_SPACE_TAB_CR_LF()
    {
        AssertNoInput(new byte[] { 0x20, 0x09, 0x0D, 0x0A, 0x20, 0x09, 0x0D, 0x0A });
    }

    [Fact]
    public void Of_returns_a_fresh_empty_copy_without_reading_for_empty_arguments()
    {
        AssertFreshCopy(Array.Empty<string>(), Array.Empty<string>());
    }

    [Fact]
    public void Of_returns_a_fresh_copy_without_reading_for_a_positional_argument()
    {
        AssertFreshCopy(new[] { "P" }, new[] { "P" });
    }

    [Fact]
    public void Of_treats_a_dash_among_other_arguments_as_an_ordinary_word()
    {
        AssertFreshCopy(new[] { "-", "P" }, new[] { "-", "P" });
    }

    [Fact]
    public void Of_treats_two_dashes_as_ordinary_words()
    {
        AssertFreshCopy(new[] { "-", "-" }, new[] { "-", "-" });
    }

    private static void AssertFreshCopy(string[] args, string[] expected)
    {
        var io = new FakeIo([]);

        var words = Words.Of(io, args);

        Assert.Equal(expected, words);
        Assert.NotSame(args, words);
        words.Add("changed");
        Assert.Equal(expected, args);
        Assert.Equal(0, io.ReadCount);
        AssertNoWrites(io);
    }

    private static void AssertNoInput(byte[] bytes)
    {
        var io = new FakeIo(bytes);

        var exception = Assert.Throws<CayleyException>(() => Words.Of(io, new[] { "-" }));

        Assert.Equal("<stdin>: no input", exception.Message);
        Assert.Equal(1, io.ReadCount);
        AssertNoWrites(io);
    }

    private static void AssertNoWrites(FakeIo io)
    {
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
