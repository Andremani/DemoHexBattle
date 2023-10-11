using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wunderwunsch.HexMapLibrary;
using Wunderwunsch.HexMapLibrary.Generic;
using DG.Tweening;
using Andremani.DemoHexBattle.Pathfinding;
using Andremani.DemoHexBattle.Pathfinding.Heuristics;

namespace Andremani.DemoHexBattle
{
    public class BattleGrid : MonoBehaviour
    {
        public class BattleTileData
        {
            public bool isPassable;

            public GameObject tileFoundationInstance;

            public GameObject unitInstance = null; //
        }

        [SerializeField] private float oneHexMovementDuration = 0.2f;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(15, 11);
        [SerializeField] private GameObject tilePrefab = null;
        [SerializeField] private Material baseTileMaterial;
        [SerializeField] private GameObject reachableMarker = null;
        [SerializeField] private GameObject edgeReachableBorder = null;

        private HexMap<BattleTileData> hexMap;
        [HideInInspector] public HexMouse hexMouse;
        private List<GameObject> reachableTilesMarkers; // the gameObjects we use to visualise which tiles are within movementRange
        [SerializeField] private Camera battleCamera;

        private Astar astar;
        private List<Node> pathNodes;
        private bool isMovementHappening;

        public delegate void CallbackDelegate();
        public bool IsMovementHappening => isMovementHappening;

        void Awake()
        {
            hexMouse = gameObject.AddComponent<HexMouse>();
            hexMap = new HexMap<BattleTileData>(HexMapBuilder.CreateRectangularShapedMap(mapSize), null);

            SetupCamera();
        }

        public void Init()
        {
            hexMouse.Init(hexMap, battleCamera);
            CreateGridTiles();

            ClearReachableTileVisuals();
            reachableTilesMarkers = new List<GameObject>();
            astar = new AstarHexGrid(new HexManhattanHeuristic(), hexMap);
            pathNodes = new List<Node>();
        }

        private void CreateGridTiles()
        {
            foreach (var tile in hexMap.Tiles)
            {
                if (tile.Data.tileFoundationInstance != null)
                {
                    Destroy(tile.Data.tileFoundationInstance);
                }

                GameObject tileInstance = Instantiate(tilePrefab, transform);
                tileInstance.GetComponent<Renderer>().material = baseTileMaterial;
                tileInstance.name = "MapTile_" + tile.Position;
                tileInstance.transform.position = tile.CartesianPosition;

                tile.Data.isPassable = true;
                tile.Data.tileFoundationInstance = tileInstance;
            }
        }

        public Vector3Int CreateUnit(Vector2Int unitOffsetCoords, GameObject unitPrefab)
        {
            Vector3Int unitCubicCoords = HexConverter.OffsetTileCoordToTileCoord(unitOffsetCoords);
            Vector3 unitPosition = hexMap.TilesByPosition[unitCubicCoords].CartesianPosition + Vector3.up * 0.02f;
            Quaternion unitRotation = Quaternion.Euler(90, 180, 0);
            GameObject unitParentTile = hexMap.TilesByPosition[unitCubicCoords].Data.tileFoundationInstance;
            GameObject newUnit = Instantiate(unitPrefab, unitPosition, unitRotation, unitParentTile.transform);
            hexMap.TilesByPosition[unitCubicCoords].Data.unitInstance = newUnit;
            hexMap.TilesByPosition[unitCubicCoords].Data.isPassable = false;
            return unitCubicCoords;
        }

        public void DeleteUnit(Vector3Int tileCoords)
        {
            Destroy(hexMap.TilesByPosition[tileCoords].Data.unitInstance);
            hexMap.TilesByPosition[tileCoords].Data.unitInstance = null;
            hexMap.TilesByPosition[tileCoords].Data.isPassable = true;
        }

        public GameObject GetUnitInstanceInTile(Vector3Int tileCoords)
        {
            return hexMap.TilesByPosition[tileCoords].Data.unitInstance;
        }

        public MapSizeData GetMapSizeData()
        {
            return hexMap.MapSizeData;
        }

        public void MoveUnit(Vector3Int unitLocation, Vector3Int destinationTileCoord, CallbackDelegate callback)
        {
            isMovementHappening = true;

            GameObject unitInstance = hexMap.TilesByPosition[unitLocation].Data.unitInstance;
            hexMap.TilesByPosition[unitLocation].Data.unitInstance = null;
            hexMap.TilesByPosition[unitLocation].Data.isPassable = true;

            //unitInstance.transform.position = HexConverter.TileCoordToCartesianCoord(destinationTileCoord, 0.02f);
            StartCoroutine(MoveUnitRoutine(unitInstance, destinationTileCoord, callback));
        }

        private IEnumerator MoveUnitRoutine(GameObject unitGameObject, Vector3Int destinationTileCoord, CallbackDelegate callback)
        {
            for (int i = 1; i < pathNodes.Count; i++)
            {
                Vector3 currentPathPoint = HexConverter.OffsetTileCoordToCartesianCoord(new Vector2Int(pathNodes[i].X, pathNodes[i].Y));
                currentPathPoint.y += 0.2f;

                bool stepIsOver = false;
                Tween step = unitGameObject.transform.DOMove(currentPathPoint, oneHexMovementDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => stepIsOver = true);
                yield return new WaitUntil(() => stepIsOver);
            }

            hexMap.TilesByPosition[destinationTileCoord].Data.isPassable = false;
            hexMap.TilesByPosition[destinationTileCoord].Data.unitInstance = unitGameObject;
            isMovementHappening = false;
            callback?.Invoke();
        }

        public int IsTilePassable(Vector3Int hexTileCoords)
        {
            if (hexMap.TilesByPosition[hexTileCoords].Data.isPassable) { return 0; } // 0 is passable for A*
            else { return 0; }
        }

        public List<Vector3Int> GetTilesOnEdge(Vector3Int edgeCoord)
        {
            List<Vector3Int> tilesOnEdge = hexMap.GetTilePositions.AdjacentToEdge(edgeCoord);
            //foreach(var tilePos in tilesOnEdge)
            //{
            //    Instantiate(reachableMarker, HexConverter.TileCoordToCartesianCoord(tilePos), Quaternion.identity).GetComponent<Renderer>().material.color = Color.white;
            //}
            return tilesOnEdge;
        }

        private List<Vector3Int> CollectPassabilityData()
        {
            List<Vector3Int> nodes = new List<Vector3Int>();
            foreach (var tile in hexMap.Tiles)
            {
                int passability;
                if (tile.Data.isPassable)
                {
                    passability = 0; // passable
                }
                else
                {
                    passability = 1; // impassable
                }
                Vector2Int offsetCoord = HexConverter.TileCoordToOffsetTileCoord(tile.Position);
                nodes.Add(new Vector3Int(offsetCoord.x, offsetCoord.y, passability));
            }
            return nodes;
        }

        public bool CreatePath(Vector3Int startPos, Vector3Int endPos, int length = 200)
        {
            List<Vector3Int> nodes = CollectPassabilityData();

            pathNodes = astar.CreatePath(nodes, startPos, endPos, length);
            if (pathNodes == null)
            {
                return false;
            }
            return true;
        }

        public void DrawPath()
        {
            if (pathNodes == null || pathNodes.Count == 0)
            {
                Debug.LogError("BattleGrid - DrawPath() - pathNodes is null or path = 0");
                Debug.Break();
                return;
            }

            foreach (var node in pathNodes)
            {
                var tile = hexMap.TilesByPosition[HexConverter.OffsetTileCoordToTileCoord(new Vector2Int(node.X, node.Y))];
                tile.Data.tileFoundationInstance.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        public void ClearPath()
        {
            foreach (var tile in hexMap.Tiles)
            {
                tile.Data.tileFoundationInstance.GetComponent<Renderer>().material.color = Color.black;
            }
        }

        public void DrawReachableTiles(Vector3Int startingTile, int distance, Color? nullableColor = null)
        {
            List<Tile<BattleTileData>> reachableTiles = CalculateReachableTiles(startingTile, distance);
            Color color = nullableColor ?? Color.white;
            UpdateReachableTileVisuals(reachableTiles, color);
        }
        private List<Tile<BattleTileData>> CalculateReachableTiles(Vector3Int startTile, int movementPoints)
        {
            //this is a very simple and inefficient solution to the problem, but our main focus is using the hex library.
            //in a real implementation you want to use a pathfinding algorithm like A*Star and better suited data structures like a priority queue.
            Dictionary<Tile<BattleTileData>, int> costToTile = new Dictionary<Tile<BattleTileData>, int>();
            Queue<Tile<BattleTileData>> openTiles = new Queue<Tile<BattleTileData>>();
            var currentTile = hexMap.TilesByPosition[startTile];
            //if (!currentTile.Data.isPassable) return new List<Tile<MyTile>>(); //we return an empty list if the start tile is unpassable.
            openTiles.Enqueue(currentTile);
            costToTile.Add(currentTile, 0);
            int safeGuard = 0;
            while (openTiles.Count > 0 && safeGuard < 1000)
            {
                safeGuard++;
                currentTile = openTiles.Dequeue();
                int costToCurTile = costToTile[currentTile];
                var neighbours = hexMap.GetTiles.AdjacentToTile(currentTile);
                foreach (var neighbour in neighbours)
                {

                    int newCost = costToCurTile;
                    newCost += neighbour.Data.isPassable ? 1 : 999;
                    //if (neighbour.Data.hasForest) newCost += extraCostForest;

                    if (newCost > movementPoints) continue;

                    int oldCost = costToTile.ContainsKey(neighbour) ? costToTile[neighbour] : int.MaxValue;
                    if (newCost >= oldCost) continue; //we have been on this tile earlier with a lower cost;

                    openTiles.Enqueue(neighbour);
                    costToTile[neighbour] = newCost; //using the index adds the key/value pair if it is not in the dictionary and updates it if it already exists 
                }
            }

            List<Tile<BattleTileData>> tilesInRange = costToTile.Keys.ToList();
            return tilesInRange;
        }
        private void UpdateReachableTileVisuals(List<Tile<BattleTileData>> reachableTiles, Color borderColor)
        {
            ClearReachableTileVisuals();

            foreach (Tile<BattleTileData> tile in reachableTiles)
            {
                GameObject tileObj = Instantiate(reachableMarker, tile.CartesianPosition, Quaternion.identity, transform);
                tileObj.transform.position += new Vector3(0, 0.1f, 0); //0.1f = explicitly set y-Coord of the tile so it is slightly above the tiles of the map
                reachableTilesMarkers.Add(tileObj);
            }

            List<Vector3Int> borderEdges = hexMap.GetEdgePositions.TileBorders(reachableTiles);

            foreach (var edgePos in borderEdges)
            {
                EdgeAlignment orientation = HexUtility.GetEdgeAlignment(edgePos);
                float angle = HexUtility.anglebyEdgeAlignment[orientation];
                GameObject edgeObj = Instantiate(edgeReachableBorder, HexConverter.EdgeCoordToCartesianCoord(edgePos) + new Vector3(0, -0.04f, 0), Quaternion.Euler(0, angle, 0), transform);
                edgeObj.GetComponent<Renderer>().material.color = borderColor;
                reachableTilesMarkers.Add(edgeObj);
            }
        }
        private void ClearReachableTileVisuals()
        {
            if(reachableTilesMarkers != null)
            {
                foreach (GameObject g in reachableTilesMarkers)
                {
                    Destroy(g);
                }
                reachableTilesMarkers.Clear();
            }
        }

        private void SetupCamera()
        {
            battleCamera.transform.position = new Vector3(hexMap.MapSizeData.center.x, 4, hexMap.MapSizeData.center.z);
            battleCamera.orthographic = true;
            battleCamera.transform.rotation = Quaternion.Euler(90, 180, 0); //rotates the camera to it looks at the XZ-plane
            battleCamera.orthographicSize = hexMap.MapSizeData.extents.z * 2 * 0.7f;
        }
    }
}
