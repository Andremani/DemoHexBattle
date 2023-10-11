using System;

namespace Andremani.DemoHexBattle.Pathfinding.Heuristics
{
    public class ManhattanHeuristic : Heuristic
    {
        const int movementCost = 100;

        public override HeuristicTypes HeuristicType => HeuristicTypes.SquareGrid;
        public override int DefaultMoveCost => movementCost;
        public override int DiagonalMoveCost => movementCost;

        public override int GetHeuristic(Node a, Node b)
        {
            var D = movementCost;
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            return D * (dx + dy);
        }
    }
}