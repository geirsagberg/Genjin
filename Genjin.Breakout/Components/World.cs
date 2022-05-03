namespace Genjin.Breakout.Components;

public class World : IDrawable {
    public void Draw() {
        foreach (var drawSystem in systems.OfType<IDrawSystem>()) {
            drawSystem.Draw();
        }
    }

    private readonly Dictionary<Type, Dictionary<long, Object>> componentPoolsByType = new();

    // private readonly Dictionary<long, BitVector32> componentBitsByEntity = new();

    private readonly Dictionary<long, Entity> entities = new();

    private readonly List<ISystem> systems = new();

    private long entityCount;

    public Entity CreateEntity() {
        var entity = new Entity(++entityCount, this);
        entities[entity.Id] = entity;
        return entity;
    }

    private Dictionary<long, Object> GetOrCreateComponentPool(Type componentType) {
        if (!componentPoolsByType.TryGetValue(componentType, out var componentPool)) {
            componentPool = new Dictionary<long, Object>();
            componentPoolsByType[componentType] = componentPool;
        }

        return componentPool;
    }

    public void AddComponent<T>(long entity, T component) where T : notnull {
        var componentPool = GetOrCreateComponentPool(typeof(T));
        componentPool[entity] = component;
    }

    public IEnumerable<Entity> GetEntitiesMatchingAll(params Type[] types) =>
        types.Select(GetOrCreateComponentPool)
            .Select(p => p.Keys.AsEnumerable()).Aggregate((a, b) => a.Intersect(b))
            .Select(id => entities[id]);

    public T GetComponent<T>(long entity) =>
        componentPoolsByType[typeof(T)][entity] is T
            ? (T) componentPoolsByType[typeof(T)][entity]
            : throw new Exception($"Entity {entity} does not have component of type {typeof(T).Name}");

    public void AddSystem(ISystem system) => systems.Add(system);
}
