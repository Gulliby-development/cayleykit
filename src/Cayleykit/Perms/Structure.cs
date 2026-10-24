using System.Numerics;

namespace Cayleykit;

public static class Structure
{
    public static string CycleType(Perm p)
    {
        return Text.Join(p.Cycles().Select(cycle => cycle.Count).OrderByDescending(length => length));
    }

    public static string OneLine(Perm p)
    {
        return Text.Join(p.Images);
    }

    public static string Sign(Perm p)
    {
        return p.IsEven ? "even" : "odd";
    }

    // Order of the centralizer of p in the symmetric group of p's degree: the product over the
    // cycle lengths k (fixed points are cycles of length 1) of k to the power m times m factorial,
    // where m is the number of cycles of length k.
    public static BigInteger CentralizerOrder(Perm p)
    {
        var order = BigInteger.One;

        foreach (var group in p.Cycles().GroupBy(cycle => cycle.Count))
        {
            for (var index = 1; index <= group.Count(); index++)
            {
                order *= group.Key * index;
            }
        }

        return order;
    }

    public static BigInteger ClassSize(Perm p)
    {
        var factorial = BigInteger.One;

        for (var value = 2; value <= p.Degree; value++)
        {
            factorial *= value;
        }

        return factorial / CentralizerOrder(p);
    }

    public static Perm Conjugate(Perm p, Perm q)
    {
        return q.Inverse().Then(p).Then(q);
    }

    public static Perm Commutator(Perm p, Perm q)
    {
        return p.Inverse().Then(q.Inverse()).Then(p).Then(q);
    }
}
