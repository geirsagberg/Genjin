namespace Genjin.Example;

public record AnimatedSprite<T>(SpriteSheet SpriteSheet, Dictionary<T, Animation> Animations) where T : notnull;

public interface INotification
{
}
