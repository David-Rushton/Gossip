namespace Gossip.Model;

public class Node
{
    private readonly Random _random = new();

    public int GossipCounter { get; private set; } = 0;

    public void PostGossip() =>
        GossipCounter++;

    public IEnumerable<int> ProcessGossip(Node[] nodes)
    {
        if (GossipCounter == 0)
            yield break;

        foreach (var _ in Enumerable.Range(0, 1))
        {
            var index = _random.Next(nodes.Count());
            nodes[index].PostGossip();
            yield return index;
        }
    }
}
