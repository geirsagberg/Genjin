namespace Genjin.Breakout.Components;

public class World : IDrawable {
    private readonly Dictionary<long, long> componentBitsByEntity = new();
    private readonly Dictionary<Type, int> componentIdByType = new();
    private readonly Dictionary<Type, Dictionary<long, Object>> componentPoolsByType = new();
    private readonly Dictionary<long, Entity> entities = new();

    private readonly List<ISystem> systems = new();

    private int componentCount;
    private long entityCount;

    public void Draw() {
        foreach (var drawSystem in systems.OfType<IDrawSystem>()) {
            drawSystem.Draw();
        }
    }

    public Entity CreateEntity() {
        var entity = new Entity(++entityCount, this);
        entities[entity.Id] = entity;
        return entity;
    }

    private Dictionary<long, Object> GetOrCreateComponentPool(Type componentType) {
        if (!componentPoolsByType.TryGetValue(componentType, out var componentPool)) {
            var componentId = ++componentCount;
            if (componentId > 64) {
                throw new Exception("Too many components");
            }

            componentPool = new Dictionary<long, Object>();
            componentPoolsByType[componentType] = componentPool;
            componentIdByType[componentType] = componentId;
        }

        return componentPool;
    }

    public void AddComponent<T>(long entity, T component) where T : notnull {
        var componentPool = GetOrCreateComponentPool(typeof(T));
        componentPool[entity] = component;
        componentBitsByEntity[entity] = componentBitsByEntity.GetValueOrDefault(entity, 0) |
            (1L << (componentIdByType[typeof(T)] - 1));
    }

    public IEnumerable<Entity> GetEntitiesMatchingAll(params Type[] types) {
        var bits = types.Select(type => componentIdByType.GetValueOrDefault(type))
            .Aggregate(0L, (bits, id) => id == 0 ? bits : bits | (1L << (id - 1)));
        return componentBitsByEntity.Where(kvp => (kvp.Value & bits) == bits)
            .Select(kvp => entities[kvp.Key]);
    }

    public T GetComponent<T>(long entity) =>
        componentPoolsByType[typeof(T)][entity] is T
            ? (T) componentPoolsByType[typeof(T)][entity]
            : throw new Exception($"Entity {entity} does not have component of type {typeof(T).Name}");

    public void AddSystem(ISystem system) => systems.Add(system);
}
