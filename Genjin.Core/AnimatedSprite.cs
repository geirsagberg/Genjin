namespace Genjin.Example;

public record AnimatedSprite<T> where T : notnull(SpriteSheet SpriteSheet, Dictionary<T, Animation> Animations);