namespace Genjin.Core.Entities;

public interface IEntityManager {
    Entity CreateEntity();
    IEnumerable<Entity> GetEntitiesMatchingAll(params Type[] types);
}
