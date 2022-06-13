namespace Genjin.Core.Entities;

public readonly record struct Aspect(long AllBits = 0, long AnyBits = 0, long ExcludeBits = 0) {
    public bool IsInterested(long componentBits) {
        var matchesAll = MatchesAll(componentBits);
        var matchesAny = MatchesAny(componentBits);
        var matchesExclude = MatchesExclude(componentBits);
        return matchesAll && matchesAny && !matchesExclude;
    }

    public bool MatchesAll(long componentBits) => AllBits == 0 || (AllBits & componentBits) == AllBits;

    public bool MatchesAny(long componentBits) => AnyBits == 0 || (AnyBits & componentBits) != 0;

    public bool MatchesExclude(long componentBits) => ExcludeBits == 0 || (ExcludeBits & componentBits) == 0;
}
