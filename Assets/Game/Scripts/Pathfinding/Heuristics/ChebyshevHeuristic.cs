using System;

namespace Andremani.DemoHexBattle.Pathfinding.Heuristics
{
    public class ChebyshevHeuristic : Heuristic
    {
        const int movementCost = 1;

        public override HeuristicTypes HeuristicType => HeuristicTypes.SquareGrid;
        public override int DefaultMoveCost => movementCost;
        public override int DiagonalMoveCost => movementCost;

        public override int GetHeuristic(Node a, Node b)
        {
            var D = movementCost;
            var D2 = movementCost;
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            var result = (int)(D * (dx + dy) + (D2 - 2 * D));
            return result;
        }
    }
}
