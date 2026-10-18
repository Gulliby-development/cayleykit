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

    public static Perm Conjugate(Perm p, Perm q)
    {
        return q.Inverse().Then(p).Then(q);
    }

    public static Perm Commutator(Perm p, Perm q)
    {
        return p.Inverse().Then(q.Inverse()).Then(p).Then(q);
    }
}
