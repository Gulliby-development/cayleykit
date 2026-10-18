namespace Cayleykit.Tests;

public class PermTests
{
    [Theory]
    [InlineData(new[] { 2, 3, 1 }, 1, 2)]
    [InlineData(new[] { 2, 3, 1 }, 2, 3)]
    [InlineData(new[] { 2, 3, 1 }, 3, 1)]
    [InlineData(new[] { 1, 3, 2 }, 2, 3)]
    [InlineData(new[] { 2, 1, 4, 3 }, 4, 3)]
    [InlineData(new[] { 1 }, 1, 1)]
    public void FromImages_sets_the_degree_images_and_point_accessors(int[] images, int point, int expectedImage)
    {
        var permutation = Perm.FromImages(images);

        Assert.Equal(images.Length, permutation.Degree);
        Assert.Equal(expectedImage, permutation.Image(point));
        Assert.Equal(images, permutation.Images);
    }

    [Fact]
    public void Identity_has_literal_images_for_degree_one()
    {
        var permutation = Perm.Identity(1);

        Assert.Equal(1, permutation.Degree);
        Assert.Equal(new[] { 1 }, permutation.Images);
        Assert.True(permutation.IsIdentity);
    }

    [Fact]
    public void Identity_has_literal_images_for_degree_three()
    {
        var permutation = Perm.Identity(3);

        Assert.Equal(3, permutation.Degree);
        Assert.Equal(new[] { 1, 2, 3 }, permutation.Images);
        Assert.True(permutation.IsIdentity);
    }

    [Fact]
    public void Identity_has_literal_images_for_degree_sixty_four()
    {
        var permutation = Perm.Identity(64);

        Assert.Equal(64, permutation.Degree);
        Assert.Equal(new[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32,
            33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64
        }, permutation.Images);
        Assert.True(permutation.IsIdentity);
    }

    [Fact]
    public void FromImages_defensively_copies_the_callers_image_array()
    {
        var input = new[] { 2, 3, 1 };
        var permutation = Perm.FromImages(input);

        input[0] = 1;
        input[1] = 2;
        input[2] = 3;

        Assert.Equal(new[] { 2, 3, 1 }, permutation.Images);
        Assert.Equal(3, permutation.Image(2));
    }

    [Fact]
    public void Images_cannot_be_mutated_through_a_list_interface()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1 });
        var images = Assert.IsAssignableFrom<IList<int>>(permutation.Images);

        Assert.Throws<NotSupportedException>(() => images[0] = 1);
        Assert.Throws<NotSupportedException>(() => images.Add(4));
        Assert.Equal(new[] { 2, 3, 1 }, permutation.Images);
    }

    [Fact]
    public void Cycles_and_their_nested_lists_cannot_be_mutated()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1 });
        var cycles = Assert.IsAssignableFrom<IList<IReadOnlyList<int>>>(permutation.Cycles());
        var cycle = Assert.IsAssignableFrom<IList<int>>(cycles[0]);

        Assert.Throws<NotSupportedException>(() => cycles[0] = new[] { 1, 2, 3 });
        Assert.Throws<NotSupportedException>(() => cycles.Add(new[] { 4 }));
        Assert.Throws<NotSupportedException>(() => cycle[0] = 3);
        Assert.Throws<NotSupportedException>(() => cycle.Add(4));
        Assert.Equal(new[] { 1, 2, 3 }, permutation.Cycles()[0]);
    }

    [Fact]
    public void Separately_constructed_permutations_have_equal_values_and_hashes()
    {
        var left = Perm.FromImages(new[] { 2, 3, 1 });
        var right = Perm.FromImages(new[] { 2, 3, 1 });

        Assert.True(left.Equals(right));
        Assert.True(left.Equals((object)right));
        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
        Assert.Equal(0, left.CompareTo(right));
    }

    [Fact]
    public void Different_images_are_not_equal_as_typed_or_object_values()
    {
        var left = Perm.FromImages(new[] { 2, 3, 1 });
        var right = Perm.FromImages(new[] { 2, 1, 3 });

        Assert.False(left.Equals(right));
        Assert.False(left.Equals((object)right));
        Assert.NotEqual(left, right);
    }

    [Fact]
    public void Permutation_is_not_equal_to_null()
    {
        var permutation = Perm.FromImages(new[] { 1 });

        Assert.False(permutation.Equals((Perm?)null));
        Assert.False(permutation.Equals((object?)null));
    }

    [Fact]
    public void CompareTo_uses_numeric_lexicographic_order_at_the_first_different_image()
    {
        var left = Perm.FromImages(new[] { 1, 2, 3 });
        var right = Perm.FromImages(new[] { 1, 3, 2 });

        Assert.True(left.CompareTo(right) < 0);
        Assert.True(right.CompareTo(left) > 0);
    }

    [Fact]
    public void CompareTo_continues_numeric_lexicographic_order_after_a_shared_prefix()
    {
        var left = Perm.FromImages(new[] { 2, 3, 1 });
        var right = Perm.FromImages(new[] { 2, 1, 3 });

        Assert.True(left.CompareTo(right) > 0);
        Assert.True(right.CompareTo(left) < 0);
    }

    [Fact]
    public void CompareTo_places_a_non_null_permutation_after_null()
    {
        var permutation = Perm.FromImages(new[] { 1 });

        Assert.True(permutation.CompareTo(null) > 0);
    }

    [Fact]
    public void Then_applies_the_left_permutation_first_and_the_right_permutation_second()
    {
        var p = Perm.FromImages(new[] { 2, 3, 1 });
        var q = Perm.FromImages(new[] { 2, 1, 3 });

        Assert.Equal(new[] { 1, 3, 2 }, p.Then(q).Images);
        Assert.Equal(new[] { 3, 2, 1 }, q.Then(p).Images);
    }

    [Fact]
    public void Inverse_reverses_a_three_cycle()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1 });

        Assert.Equal(new[] { 3, 1, 2 }, permutation.Inverse().Images);
    }

    [Fact]
    public void Inverse_of_a_transposition_is_the_same_permutation()
    {
        var permutation = Perm.FromImages(new[] { 2, 1, 3 });

        Assert.Equal(new[] { 2, 1, 3 }, permutation.Inverse().Images);
    }

    [Fact]
    public void A_permutation_composed_with_its_inverse_is_identity()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1 });

        Assert.Equal(new[] { 1, 2, 3 }, permutation.Then(permutation.Inverse()).Images);
        Assert.Equal(new[] { 1, 2, 3 }, permutation.Inverse().Then(permutation).Images);
    }

    [Fact]
    public void Identity_composition_preserves_a_permutation_in_both_orders()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1 });
        var identity = Perm.Identity(3);

        Assert.Equal(new[] { 2, 3, 1 }, identity.Then(permutation).Images);
        Assert.Equal(new[] { 2, 3, 1 }, permutation.Then(identity).Images);
    }

    [Fact]
    public void Inverse_of_identity_is_identity()
    {
        var identity = Perm.Identity(4);

        Assert.Equal(new[] { 1, 2, 3, 4 }, identity.Inverse().Images);
    }

    [Fact]
    public void Cycles_include_fixed_points_and_follow_the_image_map_in_canonical_order()
    {
        var permutation = Perm.FromImages(new[] { 3, 2, 1, 5, 4, 6 });
        var cycles = permutation.Cycles();

        Assert.Equal(4, cycles.Count);
        Assert.Equal(new[] { 1, 3 }, cycles[0]);
        Assert.Equal(new[] { 2 }, cycles[1]);
        Assert.Equal(new[] { 4, 5 }, cycles[2]);
        Assert.Equal(new[] { 6 }, cycles[3]);
    }

    [Fact]
    public void Cycles_retain_fixed_points_after_nontrivial_cycles()
    {
        var permutation = Perm.FromImages(new[] { 2, 3, 1, 5, 4, 6 });
        var cycles = permutation.Cycles();

        Assert.Equal(3, cycles.Count);
        Assert.Equal(new[] { 1, 2, 3 }, cycles[0]);
        Assert.Equal(new[] { 4, 5 }, cycles[1]);
        Assert.Equal(new[] { 6 }, cycles[2]);
    }

    [Fact]
    public void Degree_sixty_four_transposition_cycles_include_every_fixed_point()
    {
        var permutation = Perm.FromImages(new[]
        {
            64, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32,
            33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 1
        });
        var cycles = permutation.Cycles();

        Assert.Equal(new[]
        {
            new[] { 1, 64 }, new[] { 2 }, new[] { 3 }, new[] { 4 }, new[] { 5 }, new[] { 6 }, new[] { 7 },
            new[] { 8 }, new[] { 9 }, new[] { 10 }, new[] { 11 }, new[] { 12 }, new[] { 13 }, new[] { 14 },
            new[] { 15 }, new[] { 16 }, new[] { 17 }, new[] { 18 }, new[] { 19 }, new[] { 20 }, new[] { 21 },
            new[] { 22 }, new[] { 23 }, new[] { 24 }, new[] { 25 }, new[] { 26 }, new[] { 27 }, new[] { 28 },
            new[] { 29 }, new[] { 30 }, new[] { 31 }, new[] { 32 }, new[] { 33 }, new[] { 34 }, new[] { 35 },
            new[] { 36 }, new[] { 37 }, new[] { 38 }, new[] { 39 }, new[] { 40 }, new[] { 41 }, new[] { 42 },
            new[] { 43 }, new[] { 44 }, new[] { 45 }, new[] { 46 }, new[] { 47 }, new[] { 48 }, new[] { 49 },
            new[] { 50 }, new[] { 51 }, new[] { 52 }, new[] { 53 }, new[] { 54 }, new[] { 55 }, new[] { 56 },
            new[] { 57 }, new[] { 58 }, new[] { 59 }, new[] { 60 }, new[] { 61 }, new[] { 62 }, new[] { 63 }
        }, cycles.Select(cycle => cycle.ToArray()));
    }

    [Theory]
    [InlineData(new[] { 1 }, 1)]
    [InlineData(new[] { 1, 2, 3 }, 1)]
    [InlineData(new[] { 2, 1, 3 }, 2)]
    [InlineData(new[] { 2, 3, 1 }, 3)]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, 6)]
    [InlineData(new[] { 2, 3, 1, 5, 6, 7, 4 }, 12)]
    [InlineData(new[] { 64, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 1 }, 2)]
    public void Order_is_the_exact_lcm_of_the_cycle_lengths(int[] images, long expectedOrder)
    {
        Assert.Equal(expectedOrder, Perm.FromImages(images).Order());
    }

    [Theory]
    [InlineData(new[] { 1 }, true, true)]
    [InlineData(new[] { 1, 2, 3 }, true, true)]
    [InlineData(new[] { 2, 1, 3 }, false, false)]
    [InlineData(new[] { 2, 3, 1 }, false, true)]
    [InlineData(new[] { 2, 3, 1, 5, 4, 6 }, false, false)]
    [InlineData(new[] { 2, 3, 1, 5, 6, 7, 4 }, false, false)]
    [InlineData(new[] { 2, 1 }, false, false)]
    public void Identity_and_parity_match_the_cycle_structure(int[] images, bool isIdentity, bool isEven)
    {
        var permutation = Perm.FromImages(images);

        Assert.Equal(isIdentity, permutation.IsIdentity);
        Assert.Equal(isEven, permutation.IsEven);
    }

    [Theory]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, 0, new[] { 1, 2, 3, 4, 5 })]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, 1, new[] { 2, 3, 1, 5, 4 })]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, -1, new[] { 3, 1, 2, 5, 4 })]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, -7, new[] { 3, 1, 2, 5, 4 })]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, long.MinValue, new[] { 2, 3, 1, 4, 5 })]
    [InlineData(new[] { 2, 3, 1, 5, 4 }, long.MaxValue, new[] { 2, 3, 1, 5, 4 })]
    [InlineData(new[] { 2, 3, 4, 1 }, 1000000000000000000, new[] { 1, 2, 3, 4 })]
    [InlineData(new[] { 1, 2, 3 }, long.MinValue, new[] { 1, 2, 3 })]
    public void Power_reduces_all_exponents_by_the_exact_order(
        int[] images,
        long exponent,
        int[] expectedImages)
    {
        Assert.Equal(expectedImages, Perm.FromImages(images).Power(exponent).Images);
    }
}
