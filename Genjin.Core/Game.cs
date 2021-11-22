namespace Genjin.Core;

public class Game
{
    private int y = 1;

    public void Start()
    {
        Console.WriteLine("Counter: " + y++);
        Thread.Sleep(1000);
    }
}
