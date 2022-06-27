using Genjin.Core.Extensions;

namespace Genjin.Core.Entities;

public class World : IDrawable, IEntityManager {
    private readonly Action<IUpdatable> addUpdatable;

    public World(Action<IUpdatable> addUpdatable) {
        this.addUpdatable = addUpdatable;
    }

    // Bitmask of active components per entity ID
    private readonly Dictionary<long, long> componentBitsByEntity = new();

    // Mapping from component type to component ID
    private readonly Dictionary<Type, int> componentIdsByType = new();

    // Actual storage of components per entity
    private readonly Dictionary<Type, Dictionary<long, Object>> componentsByEntityByType = new();

    // Mapping from entity ID to entity object
    private readonly Dictionary<long, Entity> entitiesById = new();

    // Entities cached by aspect. Kept updated when components are added or removed.
    private readonly Dictionary<Aspect, HashSet<Entity>> entitiesByAspect = new();

    private readonly List<ISystem> systems = new();

    private int componentCount;
    private long entityCount;
    private readonly List<Simulation> simulations = new();

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
            var componentPool = new Dictionary<long, Object>();
            componentsByEntityByType[componentType] = componentPool;
            return componentPool;
        });

    private int GetOrCreateComponentId(Type componentType) =>
        componentIdsByType.GetOrCreate(componentType, () => {
            var componentId = ++componentCount;
            if (componentId > 64) {
                throw new Exception("Too many components");
            }

            componentIdsByType[componentType] = componentId;
            return componentId;
        });

    public void AddComponent<T>(long entity, T component) where T : notnull {
        var componentId = GetOrCreateComponentId(typeof(T));
        var componentPool = GetOrCreateComponentPool(typeof(T));
        componentPool[entity] = component;
        var componentBits = componentBitsByEntity.GetOrCreate(entity, 0) |
            (1L << (componentId - 1));
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
        var aspect = new Aspect(componentBits);
        return entitiesByAspect.GetOrCreate(aspect, () => GetEntitiesMatching(aspect));
    }

    private HashSet<Entity> GetEntitiesMatching(Aspect aspect) =>
        componentBitsByEntity
            .Where(pair => aspect.MatchesAll(pair.Value))
            .Select(pair => entitiesById[pair.Key])
            .ToHashSet();

    private long GetComponentBits(IEnumerable<Type> types) =>
        types.Select(GetOrCreateComponentId)
            .Aggregate(0L, (bits, id) => id == 0 ? bits : bits | (1L << (id - 1)));

    public T GetComponent<T>(long entity) =>
        componentsByEntityByType[typeof(T)][entity] is T
            ? (T) componentsByEntityByType[typeof(T)][entity]
            : throw new Exception($"Entity {entity} does not have component of type {typeof(T).Name}");

    public T? TryGetComponent<T>(long entity) where T : class =>
        componentsByEntityByType[typeof(T)].TryGetValue(entity, out var component)
            ? component as T
            : null;

    public World AddSystem(ISystem system) {
        systems.Add(system);
        return this;
    }

    public World AddSimulationSystems(params ISimulationSystem[] simulationSystems) {
        var simulation = new Simulation();
        foreach (var system in simulationSystems) {
            simulation.OnUpdate += system.Update;
        }

        addUpdatable(simulation);
        return this;
    }
}
