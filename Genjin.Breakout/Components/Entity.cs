namespace Genjin.Breakout.Components;

public class Entity {
    public Entity(long id, World world) {
        Id = id;
        World = world;
    }

    public long Id { get; }

    private World World { get; }

    public T Get<T>() => World.GetComponent<T>(Id);
    public void Add<T>(T component) where T : notnull => World.AddComponent(Id, component);
}
