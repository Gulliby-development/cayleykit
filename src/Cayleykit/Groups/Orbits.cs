namespace Cayleykit;

public static class Orbits
{
    public static List<List<int>> Of(int degree, IReadOnlyList<Perm> gens)
    {
        var visited = new bool[degree + 1];
        var orbits = new List<List<int>>();

        for (var point = 1; point <= degree; point++)
        {
            if (visited[point])
            {
                continue;
            }

            var orbit = new List<int>();
            var queue = new Queue<int>();
            visited[point] = true;
            queue.Enqueue(point);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                orbit.Add(current);

                foreach (var generator in gens)
                {
                    var image = generator.Image(current);

                    if (!visited[image])
                    {
                        visited[image] = true;
                        queue.Enqueue(image);
                    }
                }
            }

            orbit.Sort();
            orbits.Add(orbit);
        }

        return orbits;
    }
}
