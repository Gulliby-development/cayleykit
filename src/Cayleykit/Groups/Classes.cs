namespace Cayleykit;

public record ConjClass(Perm Representative, int Size);

public static class Classes
{
    public static List<ConjClass> Of(IReadOnlyList<Perm> elements, IReadOnlyList<Perm> gens)
    {
        var visited = new HashSet<Perm>();
        var classes = new List<ConjClass>();

        foreach (var element in elements)
        {
            if (!visited.Add(element))
            {
                continue;
            }

            var orbit = new List<Perm> { element };

            for (var currentIndex = 0; currentIndex < orbit.Count; currentIndex++)
            {
                var current = orbit[currentIndex];

                foreach (var generator in gens)
                {
                    var conjugate = Structure.Conjugate(current, generator);

                    if (visited.Add(conjugate))
                    {
                        orbit.Add(conjugate);
                    }
                }
            }

            classes.Add(new ConjClass(element, orbit.Count));
        }

        return classes;
    }
}
