namespace Genjin.Example;

public enum AnimationMode {
    Once,
    Loop,
    PingPong
}

public record Animation(Range Frames, float Fps = 15, AnimationMode Mode = AnimationMode.Loop);
