using System;

namespace Andremani.DemoHexBattle.Pathfinding.Heuristics
{
    public class OctileHeuristic : Heuristic
    {
        const int straightMoveCost = 100;
        const int diagonalMoveCost = 141;

        public override HeuristicTypes HeuristicType => HeuristicTypes.SquareGrid;
        public override int DefaultMoveCost => straightMoveCost;
        public override int DiagonalMoveCost => diagonalMoveCost;

        public override int GetHeuristic(Node a, Node b)
        {
            var D = straightMoveCost;
            var D2 = diagonalMoveCost;
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            var result = (int)(D * (dx + dy) + (D2 - 2 * D));
            return result;
        }
    }
}