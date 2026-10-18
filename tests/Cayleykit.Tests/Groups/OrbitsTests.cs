using System.Diagnostics;

namespace Cayleykit.Tests;

public class OrbitsTests
{
    [Fact]
    public void Identity_generator_leaves_the_only_point_as_a_singleton_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1 } },
            1,
            ParseGenerators(1, "()"));
    }

    [Fact]
    public void Empty_generators_leave_each_point_as_its_own_orbit()
    {
        AssertOrbits(
            new[]
            {
                new[] { 1 },
                new[] { 2 },
                new[] { 3 },
                new[] { 4 }
            },
            4,
            ParseGenerators(4));
    }

    [Fact]
    public void Disjoint_cycle_generators_produce_two_sorted_orbits()
    {
        AssertOrbits(
            new[]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5 }
            },
            5,
            ParseGenerators(5, "(1,2,3)", "(4,5)"));
    }

    [Fact]
    public void A_cycle_and_a_transposition_generate_one_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1, 2, 3 } },
            3,
            ParseGenerators(3, "(1,2,3)", "(1,2)"));
    }

    [Fact]
    public void Four_cycle_and_adjacent_transposition_generate_one_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1, 2, 3, 4 } },
            4,
            ParseGenerators(4, "(1,2,3,4)", "(1,2)"));
    }

    [Fact]
    public void Alternating_group_generators_have_one_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1, 2, 3, 4 } },
            4,
            ParseGenerators(4, "(1,2,3)", "(1,2)(3,4)"));
    }

    [Fact]
    public void Dihedral_group_generators_have_one_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1, 2, 3, 4 } },
            4,
            ParseGenerators(4, "(1,2,3,4)", "(2,4)"));
    }

    [Fact]
    public void Quaternion_group_generators_have_one_orbit()
    {
        AssertOrbits(
            new[] { new[] { 1, 2, 3, 4, 5, 6, 7, 8 } },
            8,
            ParseGenerators(8, "(1,3,2,4)(5,7,6,8)", "(1,5,2,6)(3,8,4,7)"));
    }

    [Fact]
    public void Repeated_identity_and_transposition_generators_keep_fixed_points_ordered()
    {
        AssertOrbits(
            new[]
            {
                new[] { 1 },
                new[] { 2, 4 },
                new[] { 3 },
                new[] { 5 },
                new[] { 6 }
            },
            6,
            ParseGenerators(6, "(2,4)", "()", "(2,4)"));
    }

    [Fact]
    public void Reordered_duplicate_and_identity_generators_preserve_the_literal_partition()
    {
        var expected = new[] { new[] { 1, 2, 3 } };

        AssertOrbits(expected, 3, ParseGenerators(3, "(1,2,3)", "(1,2)"));
        AssertOrbits(expected, 3, ParseGenerators(3, "(1,2)", "()", "(1,2,3)", "(1,2)"));
    }

    [Fact]
    public void Repeated_calls_return_the_same_literal_partition()
    {
        var generators = ParseGenerators(6, "(2,4)", "()", "(2,4)");
        var expected = new[]
        {
            new[] { 1 },
            new[] { 2, 4 },
            new[] { 3 },
            new[] { 5 },
            new[] { 6 }
        };

        AssertOrbits(expected, 6, generators);
        AssertOrbits(expected, 6, generators);
    }

    [Fact]
    public void Degree_sixty_four_with_eight_literal_generators_has_every_expected_orbit()
    {
        AssertOrbits(
            new[]
            {
                new[] { 1, 2 },
                new[] { 3 },
                new[] { 4 },
                new[] { 5 },
                new[] { 6 },
                new[] { 7 },
                new[] { 8 },
                new[] { 9 },
                new[] { 10 },
                new[] { 11 },
                new[] { 12 },
                new[] { 13 },
                new[] { 14 },
                new[] { 15 },
                new[] { 16 },
                new[] { 17 },
                new[] { 18 },
                new[] { 19 },
                new[] { 20 },
                new[] { 21 },
                new[] { 22 },
                new[] { 23 },
                new[] { 24 },
                new[] { 25 },
                new[] { 26 },
                new[] { 27 },
                new[] { 28 },
                new[] { 29 },
                new[] { 30 },
                new[] { 31 },
                new[] { 32 },
                new[] { 33 },
                new[] { 34 },
                new[] { 35 },
                new[] { 36 },
                new[] { 37 },
                new[] { 38 },
                new[] { 39 },
                new[] { 40 },
                new[] { 41 },
                new[] { 42 },
                new[] { 43 },
                new[] { 44 },
                new[] { 45 },
                new[] { 46 },
                new[] { 47 },
                new[] { 48 },
                new[] { 49 },
                new[] { 50 },
                new[] { 51 },
                new[] { 52 },
                new[] { 53 },
                new[] { 54 },
                new[] { 55 },
                new[] { 56 },
                new[] { 57 },
                new[] { 58 },
                new[] { 59 },
                new[] { 60 },
                new[] { 61 },
                new[] { 62 },
                new[] { 63 },
                new[] { 64 }
            },
            64,
            ParseGenerators(64, "(1,2)", "(1,2)", "()", "(1,2)", "()", "()", "()", "()"));
    }

    [Fact]
    public void Representative_degree_sixty_four_and_cube_orbit_workloads_finish_within_ten_seconds()
    {
        var degreeSixtyFourGenerators =
            ParseGenerators(64, "(1,2)", "(1,2)", "()", "(1,2)", "()", "()", "()", "()");
        var cubeGenerators = ParseGenerators(
            54,
            "(1,7,9,3)(2,4,8,6)(16,25,43,52)(17,26,44,53)(18,27,45,54)",
            "(3,52,30,21)(6,49,33,24)(9,46,36,27)(10,16,18,12)(11,13,17,15)",
            "(1,16,36,39)(2,13,35,42)(3,10,34,45)(19,25,27,21)(20,22,26,24)",
            "(10,46,37,19)(11,47,38,20)(12,48,39,21)(28,34,36,30)(29,31,35,33)",
            "(1,19,28,54)(4,22,31,51)(7,25,34,48)(37,43,45,39)(38,40,44,42)",
            "(7,37,30,18)(8,40,29,15)(9,43,28,12)(46,52,54,48)(47,49,53,51)");

        var stopwatch = Stopwatch.StartNew();

        _ = Orbits.Of(64, degreeSixtyFourGenerators);
        _ = Orbits.Of(54, cubeGenerators);

        stopwatch.Stop();

        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(10),
            $"Representative orbit workloads took {stopwatch.Elapsed}.");
    }

    private static List<Perm> ParseGenerators(int degree, params string[] texts)
    {
        return Notation.ParseAll(texts, degree).Perms;
    }

    private static void AssertOrbits(int[][] expected, int degree, IReadOnlyList<Perm> generators)
    {
        var actual = Orbits.Of(degree, generators);

        Assert.Equal(expected.Length, actual.Count);

        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index], actual[index]);
        }
    }
}
