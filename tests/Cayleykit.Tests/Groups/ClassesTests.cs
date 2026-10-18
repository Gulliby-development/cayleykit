using System.Diagnostics;

namespace Cayleykit.Tests;

public class ClassesTests
{
    [Fact]
    public void Of_returns_the_exact_s3_classes()
    {
        var elements = ParseElements(
            3,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
        var generators = ParseGenerators(3, "(1,2,3)", "(1,2)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(2,3)", 3),
            ("(1,2,3)", 2));
    }

    [Fact]
    public void Of_returns_the_exact_s4_classes_within_the_time_budget()
    {
        var elements = ParseElements(
            4,
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
        var generators = ParseGenerators(4, "(1,2,3,4)", "(1,2)");
        var stopwatch = Stopwatch.StartNew();

        var classes = Classes.Of(elements, generators);

        stopwatch.Stop();

        AssertClasses(classes,
            ("()", 1),
            ("(3,4)", 6),
            ("(2,3,4)", 8),
            ("(1,2)(3,4)", 3),
            ("(1,2,3,4)", 6));
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"S4 class enumeration took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Of_returns_the_exact_a4_classes_and_splits_the_three_cycles()
    {
        var elements = ParseElements(
            4,
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
        var generators = ParseGenerators(4, "(1,2,3)", "(1,2,4)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(2,3,4)", 4),
            ("(2,4,3)", 4),
            ("(1,2)(3,4)", 3));
    }

    [Fact]
    public void Of_returns_the_exact_dihedral_order_eight_classes()
    {
        var elements = ParseElements(
            4,
            "()",
            "(2,4)",
            "(1,2)(3,4)",
            "(1,2,3,4)",
            "(1,3)",
            "(1,3)(2,4)",
            "(1,4,3,2)",
            "(1,4)(2,3)");
        var generators = ParseGenerators(4, "(1,2,3,4)", "(2,4)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(2,4)", 2),
            ("(1,2)(3,4)", 2),
            ("(1,2,3,4)", 2),
            ("(1,3)(2,4)", 1));
    }

    [Fact]
    public void Of_returns_the_exact_quaternion_group_classes()
    {
        var elements = ParseElements(
            8,
            "()",
            "(1,2)(3,4)(5,6)(7,8)",
            "(1,3,2,4)(5,7,6,8)",
            "(1,4,2,3)(5,8,6,7)",
            "(1,5,2,6)(3,8,4,7)",
            "(1,6,2,5)(3,7,4,8)",
            "(1,7,2,8)(3,5,4,6)",
            "(1,8,2,7)(3,6,4,5)");
        var generators = ParseGenerators(
            8,
            "(1,3,2,4)(5,7,6,8)",
            "(1,5,2,6)(3,8,4,7)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(1,2)(3,4)(5,6)(7,8)", 1),
            ("(1,3,2,4)(5,7,6,8)", 2),
            ("(1,5,2,6)(3,8,4,7)", 2),
            ("(1,7,2,8)(3,5,4,6)", 2));
    }

    [Fact]
    public void Of_returns_the_trivial_class_for_an_empty_generator_list()
    {
        var elements = ParseElements(3, "()");

        AssertClasses(Classes.Of(elements, Array.Empty<Perm>()),
            ("()", 1));
    }

    [Fact]
    public void Of_returns_the_trivial_class_for_identity_generators()
    {
        var elements = ParseElements(2, "()");
        var generators = ParseGenerators(2, "()", "()");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1));
    }

    [Fact]
    public void Of_returns_each_element_as_a_class_for_an_abelian_cyclic_group()
    {
        var elements = ParseElements(3, "()", "(1,2,3)", "(1,3,2)");
        var generators = ParseGenerators(3, "(1,2,3)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(1,2,3)", 1),
            ("(1,3,2)", 1));
    }

    [Fact]
    public void Of_ignores_duplicate_identity_and_redundant_generators()
    {
        var elements = ParseElements(
            3,
            "()",
            "(2,3)",
            "(1,2)",
            "(1,2,3)",
            "(1,3,2)",
            "(1,3)");
        var generators = ParseGenerators(
            3,
            "()",
            "(1,2,3)",
            "(1,2)",
            "(1,2)",
            "(2,3)");

        AssertClasses(Classes.Of(elements, generators),
            ("()", 1),
            ("(2,3)", 3),
            ("(1,2,3)", 2));
    }

    [Fact]
    public void Of_returns_the_same_ordered_classes_on_repeated_calls()
    {
        var elements = ParseElements(
            4,
            "()",
            "(2,4)",
            "(1,2)(3,4)",
            "(1,2,3,4)",
            "(1,3)",
            "(1,3)(2,4)",
            "(1,4,3,2)",
            "(1,4)(2,3)");
        var generators = ParseGenerators(4, "(1,2,3,4)", "(2,4)");

        var expected = new[]
        {
            ("()", 1),
            ("(2,4)", 2),
            ("(1,2)(3,4)", 2),
            ("(1,2,3,4)", 2),
            ("(1,3)(2,4)", 1)
        };

        AssertClasses(Classes.Of(elements, generators), expected);
        AssertClasses(Classes.Of(elements, generators), expected);
    }

    private static List<Perm> ParseElements(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static List<Perm> ParseGenerators(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static void AssertClasses(List<ConjClass> actual, params (string Representative, int Size)[] expected)
    {
        Assert.Equal(expected, actual.Select(conjClass => (Notation.Format(conjClass.Representative), conjClass.Size)));
    }
}
