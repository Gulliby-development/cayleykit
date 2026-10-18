using System.Numerics;

namespace Cayleykit;

/// <summary>
/// Stores a deterministic strong-generating chain, with level i stabilizing all points below i.
/// </summary>
public sealed class Chain
{
    private sealed record StrongGenerator(Perm Permutation, int FirstMovedPoint);

    private sealed class Level
    {
        public required List<Perm> Generators { get; init; }

        public required Dictionary<int, Perm> Representatives { get; init; }

        public required Dictionary<int, Perm> InverseRepresentatives { get; init; }

        public required int[] OrbitPoints { get; init; }
    }

    private readonly int _degree;
    private readonly List<StrongGenerator> _strongGenerators = [];
    private readonly HashSet<Perm> _knownGenerators = [];
    private Level[] _levels = [];
    private IReadOnlyList<int> _base;
    private IReadOnlyList<int> _sizes;

    private Chain(int degree)
    {
        _degree = degree;
        _base = Array.AsReadOnly(Array.Empty<int>());
        _sizes = Array.AsReadOnly(Array.Empty<int>());
    }

    public BigInteger Order { get; private set; } = BigInteger.One;

    public IReadOnlyList<int> Base => _base;

    public IReadOnlyList<int> Sizes => _sizes;

    public static Chain Build(int degree, IReadOnlyList<Perm> gens)
    {
        var chain = new Chain(degree);

        foreach (var generator in gens)
        {
            chain.AddStrongGenerator(generator);
            chain.AddStrongGenerator(generator.Inverse());
        }

        chain.CloseSchreierRelations();
        chain.BuildTransversals();
        chain.SetOrderAndBase();

        return chain;
    }

    public bool Contains(Perm p)
    {
        if (p.Degree != _degree)
        {
            return false;
        }

        return Sift(p, 0) is null;
    }

    private void AddStrongGenerator(Perm generator)
    {
        if (generator.IsIdentity || !_knownGenerators.Add(generator))
        {
            return;
        }

        var firstMovedPoint = 1;

        while (firstMovedPoint <= _degree && generator.Image(firstMovedPoint) == firstMovedPoint)
        {
            firstMovedPoint++;
        }

        _strongGenerators.Add(new StrongGenerator(generator, firstMovedPoint));
    }

    private void CloseSchreierRelations()
    {
        while (true)
        {
            BuildTransversals();
            var residue = FindUnclosedSchreierRelation();

            if (residue is null)
            {
                return;
            }

            AddStrongGenerator(residue);
            var inverseResidue = Sift(residue.Inverse(), 0);

            if (inverseResidue is not null)
            {
                AddStrongGenerator(inverseResidue);
            }
        }
    }

    // With left-first Then composition, t[x].Then(g).Then(t[g(x)]^-1) fixes the current base point.
    private Perm? FindUnclosedSchreierRelation()
    {
        for (var levelIndex = 0; levelIndex < _degree; levelIndex++)
        {
            var level = _levels[levelIndex];

            foreach (var point in level.OrbitPoints)
            {
                var representative = level.Representatives[point];

                foreach (var generator in level.Generators)
                {
                    var image = generator.Image(point);
                    var schreier = representative
                        .Then(generator)
                        .Then(level.InverseRepresentatives[image]);
                    var residue = Sift(schreier, levelIndex + 1);

                    if (residue is not null)
                    {
                        return residue;
                    }
                }
            }
        }

        return null;
    }

    // Strip each base image by composing with the inverse representative for that image.
    private Perm? Sift(Perm permutation, int startLevel)
    {
        var residue = permutation;

        for (var levelIndex = startLevel; levelIndex < _degree; levelIndex++)
        {
            var point = levelIndex + 1;
            var image = residue.Image(point);
            var level = _levels[levelIndex];

            if (!level.Representatives.TryGetValue(image, out _))
            {
                return residue;
            }

            residue = residue.Then(level.InverseRepresentatives[image]);

            if (residue.IsIdentity)
            {
                return null;
            }
        }

        return residue.IsIdentity ? null : residue;
    }

    // At level i, each representative t[x] maps point i to x in the subgroup fixing all earlier points.
    private void BuildTransversals()
    {
        var levels = new Level[_degree];
        var identity = Perm.Identity(_degree);

        for (var levelIndex = 0; levelIndex < _degree; levelIndex++)
        {
            var basePoint = levelIndex + 1;
            var generators = _strongGenerators
                .Where(generator => generator.FirstMovedPoint >= basePoint)
                .Select(generator => generator.Permutation)
                .ToList();
            var representatives = new Dictionary<int, Perm> { [basePoint] = identity };
            var inverseRepresentatives = new Dictionary<int, Perm> { [basePoint] = identity };
            var queue = new Queue<int>();
            queue.Enqueue(basePoint);

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();
                var representative = representatives[point];

                foreach (var generator in generators)
                {
                    var image = generator.Image(point);

                    if (representatives.ContainsKey(image))
                    {
                        continue;
                    }

                    var nextRepresentative = representative.Then(generator);
                    representatives.Add(image, nextRepresentative);
                    inverseRepresentatives.Add(image, nextRepresentative.Inverse());
                    queue.Enqueue(image);
                }
            }

            levels[levelIndex] = new Level
            {
                Generators = generators,
                Representatives = representatives,
                InverseRepresentatives = inverseRepresentatives,
                OrbitPoints = representatives.Keys.Order().ToArray()
            };
        }

        _levels = levels;
    }

    private void SetOrderAndBase()
    {
        var basePoints = new List<int>();
        var sizes = new List<int>();
        var order = BigInteger.One;

        for (var levelIndex = 0; levelIndex < _degree; levelIndex++)
        {
            var size = _levels[levelIndex].OrbitPoints.Length;

            if (size > 1)
            {
                basePoints.Add(levelIndex + 1);
                sizes.Add(size);
                order *= size;
            }
        }

        Order = order;
        _base = Array.AsReadOnly(basePoints.ToArray());
        _sizes = Array.AsReadOnly(sizes.ToArray());
    }
}
