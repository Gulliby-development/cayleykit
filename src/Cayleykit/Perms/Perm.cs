namespace Cayleykit;

public sealed class Perm : IComparable<Perm>, IEquatable<Perm>
{
    private readonly int[] _images;
    private readonly IReadOnlyList<int> _readOnlyImages;
    private readonly IReadOnlyList<IReadOnlyList<int>> _cycles;

    private Perm(int[] images)
    {
        _images = images;
        _readOnlyImages = Array.AsReadOnly(_images);
        _cycles = BuildCycles();
    }

    public int Degree => _images.Length;

    public IReadOnlyList<int> Images => _readOnlyImages;

    public bool IsIdentity => _images.Select((image, index) => image == index + 1).All(isFixed => isFixed);

    public bool IsEven => (Degree - _cycles.Count) % 2 == 0;

    public static Perm Identity(int degree)
    {
        var images = new int[degree];

        for (var index = 0; index < degree; index++)
        {
            images[index] = index + 1;
        }

        return new Perm(images);
    }

    public static Perm FromImages(IReadOnlyList<int> images)
    {
        var copy = new int[images.Count];

        for (var index = 0; index < images.Count; index++)
        {
            copy[index] = images[index];
        }

        return new Perm(copy);
    }

    public int Image(int point)
    {
        return _images[point - 1];
    }

    public Perm Then(Perm next)
    {
        var images = new int[Degree];

        for (var index = 0; index < Degree; index++)
        {
            images[index] = next._images[_images[index] - 1];
        }

        return new Perm(images);
    }

    public Perm Inverse()
    {
        var images = new int[Degree];

        for (var index = 0; index < Degree; index++)
        {
            images[_images[index] - 1] = index + 1;
        }

        return new Perm(images);
    }

    public Perm Power(long k)
    {
        var order = Order();
        var exponent = k % order;

        if (exponent < 0)
        {
            exponent += order;
        }

        var images = new int[Degree];

        foreach (var cycle in _cycles)
        {
            var shift = (int)(exponent % cycle.Count);

            for (var index = 0; index < cycle.Count; index++)
            {
                images[cycle[index] - 1] = cycle[(index + shift) % cycle.Count];
            }
        }

        return new Perm(images);
    }

    public long Order()
    {
        var order = 1L;

        foreach (var cycle in _cycles)
        {
            order = order / GreatestCommonDivisor(order, cycle.Count) * cycle.Count;
        }

        return order;
    }

    public IReadOnlyList<IReadOnlyList<int>> Cycles()
    {
        return _cycles;
    }

    public int CompareTo(Perm? other)
    {
        if (other is null)
        {
            return 1;
        }

        var sharedLength = Math.Min(Degree, other.Degree);

        for (var index = 0; index < sharedLength; index++)
        {
            var comparison = _images[index].CompareTo(other._images[index]);

            if (comparison != 0)
            {
                return comparison;
            }
        }

        return Degree.CompareTo(other.Degree);
    }

    public bool Equals(Perm? other)
    {
        if (other is null || Degree != other.Degree)
        {
            return false;
        }

        for (var index = 0; index < Degree; index++)
        {
            if (_images[index] != other._images[index])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Perm other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;

            foreach (var image in _images)
            {
                hash = (hash * 31) + image;
            }

            return hash;
        }
    }

    private IReadOnlyList<IReadOnlyList<int>> BuildCycles()
    {
        var visited = new bool[Degree];
        var cycles = new List<IReadOnlyList<int>>();

        for (var point = 1; point <= Degree; point++)
        {
            if (visited[point - 1])
            {
                continue;
            }

            var cycle = new List<int>();
            var current = point;

            while (!visited[current - 1])
            {
                visited[current - 1] = true;
                cycle.Add(current);
                current = _images[current - 1];
            }

            cycles.Add(cycle.AsReadOnly());
        }

        return cycles.AsReadOnly();
    }

    private static long GreatestCommonDivisor(long left, long right)
    {
        while (right != 0)
        {
            var remainder = left % right;
            left = right;
            right = remainder;
        }

        return left;
    }
}
