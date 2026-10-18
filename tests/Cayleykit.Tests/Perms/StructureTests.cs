using System.Globalization;

namespace Cayleykit.Tests;

public class StructureTests
{
    [Fact]
    public void Structural_fields_include_fixed_points_and_use_descending_cycle_lengths()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1, 5, 4, 6 });

        var cycleType = Structure.CycleType(permutation);
        var oneLine = Structure.OneLine(permutation);
        var sign = Structure.Sign(permutation);

        Assert.Equal("3 2 1", cycleType);
        Assert.Equal("2 3 1 5 4 6", oneLine);
        Assert.Equal("odd", sign);
        AssertNoLineEnding(cycleType);
        AssertNoLineEnding(oneLine);
        AssertNoLineEnding(sign);
    }

    [Fact]
    public void Structural_fields_format_an_identity_with_all_singleton_cycles()
    {
        var permutation = Perm.FromImages(new[] { 1, 2, 3 });

        var cycleType = Structure.CycleType(permutation);
        var oneLine = Structure.OneLine(permutation);
        var sign = Structure.Sign(permutation);

        Assert.Equal("1 1 1", cycleType);
        Assert.Equal("1 2 3", oneLine);
        Assert.Equal("even", sign);
        AssertNoLineEnding(cycleType);
        AssertNoLineEnding(oneLine);
        AssertNoLineEnding(sign);
    }

    [Fact]
    public void Structural_fields_report_an_even_product_of_disjoint_transpositions()
    {
        var permutation = Perm.FromImages(new[] { 2, 1, 4, 3 });

        var cycleType = Structure.CycleType(permutation);
        var oneLine = Structure.OneLine(permutation);
        var sign = Structure.Sign(permutation);

        Assert.Equal("2 2", cycleType);
        Assert.Equal("2 1 4 3", oneLine);
        Assert.Equal("even", sign);
        AssertNoLineEnding(cycleType);
        AssertNoLineEnding(oneLine);
        AssertNoLineEnding(sign);
    }

    [Fact]
    public void Conjugate_and_commutator_match_the_first_asymmetric_pair()
    {
        // P=(1,2,3), Q=(1,2); image arrays keep this test independent of notation parsing.
        var p = Perm.FromImages(new[] { 2, 3, 1 });
        var q = Perm.FromImages(new[] { 2, 1, 3 });

        Assert.Equal(Perm.FromImages(new[] { 3, 1, 2 }), Structure.Conjugate(p, q));
        Assert.Equal(Perm.FromImages(new[] { 2, 3, 1 }), Structure.Commutator(p, q));
    }

    [Fact]
    public void Conjugate_and_commutator_of_disjoint_transpositions_are_unchanged_and_identity()
    {
        // P=(1,2), Q=(3,4); disjoint cycles commute.
        var p = Perm.FromImages(new[] { 2, 1, 3, 4 });
        var q = Perm.FromImages(new[] { 1, 2, 4, 3 });

        Assert.Equal(Perm.FromImages(new[] { 2, 1, 3, 4 }), Structure.Conjugate(p, q));
        Assert.Equal(Perm.FromImages(new[] { 1, 2, 3, 4 }), Structure.Commutator(p, q));
    }

    [Fact]
    public void Conjugate_and_commutator_match_the_second_asymmetric_pair()
    {
        // P=(1,2), Q=(1,2,3); this is an asymmetric noncommuting pair.
        var p = Perm.FromImages(new[] { 2, 1, 3 });
        var q = Perm.FromImages(new[] { 2, 3, 1 });

        Assert.Equal(Perm.FromImages(new[] { 1, 3, 2 }), Structure.Conjugate(p, q));
        Assert.Equal(Perm.FromImages(new[] { 3, 1, 2 }), Structure.Commutator(p, q));
    }

    [Fact]
    public void Conjugating_and_commuting_the_identity_with_a_cycle_are_identities()
    {
        // P=(), Q=(1,2,3).
        var p = Perm.FromImages(new[] { 1, 2, 3 });
        var q = Perm.FromImages(new[] { 2, 3, 1 });

        Assert.Equal(Perm.FromImages(new[] { 1, 2, 3 }), Structure.Conjugate(p, q));
        Assert.Equal(Perm.FromImages(new[] { 1, 2, 3 }), Structure.Commutator(p, q));
    }

    [Fact]
    public void Conjugating_a_cycle_by_the_identity_preserves_it_and_commutes()
    {
        // P=(1,2,3), Q=().
        var p = Perm.FromImages(new[] { 2, 3, 1 });
        var q = Perm.FromImages(new[] { 1, 2, 3 });

        Assert.Equal(Perm.FromImages(new[] { 2, 3, 1 }), Structure.Conjugate(p, q));
        Assert.Equal(Perm.FromImages(new[] { 1, 2, 3 }), Structure.Commutator(p, q));
    }

    [Fact]
    public void Numeric_structural_formatting_is_invariant_under_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "minus";
            CultureInfo.CurrentCulture = culture;

            var permutation = Perm.FromImages(new[] { 2, 1, 3 });

            var cycleType = Structure.CycleType(permutation);
            var oneLine = Structure.OneLine(permutation);

            Assert.Equal("2 1", cycleType);
            Assert.Equal("2 1 3", oneLine);
            AssertNoLineEnding(cycleType);
            AssertNoLineEnding(oneLine);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    private static void AssertNoLineEnding(string value)
    {
        Assert.DoesNotContain('\n', value);
        Assert.DoesNotContain('\r', value);
    }
}
