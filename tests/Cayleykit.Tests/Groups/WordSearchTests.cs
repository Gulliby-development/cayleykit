using System.Diagnostics;

namespace Cayleykit.Tests;

public class WordSearchTests
{
    [Fact]
    public void Find_returns_the_shortest_dictionary_first_s3_word()
    {
        var target = Parse(3, "(1,3)");
        var generators = Parse(3, "(1,2)", "(2,3)");

        Assert.Equal(new[] { 1, 2, 1 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_uses_dictionary_order_for_the_literal_s4_word()
    {
        var target = Parse(4, "(1,2,3)");
        var generators = Parse(4, "(1,2)", "(1,2,3,4)");

        Assert.Equal(new[] { 1, 2, 1, 2, 2, 1 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_returns_a_positive_word_for_an_inverse_cycle()
    {
        var target = Parse(3, "(1,3,2)");
        var generators = ParseGenerators(3, "(1,2,3)");

        Assert.Equal(new[] { 1, 1 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_prefers_the_first_of_duplicate_generators()
    {
        var target = Parse(2, "(1,2)");
        var generators = Parse(2, "(1,2)", "(1,2)");

        Assert.Equal(new[] { 1 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_preserves_original_index_after_an_identity_generator()
    {
        var target = Parse(2, "(1,2)");
        var generators = ParseGenerators(2, "()", "(1,2)");

        Assert.Equal(new[] { 2 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_preserves_original_indices_with_eight_identity_duplicate_and_redundant_generators()
    {
        var target = Parse(3, "(1,3)");
        var generators = Parse(
            3,
            "()",
            "(1,2)",
            "(1,2)",
            "(2,3)",
            "(1,3)",
            "()",
            "(1,2,3)",
            "(1,3,2)");

        Assert.Equal(new[] { 5 }, WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_returns_an_empty_word_for_identity_with_a_generator()
    {
        var target = Parse(2, "()");
        var generators = ParseGenerators(2, "(1,2)");

        Assert.Empty(WordSearch.Find(target, generators)!);
    }

    [Fact]
    public void Find_returns_an_empty_word_for_identity_with_no_generators()
    {
        var target = Parse(1, "()");

        Assert.Empty(WordSearch.Find(target, Array.Empty<Perm>())!);
    }

    [Fact]
    public void Find_returns_null_for_a_target_outside_a_cyclic_group()
    {
        var target = Parse(3, "(1,2)");
        var generators = ParseGenerators(3, "(1,2,3)");

        Assert.Null(WordSearch.Find(target, generators));
    }

    [Fact]
    public void Find_returns_null_for_a_nonidentity_target_with_no_generators()
    {
        var target = Parse(2, "(1,2)");

        Assert.Null(WordSearch.Find(target, Array.Empty<Perm>()));
    }

    [Fact]
    public void Find_exhausts_s8_when_the_target_moves_the_fixed_ninth_point()
    {
        var target = Parse(9, "(8,9)");
        var generators = Parse(9, "(1,2,3,4,5,6,7,8)", "(1,2)");
        var stopwatch = Stopwatch.StartNew();

        var word = WordSearch.Find(target, generators);

        stopwatch.Stop();

        Assert.Null(word);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"S8 exhaustion took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Find_reconstructs_the_literal_63_generator_inverse_word_within_the_time_budget()
    {
        var target = Parse(
            64,
            "(1,64,63,62,61,60,59,58,57,56,55,54,53,52,51,50,49,48,47,46,45,44,43,42,41,40,39,38,37,36,35,34,33,32,31,30,29,28,27,26,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2)");
        var generators = ParseGenerators(
            64,
            "(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64)");
        var stopwatch = Stopwatch.StartNew();

        var word = WordSearch.Find(target, generators);

        stopwatch.Stop();

        Assert.Equal(new[]
        {
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1
        }, word);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"64-cycle reconstruction took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Find_returns_the_same_shortest_word_on_repeated_calls()
    {
        var target = Parse(3, "(1,3)");
        var generators = Parse(3, "(1,2)", "(2,3)");

        Assert.Equal(new[] { 1, 2, 1 }, WordSearch.Find(target, generators));
        Assert.Equal(new[] { 1, 2, 1 }, WordSearch.Find(target, generators));
    }

    private static Perm Parse(int degree, string text)
    {
        return Notation.ParseAll(new[] { text }, degree).Perms[0];
    }

    private static List<Perm> Parse(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static List<Perm> ParseGenerators(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }
}
