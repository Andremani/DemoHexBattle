namespace Andremani.DemoHexBattle.Pathfinding.Heuristics
{
    public abstract class Heuristic
    {
        public abstract HeuristicTypes HeuristicType { get; }
        public abstract int DefaultMoveCost { get; }
        public abstract int DiagonalMoveCost { get; }

        public abstract int GetHeuristic(Node a, Node b);
    }
}