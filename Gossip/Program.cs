using Gossip.Model;
using System.Diagnostics;

const int NodeCount = 3000;
const int MinimumRoundIntervalMs = 250;

ConfigureDisplay();
PlayGame(CreateNodes());

return;

static Node[] CreateNodes() =>
    Enumerable
        .Range(0, NodeCount)
        .Select(_ => new Node())
        .ToArray();

static void ConfigureDisplay()
{
    Console.Clear();
    Console.CursorVisible = false;
    Console.CancelKeyPress += (_, _) => Console.CursorVisible = true;
}

static void PlayGame(Node[] nodes)
{
    var round = 0;
    var roundTimer = new Stopwatch();

    // Seed gossip.
    var seed = new Random().Next(nodes.Count());
    nodes[seed].PostGossip();

    var heardGossip = new HashSet<int>(seed);
    while (heardGossip.Count < nodes.Count())
    {
        round++;
        roundTimer.Restart();

        Console.SetCursorPosition(0, 0);
        Console.WriteLine(" Gossip");
        Console.WriteLine($" Round: {round}");
        Console.WriteLine($" Gossip Heard: {heardGossip.Count}   ");
        Console.WriteLine("---------------------------------");
        Console.WriteLine();

        foreach (var node in nodes)
        {
            foreach (var id in node.ProcessGossip(nodes))
            {
                var y = (id / 100) + 5;
                var x = (id % 100) + 1;

                Console.SetCursorPosition(x, y);
                Console.Write($"{ToAnsiRgb(ToRgb(nodes[id].GossipCounter))} \x1b[0m");

                heardGossip.Add(id);
                Console.SetCursorPosition(0, 2);
                Console.WriteLine($" Gossip Heard: {heardGossip.Count}   ");
            }
        }

        // The early rounds can pass too quickly without this.
        roundTimer.Stop();
        if (roundTimer.ElapsedMilliseconds < MinimumRoundIntervalMs)
            Thread.Sleep(
                TimeSpan.FromMilliseconds(MinimumRoundIntervalMs - roundTimer.ElapsedMilliseconds));
    }

    Console.SetCursorPosition(0, 36);
    Console.WriteLine("-----------");
    Console.WriteLine(" Game Over");

    static (int red, int green, int blue) ToRgb(int value)
    {
        var offset = value * 30;
        return (
            red: Math.Min(offset, 255),
            green: 10,
            blue: Math.Min(offset, 255));
    }

    static string ToAnsiRgb((int red, int green, int blue) colours) =>
        $"\x1b[48;2;{colours.red};{colours.green};{colours.blue}m";
}
