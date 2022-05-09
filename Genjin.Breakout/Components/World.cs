using Genjin.Core.Extensions;

namespace Genjin.Breakout.Components;

public class World : IDrawable {
    private readonly Dictionary<long, long> componentBitsByEntity = new();
    private readonly Dictionary<Type, int> componentIdsByType = new();
    private readonly Dictionary<Type, Dictionary<long, Object>> componentsByEntityByType = new();
    private readonly Dictionary<long, Entity> entitiesById = new();
    private readonly Dictionary<Aspect, HashSet<Entity>> entitiesByAspect = new();

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
        entitiesById[entity.Id] = entity;
        return entity;
    }

    private Dictionary<long, Object> GetOrCreateComponentPool(Type componentType) =>
        componentsByEntityByType.GetOrCreate(componentType, () => {
            var componentId = ++componentCount;
            if (componentId > 64) {
                throw new Exception("Too many components");
            }

            var componentPool = new Dictionary<long, Object>();
            componentsByEntityByType[componentType] = componentPool;
            componentIdsByType[componentType] = componentId;
            return componentPool;
        });

    public void AddComponent<T>(long entity, T component) where T : notnull {
        var componentPool = GetOrCreateComponentPool(typeof(T));
        componentPool[entity] = component;
        var componentBits = componentBitsByEntity.GetOrCreate(entity, 0) |
            (1L << (componentIdsByType[typeof(T)] - 1));
        componentBitsByEntity[entity] = componentBits;
        foreach (var (aspect, entities) in entitiesByAspect) {
            if (aspect.MatchesExclude(componentBits)) {
                entities.Remove(entitiesById[entity]);
            } else if (aspect.MatchesAll(componentBits) || aspect.MatchesAny(componentBits)) {
                entities.Add(entitiesById[entity]);
            }
        }
    }

    public IEnumerable<Entity> GetEntitiesMatchingAll(params Type[] types) {
        var componentBits = GetComponentBits(types);
        return entitiesByAspect.GetOrCreate(new Aspect(AllBits: componentBits), () => new HashSet<Entity>());
    }

    private long GetComponentBits(Type[] types) =>
        types.Select(type => componentIdsByType.GetValueOrDefault(type))
            .Aggregate(0L, (bits, id) => id == 0 ? bits : bits | (1L << (id - 1)));

    public T GetComponent<T>(long entity) =>
        componentsByEntityByType[typeof(T)][entity] is T
            ? (T) componentsByEntityByType[typeof(T)][entity]
            : throw new Exception($"Entity {entity} does not have component of type {typeof(T).Name}");

    public void AddSystem(ISystem system) => systems.Add(system);
}