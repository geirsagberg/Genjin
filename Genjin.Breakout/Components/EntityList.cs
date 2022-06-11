namespace Genjin.Breakout.Components;

public class EntityList : List<long> {
    public void Add(Entity entity) => Add(entity.Id);
}
