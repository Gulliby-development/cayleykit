namespace Cayleykit;

public static class WordSearch
{
    public static List<int>? Find(Perm target, IReadOnlyList<Perm> gens)
    {
        if (target.IsIdentity)
        {
            return new List<int>();
        }

        var generators = new List<(Perm Permutation, int Number)>();
        var distinctGenerators = new HashSet<Perm>();

        for (var index = 0; index < gens.Count; index++)
        {
            var generator = gens[index];

            if (!generator.IsIdentity && distinctGenerators.Add(generator))
            {
                generators.Add((generator, index + 1));
            }
        }

        var identity = Perm.Identity(target.Degree);
        var nodes = new List<SearchNode> { new(identity, -1, 0) };
        var discovered = new Dictionary<Perm, int> { [identity] = 0 };

        for (var currentIndex = 0; currentIndex < nodes.Count; currentIndex++)
        {
            var current = nodes[currentIndex].Permutation;

            foreach (var (generator, number) in generators)
            {
                var next = current.Then(generator);

                if (!discovered.TryAdd(next, nodes.Count))
                {
                    continue;
                }

                nodes.Add(new SearchNode(next, currentIndex, number));

                if (next.Equals(target))
                {
                    return Reconstruct(nodes, nodes.Count - 1);
                }
            }
        }

        return null;
    }

    private static List<int> Reconstruct(IReadOnlyList<SearchNode> nodes, int index)
    {
        var word = new List<int>();

        while (nodes[index].ParentIndex >= 0)
        {
            word.Add(nodes[index].GeneratorNumber);
            index = nodes[index].ParentIndex;
        }

        word.Reverse();
        return word;
    }

    private readonly record struct SearchNode(Perm Permutation, int ParentIndex, int GeneratorNumber);
}
