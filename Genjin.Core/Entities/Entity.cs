namespace Genjin.Core.Entities;

public class Entity {
    public Entity(long id, World world) {
        Id = id;
        World = world;
    }

    public long Id { get; }

    private World World { get; }

    public T Get<T>() => World.GetComponent<T>(Id);
    public void Add<T>(T component) where T : class => World.AddComponent(Id, component);

    public T? TryGet<T>() where T : class => World.TryGetComponent<T>(Id);
}
