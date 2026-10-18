namespace Cayleykit;

public static class Closure
{
    public static List<Perm>? Elements(int degree, IReadOnlyList<Perm> gens, int limit)
    {
        if (limit < 1)
        {
            return null;
        }

        var identity = Perm.Identity(degree);
        var elements = new HashSet<Perm> { identity };
        var pending = new Queue<Perm>();
        pending.Enqueue(identity);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();

            foreach (var generator in gens)
            {
                var next = current.Then(generator);

                if (!elements.Add(next))
                {
                    continue;
                }

                if (elements.Count > limit)
                {
                    return null;
                }

                pending.Enqueue(next);
            }
        }

        var result = elements.ToList();
        result.Sort();
        return result;
    }
}
