using Veldrid;

namespace Genjin.Core;

public interface IKeyMap<T> where T : struct, Enum {
    T? this[Key key] { get; }
}
