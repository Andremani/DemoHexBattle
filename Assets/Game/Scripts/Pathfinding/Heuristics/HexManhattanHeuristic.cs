using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wunderwunsch.HexMapLibrary;

namespace Andremani.DemoHexBattle.Pathfinding.Heuristics
{
    public class HexManhattanHeuristic : Heuristic
    {
        const int straightMoveCost = 1;

        public override HeuristicTypes HeuristicType => HeuristicTypes.HexGrid;
        public override int DefaultMoveCost => straightMoveCost;
        public override int DiagonalMoveCost => straightMoveCost;

        public override int GetHeuristic(Node a, Node b)
        {
            Vector3Int a_cubeCoords = HexConverter.OffsetTileCoordToTileCoord(new Vector2Int(a.X, a.Y));
            Vector3Int b_cubeCoords = HexConverter.OffsetTileCoordToTileCoord(new Vector2Int(b.X, b.Y));

            var D = straightMoveCost;
            var result = D * Mathf.Max(Mathf.Abs(a_cubeCoords.x - b_cubeCoords.x), Mathf.Abs(a_cubeCoords.y - b_cubeCoords.y), Mathf.Abs(a_cubeCoords.z - b_cubeCoords.z));
            return result;
        }
    }
}