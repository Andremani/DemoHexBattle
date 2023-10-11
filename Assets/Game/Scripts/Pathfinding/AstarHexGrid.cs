using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wunderwunsch.HexMapLibrary;
using Wunderwunsch.HexMapLibrary.Generic;
using Andremani.DemoHexBattle.Pathfinding.Heuristics;

namespace Andremani.DemoHexBattle.Pathfinding
{
    public class AstarHexGrid : Astar
    {
        private HexMap<BattleGrid.BattleTileData> hexMap;
        private List<Node> nodes;

        public AstarHexGrid(Heuristic heuristic, HexMap<BattleGrid.BattleTileData> _hexMap) : base(heuristic)
        {
            hexMap = _hexMap;
        }

        public override List<Node> CreatePath(List<Vector3Int> grid, Vector3Int start, Vector3Int end, int length)
        {
            Node endNode = null;
            Node startNode = null;

            InitGraph(grid, start, end, ref startNode, ref endNode);

            return FindPath(startNode, endNode, length);
        }

        /// <summary>
        /// Create nodes from grid and connect them with neighbouring (AddBeighboursToNode)
        /// </summary>
        private void InitGraph(List<Vector3Int> grid, Vector3Int start, Vector3Int end, ref Node startNode, ref Node endNode)
        {
            nodes = new List<Node>(grid.Capacity);
            foreach (Vector3Int elem in grid)
            {
                nodes.Add(new Node(elem.x, elem.y, elem.z));
            }

            foreach (Node node in nodes)
            {
                AddNeighboorsToNode(node);
                if (node.X == start.x && node.Y == start.y)
                {
                    startNode = node;
                }
                if (node.X == end.x && node.Y == end.y)
                {
                    endNode = node;
                }
            }
        }

        protected void AddNeighboorsToNode(Node node)
        {
            Vector3Int cubeCoords = HexConverter.OffsetTileCoordToTileCoord(new Vector2Int(node.X, node.Y));
            Tile tile = hexMap.Tiles[hexMap.TileIndexByPosition[cubeCoords]];
            // or this // tile = hexMap.TilesByPosition[cubeCoords];

            //hexMap.GetTile.FromCartesianCoordinate

            var neighbours = hexMap.GetTiles.AdjacentToTile(tile);
            foreach(var n in neighbours)
            {
                Vector2Int neighbourOffsetCoord = HexConverter.TileCoordToOffsetTileCoord(n.Position);
                Node neigbouringNode = nodes.Find(n => new Vector2Int(n.X, n.Y) == neighbourOffsetCoord);
                if(neigbouringNode != null)
                {
                    node.neighboors.Add(neigbouringNode);
                }
            }
        }
    }
}