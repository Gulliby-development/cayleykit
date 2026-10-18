using System.Diagnostics;

namespace Cayleykit.Tests;

public class ClosureTests
{
    [Fact]
    public void Elements_returns_the_complete_sorted_s3()
    {
        var generators = Parse(3, "(1,2,3)", "(1,2)");

        var elements = Closure.Elements(3, generators, 6);

        AssertElements(elements,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
    }

    [Fact]
    public void Elements_returns_the_complete_sorted_s4()
    {
        var generators = Parse(4, "(1,2,3,4)", "(1,2)");

        var elements = Closure.Elements(4, generators, 24);

        AssertElements(elements,
            "()",
            "(3,4)",
            "(2,3)",
            "(2,3,4)",
            "(2,4,3)",
            "(2,4)",
            "(1,2)",
            "(1,2)(3,4)",
            "(1,2,3)",
            "(1,2,3,4)",
            "(1,2,4,3)",
            "(1,2,4)",
            "(1,3,2)",
            "(1,3,4,2)",
            "(1,3)",
            "(1,3,4)",
            "(1,3)(2,4)",
            "(1,3,2,4)",
            "(1,4,3,2)",
            "(1,4,2)",
            "(1,4,3)",
            "(1,4)",
            "(1,4,2,3)",
            "(1,4)(2,3)");
    }

    [Fact]
    public void Elements_returns_the_complete_sorted_a4()
    {
        var generators = Parse(4, "(1,2,3)", "(1,2,4)");

        var elements = Closure.Elements(4, generators, 12);

        AssertElements(elements,
            "()",
            "(2,3,4)",
            "(2,4,3)",
            "(1,2)(3,4)",
            "(1,2,3)",
            "(1,2,4)",
            "(1,3,2)",
            "(1,3,4)",
            "(1,3)(2,4)",
            "(1,4,2)",
            "(1,4,3)",
            "(1,4)(2,3)");
    }

    [Fact]
    public void Elements_returns_the_complete_sorted_dihedral_group_of_order_eight()
    {
        var generators = Parse(4, "(1,2,3,4)", "(2,4)");

        var elements = Closure.Elements(4, generators, 8);

        AssertElements(elements,
            "()",
            "(2,4)",
            "(1,2)(3,4)",
            "(1,2,3,4)",
            "(1,3)",
            "(1,3)(2,4)",
            "(1,4,3,2)",
            "(1,4)(2,3)");
    }

    [Fact]
    public void Elements_returns_the_complete_sorted_quaternion_group()
    {
        var generators = Parse(
            8,
            "(1,3,2,4)(5,7,6,8)",
            "(1,5,2,6)(3,8,4,7)");

        var elements = Closure.Elements(8, generators, 8);

        AssertElements(elements,
            "()",
            "(1,2)(3,4)(5,6)(7,8)",
            "(1,3,2,4)(5,7,6,8)",
            "(1,4,2,3)(5,8,6,7)",
            "(1,5,2,6)(3,8,4,7)",
            "(1,6,2,5)(3,7,4,8)",
            "(1,7,2,8)(3,5,4,6)",
            "(1,8,2,7)(3,6,4,5)");
    }

    [Fact]
    public void Elements_includes_the_last_permitted_element_at_exact_group_order()
    {
        var generators = Parse(3, "(1,2,3)", "(1,2)");

        var elements = Closure.Elements(3, generators, 6);

        AssertElements(elements,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
    }

    [Fact]
    public void Elements_returns_null_when_the_group_exceeds_the_limit_by_one()
    {
        var generators = Parse(3, "(1,2,3)", "(1,2)");

        Assert.Null(Closure.Elements(3, generators, 5));
    }

    [Fact]
    public void Elements_of_the_empty_generator_list_is_the_trivial_group()
    {
        AssertElements(Closure.Elements(3, Array.Empty<Perm>(), 1), "()");
    }

    [Fact]
    public void Elements_of_identity_generators_is_the_trivial_group()
    {
        var generators = Parse(3, "()", "()");

        AssertElements(Closure.Elements(3, generators, 1), "()");
    }

    [Fact]
    public void Elements_ignores_duplicate_and_redundant_generators()
    {
        var generators = Parse(3, "(1,2,3)", "(1,2)", "(1,2)", "(2,3)");

        AssertElements(Closure.Elements(3, generators, 6),
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
    }

    [Fact]
    public void Elements_returns_null_for_zero_limit_with_identity_generators()
    {
        var generators = Parse(2, "()");

        Assert.Null(Closure.Elements(2, generators, 0));
    }

    [Fact]
    public void Elements_returns_null_for_zero_limit_even_for_the_trivial_group()
    {
        Assert.Null(Closure.Elements(2, Array.Empty<Perm>(), 0));
    }

    [Fact]
    public void Elements_returns_null_for_negative_limit()
    {
        var generators = Parse(2, "()");

        Assert.Null(Closure.Elements(2, generators, -1));
    }

    [Fact]
    public void Elements_cuts_off_s8_at_a_small_limit_within_the_time_budget()
    {
        var generators = Parse(8, "(1,2,3,4,5,6,7,8)", "(1,2)");
        var stopwatch = Stopwatch.StartNew();

        var elements = Closure.Elements(8, generators, 1000);

        stopwatch.Stop();

        Assert.Null(elements);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"S8 cutoff took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Elements_returns_identical_sorted_results_on_repeated_calls()
    {
        var generators = Parse(3, "(1,2,3)", "(1,2)");

        var first = Closure.Elements(3, generators, 6);
        var second = Closure.Elements(3, generators, 6);

        AssertElements(first,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
        AssertElements(second,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
    }

    private static List<Perm> Parse(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static void AssertElements(List<Perm>? elements, params string[] expected)
    {
        Assert.NotNull(elements);
        Assert.Equal(expected, elements!.Select(Notation.Format));
    }
}
