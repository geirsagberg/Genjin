namespace Genjin.Core.Entities;

public class Entity {
    public Entity(long id, World world) {
        Id = id;
        World = world;
    }

    public long Id { get; }

    private World World { get; }

    public T GetComponent<T>() => World.GetComponent<T>(Id);
    public void AddComponent<T>(T component) where T : class => World.AddComponent(Id, component);

    public T? TryGetComponent<T>() where T : class => World.TryGetComponent<T>(Id);
}
