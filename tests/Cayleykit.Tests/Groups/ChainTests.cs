using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace Cayleykit.Tests;

public class ChainTests
{
    // AC-002: Assert literal values (group and generators | degree | order | base | sizes):
    // Identity () | 1 | 1 | [] | []; Empty generators | 4 | 1 | [] | [];
    // (2,4) | 6 | 2 | [2] | [2]; (1,2,3) | 3 | 3 | [1] | [3];
    // (1,2,3); (4,5) | 5 | 6 | [1,4] | [3,2];
    // S3: (1,2,3); (1,2) | 3 | 6 | [1,2] | [3,2];
    // S4: (1,2,3,4); (1,2) | 4 | 24 | [1,2,3] | [4,3,2];
    // A4: (1,2,3); (1,2)(3,4) | 4 | 12 | [1,2] | [4,3];
    // D8: (1,2,3,4); (2,4) | 4 | 8 | [1,2] | [4,2];
    // S5: (1,2,3,4,5); (1,2) | 5 | 120 | [1,2,3,4] | [5,4,3,2];
    // A5: (1,2,3,4,5); (1,2,3) | 5 | 60 | [1,2,3] | [5,4,3];
    // M11: (1,2,3,4,5,6,7,8,9,10,11); (3,7,11,8)(4,10,5,6) | 11 | 7920 | [1,2,3,4] | [11,10,9,8];
    // Pocket cube | 24 | 3674160 | [1,2,3,4,5,6] | [21,18,15,12,9,6];
    // 3x3x3 cube | 54 | 43252003274489856000 | [1,2,3,4,6,7,8,9,10,11,12,13,15,19,20,22,29,31] | [24,24,21,22,20,18,18,15,12,16,9,14,12,6,10,8,6,2].
    // AC-003: Assert booleans (group | candidate | result):
    // Empty degree 4 | () | true; Empty degree 4 | (1,2) | false;
    // C3 | (1,3,2) | true; C3 | (1,2) | false;
    // S5 | (1,2)(3,4,5) | true; A5 | (1,2)(3,4) | true; A5 | (1,2) | false;
    // M11 | generator | true; M11 | (1,2) | false;
    // Pocket cube | identity and each supplied generator | true; Pocket cube | (1,2) | false;
    // 3x3x3 cube | identity and each supplied generator | true; 3x3x3 cube | (1,2) | false.
    [Fact]
    public void Identity_generator_has_the_trivial_chain()
    {
        AssertChain(
            Chain.Build(1, ParseGenerators(1, "()")),
            "1",
            Array.Empty<int>(),
            Array.Empty<int>());
    }

    [Fact]
    public void Empty_generators_have_the_trivial_chain_at_degree_four()
    {
        AssertChain(
            Chain.Build(4, ParseGenerators(4)),
            "1",
            Array.Empty<int>(),
            Array.Empty<int>());
    }

    [Fact]
    public void A_transposition_with_fixed_points_has_one_nontrivial_level()
    {
        AssertChain(
            Chain.Build(6, ParseGenerators(6, "(2,4)")),
            "2",
            new[] { 2 },
            new[] { 2 });
    }

    [Fact]
    public void A_three_cycle_has_one_orbit_at_its_first_base_point()
    {
        AssertChain(
            Chain.Build(3, ParseGenerators(3, "(1,2,3)")),
            "3",
            new[] { 1 },
            new[] { 3 });
    }

    [Fact]
    public void Disjoint_cycles_have_their_two_literal_orbit_sizes()
    {
        AssertChain(
            Chain.Build(5, ParseGenerators(5, "(1,2,3)", "(4,5)")),
            "6",
            new[] { 1, 4 },
            new[] { 3, 2 });
    }

    [Fact]
    public void Symmetric_group_on_three_points_has_order_six()
    {
        AssertChain(
            Chain.Build(3, ParseGenerators(3, "(1,2,3)", "(1,2)")),
            "6",
            new[] { 1, 2 },
            new[] { 3, 2 });
    }

    [Fact]
    public void Symmetric_group_on_four_points_has_order_twenty_four()
    {
        AssertChain(
            Chain.Build(4, ParseGenerators(4, "(1,2,3,4)", "(1,2)")),
            "24",
            new[] { 1, 2, 3 },
            new[] { 4, 3, 2 });
    }

    [Fact]
    public void Alternating_group_on_four_points_has_order_twelve()
    {
        AssertChain(
            Chain.Build(4, ParseGenerators(4, "(1,2,3)", "(1,2)(3,4)")),
            "12",
            new[] { 1, 2 },
            new[] { 4, 3 });
    }

    [Fact]
    public void Dihedral_group_on_four_points_has_order_eight()
    {
        AssertChain(
            Chain.Build(4, ParseGenerators(4, "(1,2,3,4)", "(2,4)")),
            "8",
            new[] { 1, 2 },
            new[] { 4, 2 });
    }

    [Fact]
    public void Symmetric_group_on_five_points_has_order_one_hundred_twenty()
    {
        AssertChain(
            Chain.Build(5, ParseGenerators(5, "(1,2,3,4,5)", "(1,2)")),
            "120",
            new[] { 1, 2, 3, 4 },
            new[] { 5, 4, 3, 2 });
    }

    [Fact]
    public void Alternating_group_on_five_points_has_order_sixty()
    {
        AssertChain(
            Chain.Build(5, ParseGenerators(5, "(1,2,3,4,5)", "(1,2,3)")),
            "60",
            new[] { 1, 2, 3 },
            new[] { 5, 4, 3 });
    }

    [Fact]
    public void M11_has_its_literal_order_and_ascending_stabilizer_chain()
    {
        AssertChain(
            Chain.Build(
                11,
                ParseGenerators(
                    11,
                    "(1,2,3,4,5,6,7,8,9,10,11)",
                    "(3,7,11,8)(4,10,5,6)")),
            "7920",
            new[] { 1, 2, 3, 4 },
            new[] { 11, 10, 9, 8 });
    }

    [Fact]
    public void Pocket_cube_has_its_literal_order_and_ascending_stabilizer_chain()
    {
        AssertChain(
            Chain.Build(
                24,
                ParseGenerators(
                    24,
                    "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
                    "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
                    "(1,7,16,18)(2,5,15,20)(9,11,12,10)")),
            "3674160",
            new[] { 1, 2, 3, 4, 5, 6 },
            new[] { 21, 18, 15, 12, 9, 6 });
    }

    [Fact]
    public void Three_by_three_cube_has_its_literal_big_integer_order_and_chain()
    {
        AssertChain(
            Chain.Build(54, CubeGenerators()),
            "43252003274489856000",
            new[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 15, 19, 20, 22, 29, 31 },
            new[] { 24, 24, 21, 22, 20, 18, 18, 15, 12, 16, 9, 14, 12, 6, 10, 8, 6, 2 });
    }

    [Fact]
    public void Contains_accepts_identity_and_rejects_a_transposition_for_the_empty_group()
    {
        var chain = Chain.Build(4, ParseGenerators(4));

        AssertChain(chain, "1", Array.Empty<int>(), Array.Empty<int>());
        Assert.True(chain.Contains(ParsePermutation(4, "()")));
        Assert.False(chain.Contains(ParsePermutation(4, "(1,2)")));
        AssertChain(chain, "1", Array.Empty<int>(), Array.Empty<int>());
        Assert.True(chain.Contains(ParsePermutation(4, "()")));
    }

    [Fact]
    public void Contains_accepts_the_inverse_three_cycle_and_rejects_a_transposition()
    {
        var chain = Chain.Build(3, ParseGenerators(3, "(1,2,3)"));

        Assert.True(chain.Contains(ParsePermutation(3, "(1,3,2)")));
        Assert.False(chain.Contains(ParsePermutation(3, "(1,2)")));
        AssertChain(chain, "3", new[] { 1 }, new[] { 3 });
        Assert.True(chain.Contains(ParsePermutation(3, "(1,2,3)")));
    }

    [Fact]
    public void Contains_accepts_a_mixed_cycle_element_in_s5()
    {
        var chain = Chain.Build(5, ParseGenerators(5, "(1,2,3,4,5)", "(1,2)"));

        Assert.True(chain.Contains(ParsePermutation(5, "(1,2)(3,4,5)")));
        AssertChain(chain, "120", new[] { 1, 2, 3, 4 }, new[] { 5, 4, 3, 2 });
    }

    [Fact]
    public void Contains_distinguishes_even_and_odd_candidates_in_a5_without_mutating_the_chain()
    {
        var chain = Chain.Build(5, ParseGenerators(5, "(1,2,3,4,5)", "(1,2,3)"));

        AssertChain(chain, "60", new[] { 1, 2, 3 }, new[] { 5, 4, 3 });
        Assert.True(chain.Contains(ParsePermutation(5, "(1,2)(3,4)")));
        Assert.False(chain.Contains(ParsePermutation(5, "(1,2)")));
        AssertChain(chain, "60", new[] { 1, 2, 3 }, new[] { 5, 4, 3 });
        Assert.True(chain.Contains(ParsePermutation(5, "(1,2,3)")));
    }

    [Fact]
    public void Contains_accepts_an_m11_generator_and_rejects_a_transposition()
    {
        var chain = Chain.Build(
            11,
            ParseGenerators(11, "(1,2,3,4,5,6,7,8,9,10,11)", "(3,7,11,8)(4,10,5,6)"));

        AssertChain(chain, "7920", new[] { 1, 2, 3, 4 }, new[] { 11, 10, 9, 8 });
        Assert.True(chain.Contains(ParsePermutation(11, "(3,7,11,8)(4,10,5,6)")));
        Assert.False(chain.Contains(ParsePermutation(11, "(1,2)")));
        AssertChain(chain, "7920", new[] { 1, 2, 3, 4 }, new[] { 11, 10, 9, 8 });
        Assert.True(chain.Contains(ParsePermutation(11, "(1,2,3,4,5,6,7,8,9,10,11)")));
    }

    [Fact]
    public void Contains_accepts_each_pocket_cube_generator_and_identity_but_rejects_a_transposition()
    {
        var chain = Chain.Build(
            24,
            ParseGenerators(
                24,
                "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
                "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
                "(1,7,16,18)(2,5,15,20)(9,11,12,10)"));

        AssertChain(chain, "3674160", new[] { 1, 2, 3, 4, 5, 6 }, new[] { 21, 18, 15, 12, 9, 6 });
        Assert.True(chain.Contains(ParsePermutation(24, "()")));
        Assert.True(chain.Contains(ParsePermutation(24, "(2,23,14,10)(4,21,16,12)(5,7,8,6)")));
        Assert.True(chain.Contains(ParsePermutation(24, "(1,3,4,2)(7,11,19,23)(8,12,20,24)")));
        Assert.True(chain.Contains(ParsePermutation(24, "(1,7,16,18)(2,5,15,20)(9,11,12,10)")));
        Assert.False(chain.Contains(ParsePermutation(24, "(1,2)")));
        AssertChain(chain, "3674160", new[] { 1, 2, 3, 4, 5, 6 }, new[] { 21, 18, 15, 12, 9, 6 });
        Assert.True(chain.Contains(ParsePermutation(24, "()")));
    }

    [Fact]
    public void Contains_accepts_each_three_by_three_cube_generator_and_identity_but_rejects_a_transposition()
    {
        var chain = Chain.Build(54, CubeGenerators());

        AssertChain(
            chain,
            "43252003274489856000",
            new[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 15, 19, 20, 22, 29, 31 },
            new[] { 24, 24, 21, 22, 20, 18, 18, 15, 12, 16, 9, 14, 12, 6, 10, 8, 6, 2 });
        Assert.True(chain.Contains(ParsePermutation(54, "()")));
        Assert.True(chain.Contains(ParsePermutation(54, "(1,7,9,3)(2,4,8,6)(16,25,43,52)(17,26,44,53)(18,27,45,54)")));
        Assert.True(chain.Contains(ParsePermutation(54, "(3,52,30,21)(6,49,33,24)(9,46,36,27)(10,16,18,12)(11,13,17,15)")));
        Assert.True(chain.Contains(ParsePermutation(54, "(1,16,36,39)(2,13,35,42)(3,10,34,45)(19,25,27,21)(20,22,26,24)")));
        Assert.True(chain.Contains(ParsePermutation(54, "(10,46,37,19)(11,47,38,20)(12,48,39,21)(28,34,36,30)(29,31,35,33)")));
        Assert.True(chain.Contains(ParsePermutation(54, "(1,19,28,54)(4,22,31,51)(7,25,34,48)(37,43,45,39)(38,40,44,42)")));
        Assert.True(chain.Contains(ParsePermutation(54, "(7,37,30,18)(8,40,29,15)(9,43,28,12)(46,52,54,48)(47,49,53,51)")));
        Assert.False(chain.Contains(ParsePermutation(54, "(1,2)")));
        AssertChain(
            chain,
            "43252003274489856000",
            new[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 15, 19, 20, 22, 29, 31 },
            new[] { 24, 24, 21, 22, 20, 18, 18, 15, 12, 16, 9, 14, 12, 6, 10, 8, 6, 2 });
        Assert.True(chain.Contains(ParsePermutation(54, "()")));
    }

    [Fact]
    public void Degree_sixty_four_eight_generators_have_literal_chain_and_membership_results()
    {
        var chain = Chain.Build(
            64,
            ParseGenerators(64, "(1,2)", "(1,2)", "()", "(1,2)", "()", "()", "()", "()"));

        AssertChain(chain, "2", new[] { 1 }, new[] { 2 });
        Assert.True(chain.Contains(ParsePermutation(64, "()")));
        Assert.True(chain.Contains(ParsePermutation(64, "(1,2)")));
        Assert.False(chain.Contains(ParsePermutation(64, "(63,64)")));
        AssertChain(chain, "2", new[] { 1 }, new[] { 2 });
    }

    [Fact]
    public void Reordered_redundant_degree_sixty_four_generators_have_the_same_literal_chain()
    {
        var chain = Chain.Build(
            64,
            ParseGenerators(64, "()", "(1,2)", "()", "(1,2)", "()", "(1,2)", "()", "(1,2)"));

        AssertChain(chain, "2", new[] { 1 }, new[] { 2 });
        Assert.True(chain.Contains(ParsePermutation(64, "(1,2)")));
        Assert.False(chain.Contains(ParsePermutation(64, "(63,64)")));
    }

    [Fact]
    public void Reordered_and_redundant_s3_generators_have_the_same_literal_chain()
    {
        var chain = Chain.Build(3, ParseGenerators(3, "(1,2)", "()", "(1,2,3)", "(1,2)"));

        AssertChain(chain, "6", new[] { 1, 2 }, new[] { 3, 2 });
        Assert.True(chain.Contains(ParsePermutation(3, "(1,3,2)")));
        Assert.True(chain.Contains(ParsePermutation(3, "(1,3)")));
    }

    [Fact]
    public void Representative_degree_sixty_four_build_and_membership_workload_finishes_within_ten_seconds()
    {
        var generators = ParseGenerators(64, "(1,2)", "(1,2)", "()", "(1,2)", "()", "()", "()", "()");
        var candidate = ParsePermutation(64, "(1,2)");
        var stopwatch = Stopwatch.StartNew();

        var chain = Chain.Build(64, generators);
        var firstMembership = chain.Contains(candidate);
        var secondMembership = chain.Contains(Perm.Identity(64));

        stopwatch.Stop();

        Assert.True(firstMembership);
        Assert.True(secondMembership);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"Degree-64 chain workload took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Representative_pocket_cube_build_and_repeated_membership_workload_finishes_within_ten_seconds()
    {
        var generators = ParseGenerators(
            24,
            "(2,23,14,10)(4,21,16,12)(5,7,8,6)",
            "(1,3,4,2)(7,11,19,23)(8,12,20,24)",
            "(1,7,16,18)(2,5,15,20)(9,11,12,10)");
        var firstGenerator = ParsePermutation(24, "(2,23,14,10)(4,21,16,12)(5,7,8,6)");
        var secondGenerator = ParsePermutation(24, "(1,3,4,2)(7,11,19,23)(8,12,20,24)");
        var thirdGenerator = ParsePermutation(24, "(1,7,16,18)(2,5,15,20)(9,11,12,10)");
        var identity = Perm.Identity(24);
        var stopwatch = Stopwatch.StartNew();

        var firstChain = Chain.Build(24, generators);
        var firstIdentityAnswer = firstChain.Contains(identity);
        var firstGeneratorAnswer = firstChain.Contains(firstGenerator);
        var secondGeneratorAnswer = firstChain.Contains(secondGenerator);
        var thirdGeneratorAnswer = firstChain.Contains(thirdGenerator);
        var secondChain = Chain.Build(24, generators);
        var repeatedAnswer = secondChain.Contains(firstGenerator);

        stopwatch.Stop();

        Assert.Equal(new BigInteger(3674160), firstChain.Order);
        Assert.Equal(firstChain.Order, secondChain.Order);
        Assert.True(firstIdentityAnswer);
        Assert.True(firstGeneratorAnswer);
        Assert.True(secondGeneratorAnswer);
        Assert.True(thirdGeneratorAnswer);
        Assert.True(repeatedAnswer);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"Pocket-cube workload took {stopwatch.Elapsed}.");
    }

    [Fact]
    public void Representative_three_by_three_cube_build_and_repeated_membership_workload_finishes_within_ten_seconds()
    {
        var generators = CubeGenerators();
        var firstGenerator = ParsePermutation(54, "(1,7,9,3)(2,4,8,6)(16,25,43,52)(17,26,44,53)(18,27,45,54)");
        var secondGenerator = ParsePermutation(54, "(3,52,30,21)(6,49,33,24)(9,46,36,27)(10,16,18,12)(11,13,17,15)");
        var thirdGenerator = ParsePermutation(54, "(1,16,36,39)(2,13,35,42)(3,10,34,45)(19,25,27,21)(20,22,26,24)");
        var fourthGenerator = ParsePermutation(54, "(10,46,37,19)(11,47,38,20)(12,48,39,21)(28,34,36,30)(29,31,35,33)");
        var fifthGenerator = ParsePermutation(54, "(1,19,28,54)(4,22,31,51)(7,25,34,48)(37,43,45,39)(38,40,44,42)");
        var sixthGenerator = ParsePermutation(54, "(7,37,30,18)(8,40,29,15)(9,43,28,12)(46,52,54,48)(47,49,53,51)");
        var identity = Perm.Identity(54);
        var stopwatch = Stopwatch.StartNew();

        var firstChain = Chain.Build(54, generators);
        var identityAnswer = firstChain.Contains(identity);
        var firstAnswer = firstChain.Contains(firstGenerator);
        var secondAnswer = firstChain.Contains(secondGenerator);
        var thirdAnswer = firstChain.Contains(thirdGenerator);
        var fourthAnswer = firstChain.Contains(fourthGenerator);
        var fifthAnswer = firstChain.Contains(fifthGenerator);
        var sixthAnswer = firstChain.Contains(sixthGenerator);
        var outsideAnswer = firstChain.Contains(ParsePermutation(54, "(1,2)"));
        var secondChain = Chain.Build(54, generators);
        var repeatedAnswer = secondChain.Contains(sixthGenerator);

        stopwatch.Stop();

        Assert.Equal(BigInteger.Parse("43252003274489856000", CultureInfo.InvariantCulture), firstChain.Order);
        Assert.Equal(firstChain.Order, secondChain.Order);
        Assert.True(identityAnswer);
        Assert.True(firstAnswer);
        Assert.True(secondAnswer);
        Assert.True(thirdAnswer);
        Assert.True(fourthAnswer);
        Assert.True(fifthAnswer);
        Assert.True(sixthAnswer);
        Assert.False(outsideAnswer);
        Assert.True(repeatedAnswer);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(10), $"3x3x3-cube workload took {stopwatch.Elapsed}.");
    }

    private static void AssertChain(Chain chain, string expectedOrder, int[] expectedBase, int[] expectedSizes)
    {
        Assert.Equal(BigInteger.Parse(expectedOrder, CultureInfo.InvariantCulture), chain.Order);
        Assert.Equal(expectedBase, chain.Base);
        Assert.Equal(expectedSizes, chain.Sizes);
    }

    private static List<Perm> ParseGenerators(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static Perm ParsePermutation(int degree, string text)
    {
        return Notation.ParseAll(new[] { text }, degree).Perms[0];
    }

    private static List<Perm> CubeGenerators()
    {
        return ParseGenerators(
            54,
            "(1,7,9,3)(2,4,8,6)(16,25,43,52)(17,26,44,53)(18,27,45,54)",
            "(3,52,30,21)(6,49,33,24)(9,46,36,27)(10,16,18,12)(11,13,17,15)",
            "(1,16,36,39)(2,13,35,42)(3,10,34,45)(19,25,27,21)(20,22,26,24)",
            "(10,46,37,19)(11,47,38,20)(12,48,39,21)(28,34,36,30)(29,31,35,33)",
            "(1,19,28,54)(4,22,31,51)(7,25,34,48)(37,43,45,39)(38,40,44,42)",
            "(7,37,30,18)(8,40,29,15)(9,43,28,12)(46,52,54,48)(47,49,53,51)");
    }
}
